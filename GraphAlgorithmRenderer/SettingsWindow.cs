//#define TEST

using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using EnvDTE;
using GraphAlgorithmRendererLib.Config;
using GraphAlgorithmRendererLib.GraphRenderer;
using GraphAlgorithmRendererLib.Serializer;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using WpfExceptionViewer;
using GraphRenderer = GraphAlgorithmRendererLib.GraphRenderer.GraphRenderer;
using MessageBox = System.Windows.MessageBox;
using Process = EnvDTE.Process;
using Size = System.Drawing.Size;
using StackFrame = EnvDTE.StackFrame;


namespace GraphAlgorithmRenderer
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using GraphConfig = GraphConfig;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("52a96019-a6a1-4390-a42a-d6f3b1e160cc")]
    public class SettingsWindow : ToolWindowPane
    {
        private enum DrawingMode
        {
            ShouldBeRedrawn,
            Redrawing,
            NotChanged,
            Canceled
        }

        public delegate void MakeAction();

        public delegate GraphConfig CreateConfig();

        private void SetConfig(CreateConfig createConfig)
        {
            _config = createConfig();
            _config.Validate();
        }


        private void HandleException(MakeAction makeAction, string headerMessage)
        {
            try
            {
                makeAction();
            }
            catch (Exception e)
            {
                if (_drawingMode != DrawingMode.Canceled)
                {
                    _drawingMode = DrawingMode.NotChanged;
                }

                if (e is GraphRenderException || e is ValidationException)
                {
                    MessageBox.Show(e.Message, headerMessage, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var exceptionViewer = new ExceptionViewer(headerMessage, e);
                    exceptionViewer.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        public SettingsWindow() : base(null)
        {
            this.Caption = "Graph Visualization Settings";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            _control = new SettingsWindowControl();
            base.Content = _control;
            _control.Load.Click += LoadOnClick;
            _control.Export.Click += ExportOnClick;
        }

        private void ExportOnClick(object sender, RoutedEventArgs e)
        {
            _control.Config.Text = ConfigSerializer.ToJson(_config);
        }

        private void LoadOnClick(object sender, RoutedEventArgs e)
        {
            
            var json = _control.Config.Text;
            if (String.IsNullOrWhiteSpace(json))
            {
                var res = MessageBox.Show("Cannot deserialize an empty JSON!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            HandleException(() =>
            {
                SetConfig(() => ConfigSerializer.FromJson(json));
                ((SettingsWindowPackage)Package).OptionJsonConfig = json;
                MessageBox.Show("Successfully deserialized config!", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }, "Json deserialization error");
           
            HandleException(() => _control.MainControl.FromConfig(_config), "Oups! Something went wrong :(");


            CreateForm();
            if (_drawingMode == DrawingMode.NotChanged)
            {
                _drawingMode = DrawingMode.ShouldBeRedrawn;
                HandleException(DrawGraph, "Error while drawing graph");
            }
        }

        private void CreateForm()
        {
            if (_drawingMode != DrawingMode.Canceled && _form != null)
            {
                return;
            }
            _form = new Form { Size = new Size(800, 800) };
            _form.FormClosed += (sender, args) => _drawingMode = DrawingMode.Canceled;
            _form.TopMost = _control.MainControl.OnTop.IsChecked == true;
        }

        private void InitializeDebuggerOperations()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            var dte = (DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            _debugEvents = dte.Events.DebuggerEvents;
            _debugEvents.OnContextChanged +=
                Update;
            var debugger = dte.Debugger;
            EnvDTE.Window w = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            w.Visible = true;
            OutputWindow ow = (OutputWindow) w.Object;
            var outputWindowPane = ow.OutputWindowPanes.Add("Graph Algorithm Renderer");
            outputWindowPane.Activate();
            _debuggerOperations = new DebuggerOperations(debugger, outputWindowPane);
        }

        protected override void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            InitializeDebuggerOperations();
            _graphRenderer = new GraphRenderer(_debuggerOperations);
            ((SettingsWindowControl)Content).Config.Text = ((SettingsWindowPackage)Package).OptionJsonConfig;
            
            _control.MainControl.GenerateConfig.Click += GenerateConfigOnClick;
            _control.MainControl.ShowGraph.Click += (sender, e) =>
            {
                if (_drawingMode == DrawingMode.Canceled)
                {
                    CreateForm();
                    _drawingMode = DrawingMode.ShouldBeRedrawn;
                    HandleException(DrawGraph, "Error while drawing graph");
                }
            };

          
            _control.MainControl.OnTop.Checked += (sender, args) =>
            {
                if (_form != null)
                {
                    _form.TopMost = true;
                }
            };
            _control.MainControl.OnTop.Unchecked += (sender, args) =>
            {
                if (_form != null)
                {
                    _form.TopMost = false;
                }
            };
        }

        private void GenerateConfigOnClick(object sender, RoutedEventArgs e)
        {
            HandleException(() =>
            {
                SetConfig(() => _control.MainControl.Config);
                ((SettingsWindowPackage)Package).OptionJsonConfig = ConfigSerializer.ToJson(_config);
                MessageBox.Show("Successfully created config!", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }, "Error while generating config");

            CreateForm();
           // if (_drawingMode == DrawingMode.Redrawing) return;
            _drawingMode = DrawingMode.ShouldBeRedrawn;
            HandleException(DrawGraph, "Error while drawing graph");
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (_drawingMode != DrawingMode.ShouldBeRedrawn)
            {
                return;
            }

            HandleException(DrawGraph, "Error while drawing graph");
        }

        private void Update(Process newprocess, Program newprogram, Thread newthread, StackFrame newstackframe)
        {
            if (_drawingMode == DrawingMode.Canceled || _drawingMode == DrawingMode.Redrawing)
            {
                return;
            }

            if (newstackframe == null)
            {
                _drawingMode = DrawingMode.NotChanged;
                _form?.Hide();
                return;
            }

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            _dispatcherTimer.Start();
            _drawingMode = DrawingMode.ShouldBeRedrawn;
        }

        private GraphConfig _config;
        private Form _form;
        private DebuggerEvents _debugEvents;
        private DrawingMode _drawingMode = DrawingMode.NotChanged;
        private DispatcherTimer _dispatcherTimer;
        private readonly SettingsWindowControl _control;
        private DebuggerOperations _debuggerOperations;
        private GraphRenderer _graphRenderer;


        private void DrawGraph()
        {
            _debuggerOperations.ClearOutput();
            _drawingMode = DrawingMode.Redrawing;
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_config == null || _debuggerOperations?.IsActive != true)
            {
                _drawingMode = DrawingMode.NotChanged;
                return;
            }
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var graph = _graphRenderer.RenderGraph(_config);
            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

            Debug.WriteLine($"total time {elapsedTime}");
            if (graph == null)
            {
                return;
            }
            GViewer viewer = new GViewer {Graph = graph, Dock = DockStyle.Fill};
            CreateForm();

             _form.SuspendLayout();
            _form.Controls.Clear();
            _form.Controls.Add(viewer);
            _form.ResumeLayout();
            if (!_form.Visible)
            {
                _form.Show();
            }

            //_form.Focused = false;
            _dispatcherTimer?.Stop();
            _drawingMode = DrawingMode.NotChanged;
        }

        public string JsonConfig => ConfigSerializer.ToJson(_config);
        
    }
}