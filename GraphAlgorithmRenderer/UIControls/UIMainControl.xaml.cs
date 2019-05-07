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
            Add(textBoxNode.Text, _nodeFamilies, nodes, () => new NodeFamilyWindow(), "Node");
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

        private void Add(string text, IDictionary<ListBoxItem, Window> families,
            ItemsControl list, CreateWindow createWindow, string type)
        {
            var name = text;
            if (families.Any(kv => kv.Key.Content.Equals(name)))
            {
                MessageBox.Show($"{type} family already exists",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var item = new ListBoxItem {Content = name};
            item.MouseDoubleClick += (sender, args) => families[item]?.Show();
            families[item] = createWindow();

            list.Items.Add(item);
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            Add(textBoxEdge.Text, _edgeFamilies, edges,
                () => new EdgeFamilyWindow(_nodeFamilies.ToDictionary(kv => (string) kv.Key.Content,
                    kv => (NodeFamilyWindow) kv.Value)), "Edge");
        }

        private void RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            Remove(_edgeFamilies, edges);
        }

        public GraphConfig Config
        {
            get
            {
                return new GraphConfig
                {
                    Edges = _edgeFamilies.Values.Select(w => ((EdgeFamilyWindow) w).EdgeFamily).ToList(),
                    Nodes = _nodeFamilies.Values.Select(w => ((NodeFamilyWindow) w).NodeFamily).ToList()
                };
            }
        }

        public void FromConfig(GraphConfig config)
        {
            _edgeFamilies.Clear();
            _nodeFamilies.Clear();
            nodes.Items.Clear();
            edges.Items.Clear();
            if (config == null)
            {
                return;
            }
            foreach (var node in config.Nodes)
            {
                var window = new NodeFamilyWindow();
                window.FromNodeFamily(node);
                Add(node.Name, _nodeFamilies, nodes, () => window, "Node");
            }

            foreach (var edge in config.Edges)
            {
                var window = new EdgeFamilyWindow(_nodeFamilies.ToDictionary(kv => (string) kv.Key.Content,
                    kv => (NodeFamilyWindow) kv.Value));
                window.FromEdgeFamily(edge, config.Nodes.Select(x => x.Name).ToList());
                Add(edge.Name, _edgeFamilies, edges, () => window, "Edge");
            }
        }
    }
}