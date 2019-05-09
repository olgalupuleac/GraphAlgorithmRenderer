//#define TEST
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using EnvDTE;
using GraphAlgorithmRenderer.Config;
using GraphAlgorithmRenderer.ConfigSamples;
using GraphAlgorithmRenderer.Serializer;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.VisualStudio.Shell;
using WpfExceptionViewer;
using Condition = GraphAlgorithmRenderer.Config.Condition;
using Debugger = EnvDTE.Debugger;
using GraphRenderer = GraphAlgorithmRenderer.GraphRenderer.GraphRenderer;
using MessageBox = System.Windows.MessageBox;
using Process = EnvDTE.Process;
using Size = System.Drawing.Size;
using StackFrame = EnvDTE.StackFrame;



namespace GraphAlgorithmRenderer
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using GraphConfig = Config.GraphConfig;
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
            NotChanged,
            Canceled
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
            try
            {
                _config = ConfigSerializer.FromJson(json);
                
                MessageBox.Show("Successfully deserialized config!", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                var exceptionViewer = new ExceptionViewer("Json deserialization error", exception);
                exceptionViewer.ShowDialog();
            }

            try
            {
                _control.MainControl.FromConfig(_config);
            }
            catch (Exception exception)
            {
                var exceptionViewer = new ExceptionViewer("Oups! Something went wrong :(", exception);
                exceptionViewer.ShowDialog();
            }


            if (_drawingMode == DrawingMode.Canceled)
            {
                CreateForm();
            }

            _drawingMode = DrawingMode.ShouldBeRedrawn;
        }

        private void CreateForm()
        {
            _form = new Form { Size = new Size(800, 800) };
            _form.FormClosed += (sender, args) => _drawingMode = DrawingMode.Canceled;
        }

        protected override void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var applicationObject = (DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            _debugEvents = applicationObject.Events.DebuggerEvents;
            _debugEvents.OnContextChanged +=
                Update;
            _debugger = applicationObject.Debugger;
            var config = ConfigCreator.TreapConfig;
            Debug.WriteLine(ConfigSerializer.ToJson(config));
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            _dispatcherTimer.Start();
            _control.MainControl.GenerateConfig.Click += GenerateConfigOnClick;
        }

        private void GenerateConfigOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _config = _control.MainControl.Config;
                MessageBox.Show("Successfully created config!", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception exception)
            {
                var exceptionViewer = new ExceptionViewer("Error while creating config", exception);
                exceptionViewer.ShowDialog();
            }

            if (_drawingMode == DrawingMode.Canceled)
            {
                CreateForm();
            }

            _drawingMode = DrawingMode.ShouldBeRedrawn;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (_drawingMode != DrawingMode.ShouldBeRedrawn)
            {
                return;
            }

            DrawGraph();
            _drawingMode = DrawingMode.NotChanged;
        }

        private void Update(Process newprocess, Program newprogram, Thread newthread, StackFrame newstackframe)
        {
            if (_drawingMode == DrawingMode.Canceled)
            {
                return;
            }
            if (newstackframe == null)
            {
                _drawingMode = DrawingMode.NotChanged;
                _form.Hide();
                return;
            }

            _drawingMode = DrawingMode.ShouldBeRedrawn;
        }

        private GraphConfig _config;

        private Form _form;

        //private GraphRenderer.GraphRenderer _renderer;
        private DebuggerEvents _debugEvents;
        private DrawingMode _drawingMode = DrawingMode.NotChanged;
        private DispatcherTimer _dispatcherTimer;
        private readonly SettingsWindowControl _control;
        private Debugger _debugger;


        private void DrawGraph()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_debugger.CurrentStackFrame == null)
            {
                return;
            }
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            if (_config == null || _debugger == null)
            {
                return;
            }

            var renderer = new GraphRenderer.GraphRenderer(_config, _debugger);
            Graph graph = renderer.RenderGraph();
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Debug.WriteLine($"total time {elapsedTime}");
            GViewer viewer = new GViewer { Graph = graph, Dock = DockStyle.Fill };
            if (_form == null)
            {
                CreateForm();
            }

            _form.SuspendLayout();
            _form.Controls.Clear();
            _form.Controls.Add(viewer);
            _form.ResumeLayout();
            _form.Show();
        }
    }
}