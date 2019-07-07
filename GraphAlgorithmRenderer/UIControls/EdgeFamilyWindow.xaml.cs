using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphAlgorithmRendererLib.Config;
using static System.String;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for EdgeFamilyWindow.xaml
    /// </summary>
    public partial class EdgeFamilyWindow : Window
    {
        private EdgeEndControl _targetWindow;
        private EdgeEndControl _sourceWindow;

        public bool NameIsSet { get; set; } = true;

        public EdgeFamilyWindow(List<NodeFamilyWindow> availableNodes)
        {
            
            InitializeComponent();
            PropertiesControl.WindowGenerator = () => new EdgeConditionalPropertyWindow();
            PropertiesControl.Description = w => ((EdgeConditionalPropertyWindow) w).ConditionControl.Description;
            PropertiesControl.UpdateDescription = (w, i) =>
            {
                ((EdgeConditionalPropertyWindow) w).ok.Click += (o, sender) =>
                    i.Content = ((EdgeConditionalPropertyWindow) w).ConditionControl.Description;
            };
            SetRadioButtons(availableNodes);
            FamilyName.TextChanged += (sender, args) => NameIsSet = true;
            this.PreviewKeyDown += CloseOnEscape;
        }

        public void SetRadioButtons(List<NodeFamilyWindow> nodeFamilyWindows)
        {
            foreach (var nodeWindow in nodeFamilyWindows)
            {
                var nodeName = nodeWindow.FamilyName.Text;
                if (TargetPanel.Children.Cast<RadioButton>().Any(rb => ((string) rb.Content).Equals(nodeName)))
                {
                    continue;
                }
                var targetRadioButton = new RadioButton { Content = nodeName, GroupName = $"TargetNodes{GetHashCode()}" };
                targetRadioButton.Checked += (sender, args) =>
                {
                    _targetWindow =
                        new EdgeEndControl(
                            nodeWindow.IdentifierPartRangeControl.Ranges
                                .Where(id => !IsNullOrEmpty(id.Name)).Select(id => id.Name).ToList(), nodeName);
                };

                TargetPanel.Children.Add(targetRadioButton);
                var sourceRadioButton = new RadioButton { Content = nodeName, GroupName = $"SourceNodes{GetHashCode()}" };
                sourceRadioButton.Checked += (sender, args) =>
                    _sourceWindow =
                        new EdgeEndControl(
                            nodeWindow.IdentifierPartRangeControl.Ranges.Where(id => !IsNullOrEmpty(id.Name))
                                .Select(id => id.Name).ToList(), nodeName);
                
                nodeWindow.ok.Click += (sender, args) =>
                {
                    sourceRadioButton.Content = nodeWindow.FamilyName.Text;
                    targetRadioButton.Content = nodeWindow.FamilyName.Text;
                };

                SourcePanel.Children.Add(sourceRadioButton);
            }

            var targetButtonsToRemove = new List<RadioButton>();
            foreach (RadioButton rb in TargetPanel.Children)
            {
                if (!nodeFamilyWindows.Any(w => ((string)rb.Content).Equals(w.FamilyName.Text)))
                {
                    targetButtonsToRemove.Add(rb);
                }
            }
            targetButtonsToRemove.ForEach(rb => TargetPanel.Children.Remove(rb));
            var sourceButtonsToRemove = new List<RadioButton>();
            foreach (RadioButton rb in SourcePanel.Children)
            {
                if (!nodeFamilyWindows.Any(w => ((string)rb.Content).Equals(w.FamilyName.Text)))
                {
                    sourceButtonsToRemove.Add(rb);
                }
            }
            sourceButtonsToRemove.ForEach(rb => SourcePanel.Children.Remove(rb));
        }

        private void CloseOnEscape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Hide();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            ShowEdgeEnd(_targetWindow, "Target");
        }

        private void ShowEdgeEnd(EdgeEndControl window, string type)
        {
            if (window == null)
            {
                MessageBox.Show($"{type} node family is not defined", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            window.Show();
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            ShowEdgeEnd(_sourceWindow, "Source");
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            foreach (var range in IdentifierPartRangeControl.Ranges)
            {
                if (IsNullOrEmpty(range.Name) ||
                    !IsNullOrEmpty(range.BeginTemplate) && !IsNullOrEmpty(range.EndTemplate)) continue;
                MessageBox.Show($"Identifier range {range.Name} is not finished", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (_sourceWindow == null)
            {
                MessageBox.Show("Source node is not specified", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (_targetWindow == null)
            {
                MessageBox.Show("Target node is not specified", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Hide();
        }


        public EdgeFamily EdgeFamily
        {
            get
            {
                var conditionalProperties = PropertiesControl.Windows.Cast<EdgeConditionalPropertyWindow>()
                    .Select(w => w.ConditionalProperty).ToList();

                //TODO check for null
                return new EdgeFamily(IdentifierPartRangeControl.Ranges.ToList(), _sourceWindow.EdgeEnd,
                    _targetWindow.EdgeEnd)
                {
                    Name = FamilyName.Text,
                    ValidationTemplate = validationTemplateBox.Text,
                    ConditionalProperties = conditionalProperties
                };
            }
        }

        private void SetNodeFamilies(string target, string source)
        {
            Debug.WriteLine($"target {target}");
            foreach (var child in TargetPanel.Children)
            {
                if (child is RadioButton rb)
                {
                    Debug.WriteLine($"{rb.Content}");
                    if ((string) rb.Content != target)
                    {
                        continue;
                    }

                    rb.IsChecked = true;
                }
            }

            foreach (var child in SourcePanel.Children)
            {
                if (child is RadioButton rb && (string) rb.Content == source)
                {
                    rb.IsChecked = true;
                }
            }
        }

        public void FromEdgeFamily(EdgeFamily edgeFamily)
        {
            IdentifierPartRangeControl.FromRanges(edgeFamily.Ranges);
            validationTemplateBox.Text = edgeFamily.ValidationTemplate;
            FamilyName.Text = edgeFamily.Name;

            SetNodeFamilies(edgeFamily.Target.NodeFamilyName, edgeFamily.Source.NodeFamilyName);
            _targetWindow = new EdgeEndControl(edgeFamily.Target);
            _sourceWindow = new EdgeEndControl(edgeFamily.Source);
            _targetWindow.EdgeEndIdParts.ForEach(x => Debug.WriteLine($"{x.IdPart}, {x.Template}"));

            var windows = new List<Window>();
            foreach (var conditionalProperty in edgeFamily.ConditionalProperties)
            {
                var window = new EdgeConditionalPropertyWindow();
                window.FromConditionalProperty(conditionalProperty);
                windows.Add(window);
            }

            PropertiesControl.SetNewWindows(windows);
        }
    }
}