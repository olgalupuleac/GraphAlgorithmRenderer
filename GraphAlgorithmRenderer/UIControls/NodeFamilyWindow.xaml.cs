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
using GraphAlgorithmRenderer.GraphElementIdentifier;
using static System.String;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for NodeFamilyWindow.xaml
    /// </summary>
    public partial class NodeFamilyWindow : Window
    {
        private Dictionary<ListBoxItem, NodeConditionalPropertyWindow> _properties;
        public ObservableCollection<IdentifierPartTemplate> Ranges { get; set; }

        public NodeFamilyWindow()
        {
            InitializeComponent();
            Ranges = new ObservableCollection<IdentifierPartTemplate>();
            identifiers.ItemsSource = Ranges;
            _properties = new Dictionary<ListBoxItem, NodeConditionalPropertyWindow>();
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

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            foreach (var range in Ranges)
            {
                if (IsNullOrEmpty(range.Name) ||
                    !IsNullOrEmpty(range.BeginTemplate) && !IsNullOrEmpty(range.EndTemplate)) continue;
                MessageBox.Show($"Identifier range {range.Name} is not finished", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Hide();
        }

        private ListBoxItem AddNewProperty()
        {
            var priority = _properties.Count + 1;
            var item = new ListBoxItem { Content = $"Property#{priority}" };

            _properties[item] = new NodeConditionalPropertyWindow(priority);
            item.MouseDoubleClick += (o, args) => _properties[item].Show();
            properties.Items.Add(item);
            return item;
        }

        private void AddProperty_Click(object sender, RoutedEventArgs e)
        {
            var item = AddNewProperty();
            _properties[item].Show();
        }

        private void RemoveProperty_Click(object sender, RoutedEventArgs e)
        {
            if (!(properties.SelectedItem is ListBoxItem item))
            {
                return;
            }

            _properties[item].Hide();
            _properties.Remove(item);
            properties.Items.Remove(item);
            for (var i = 0; i < properties.Items.Count; i++)
            {
                ((ListBoxItem) properties.Items[i]).Content = $"Property#{i + 1}";
                _properties[((ListBoxItem) properties.Items[i])].Priority = i;
            }
        }

        public NodeFamily NodeFamily
        {
            get
            {
                var conditionalProperties = _properties.Values.OrderBy(w => w.Priority)
                    .Select(w => w.ConditionalProperty).ToList();
                conditionalProperties.Reverse();
                //TODO check for null
                return new NodeFamily(Ranges.ToList())
                {
                    ValidationTemplate = validationTemplateBox.Text,
                    ConditionalProperties = conditionalProperties
                };
            }
        }

        public void FromNodeFamily(NodeFamily nodeFamily)
        {
            Ranges.Clear();
            nodeFamily.Ranges.ForEach(r => Ranges.Add(r));
            validationTemplateBox.Text = nodeFamily.ValidationTemplate;
            var conditionalProperties = ((IEnumerable<ConditionalProperty<INodeProperty>>) nodeFamily.ConditionalProperties)
                .Reverse().ToList();
            properties.Items.Clear();
            _properties.Clear();
            for (int i = 0; i < conditionalProperties.Count; i++)
            {
                var item = AddNewProperty();
                _properties[item].FromConditionalProperty(i + 1, conditionalProperties[i]);
            }


        }
    }
}