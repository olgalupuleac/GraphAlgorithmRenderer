using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphAlgorithmRenderer.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    /// <summary>
    /// Interaction logic for UIMainControl.xaml
    /// </summary>
    public partial class UIMainControl : UserControl
    {
        private readonly Dictionary<ListBoxItem, Window> _nodeFamilies;
        private readonly Dictionary<ListBoxItem, Window> _edgeFamilies;
        public UIMainControl()
        {
            _nodeFamilies = new Dictionary<ListBoxItem, Window>();
            _edgeFamilies = new Dictionary<ListBoxItem, Window>();
            InitializeComponent();
            this.DataContext = this;        
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            Add(textBoxNode, _nodeFamilies, nodes, () => new NodeFamilyWindow(), "Node");
        }

        private void RemoveNode_Click(object sender, RoutedEventArgs e)
        {
           Remove(_nodeFamilies, nodes);
        }

        private void Remove(IDictionary<ListBoxItem, Window> families,
            Selector list)
        {
            if (!(list.SelectedItem is ListBoxItem item))
            {
                return;
            }

            families.Remove(item);
            list.Items.Remove(item);
        }

        private delegate Window CreateWindow();

        private void Add(TextBox textBox, IDictionary<ListBoxItem, Window> families,
            ItemsControl list, CreateWindow createWindow, string type)
        {
            var name = textBox.Text;
            if (families.Any(kv => kv.Key.Content.Equals(name)))
            {
                MessageBox.Show($"{type} family already exists",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var item = new ListBoxItem { Content = name };
            item.Selected += (sender, args) => families[item]?.Show();
            families[item] = createWindow();

            list.Items.Add(item);
            families[item]?.Show();
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            Add(textBoxEdge, _edgeFamilies, edges, () => null, "Edge");
        }

        private void RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            Remove(_edgeFamilies, edges);
        }
    }
}
