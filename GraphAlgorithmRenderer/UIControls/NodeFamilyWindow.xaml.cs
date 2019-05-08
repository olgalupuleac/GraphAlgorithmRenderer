using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GraphAlgorithmRenderer.Config;
using static System.String;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for NodeFamilyWindow.xaml
    /// </summary>
    public partial class NodeFamilyWindow : Window
    {

        public NodeFamilyWindow()
        {
            InitializeComponent();

            PropertiesControl.WindowGenerator = () => new NodeConditionalPropertyWindow();
            PropertiesControl.Description = w => ((NodeConditionalPropertyWindow)w).ConditionControl.Description;
            PropertiesControl.UpdateDescription = (w, i) =>
            {
                ((NodeConditionalPropertyWindow)w).ok.Click += (o, sender) =>
                    i.Content = ((NodeConditionalPropertyWindow)w).ConditionControl.Description;

            };
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true; // cancels the window close    
            Hide(); // Programmatically hides the window
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

            Hide();
        }

        

        public NodeFamily NodeFamily
        {
            get
            {
                var conditionalProperties = PropertiesControl.Windows.Cast<NodeConditionalPropertyWindow>()
                    .Select(w => w.ConditionalProperty).ToList();
                conditionalProperties.Reverse();
                //TODO check for null
                return new NodeFamily(IdentifierPartRangeControl.Ranges.ToList())
                {
                    Name = familyName.Text,
                    ValidationTemplate = validationTemplateBox.Text,
                    ConditionalProperties = conditionalProperties
                };
            }
        }

        public void FromNodeFamily(NodeFamily nodeFamily)
        {
            IdentifierPartRangeControl.FromRanges(nodeFamily.Ranges);
            validationTemplateBox.Text = nodeFamily.ValidationTemplate;
            familyName.Text = nodeFamily.Name;
            var conditionalProperties = ((IEnumerable<ConditionalProperty<INodeProperty>>) nodeFamily.ConditionalProperties)
                .Reverse().ToList();
            var windows = new List<Window>();
            foreach (var conditionalProperty in conditionalProperties)
            {
                var window = new NodeConditionalPropertyWindow();
                window.FromConditionalProperty(conditionalProperty);
                windows.Add(window);
            }
            PropertiesControl.SetNewWindows(windows);
        }
    }
}