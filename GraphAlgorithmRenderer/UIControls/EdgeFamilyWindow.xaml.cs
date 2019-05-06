using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for EdgeFamilyWindow.xaml
    /// </summary>
    public partial class EdgeFamilyWindow : Window
    {
        public ObservableCollection<IdentifierPartTemplate> Ranges { get; set; }
        private readonly Dictionary<ListBoxItem, NodeFamilyWindow> _availableNodes;

        public EdgeFamilyWindow(Dictionary<ListBoxItem, NodeFamilyWindow> availableNodes)
        {
            _availableNodes = availableNodes;
            InitializeComponent();
            Ranges = new ObservableCollection<IdentifierPartTemplate>();
            identifiers.ItemsSource = Ranges;
            foreach (var node in _availableNodes.Keys)
            {
                TargetList.Items.Add(new ListBoxItem {Content = node.Content});
                SourceList.Items.Add(new ListBoxItem {Content = node.Content});
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
    }
}