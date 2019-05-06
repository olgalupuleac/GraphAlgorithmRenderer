using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphAlgorithmRenderer.Config;
using static System.String;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for EdgeFamilyWindow.xaml
    /// </summary>
    public partial class EdgeFamilyWindow : Window
    {
        public ObservableCollection<IdentifierPartTemplate> Ranges { get; set; }
        private readonly Dictionary<string, NodeFamilyWindow> _availableNodes;
        private EdgeEndControl _targetWindow;
        private EdgeEndControl _sourceWindow;

        public EdgeFamilyWindow(Dictionary<string, NodeFamilyWindow> availableNodes)
        {
            _availableNodes = availableNodes;
            InitializeComponent();
            Ranges = new ObservableCollection<IdentifierPartTemplate>();
            identifiers.ItemsSource = Ranges;
            foreach (var node in _availableNodes.Keys)
            {
                var targetRadioButton = new RadioButton {Content = node, GroupName = "TargetNodes"};
                targetRadioButton.Checked += (sender, args) => 
                    _targetWindow = new EdgeEndControl(_availableNodes[node].Ranges.Where(id => !IsNullOrEmpty(id.Name)).Select(id => id.Name).ToList()); 
                TargetPanel.Children.Add(targetRadioButton);
                var sourceRadioButton = new RadioButton {Content = node, GroupName = "SourceNodes"};
                sourceRadioButton.Checked += (sender, args) =>
                    _sourceWindow = new EdgeEndControl(_availableNodes[node].Ranges.Where(id => !IsNullOrEmpty(id.Name)).Select(id => id.Name).ToList());
                SourcePanel.Children.Add(sourceRadioButton);
            }
        }
       
        private void AddId_Click(object sender, RoutedEventArgs e)
        {
            Ranges.Add(new IdentifierPartTemplate());
        }

        private void RemoveId_Click(object sender, RoutedEventArgs e)
        {
            if (identifiers.SelectedItem is IdentifierPartTemplate id)
            {
                Ranges.Remove(id);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true; // cancels the window close    
            Hide(); // Programmatically hides the window
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
            var w = new EdgeConditionalPropertyWindow();
            w.Show();
        }
    }
}