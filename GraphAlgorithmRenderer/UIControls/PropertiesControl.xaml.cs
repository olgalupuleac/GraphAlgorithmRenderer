using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for PropertiesControl.xaml
    /// </summary>
    public partial class PropertiesControl : UserControl
    {
        public delegate Window CreateNewWindow();

        public CreateNewWindow WindowGenerator { get; set; }

        public UpdateDescriptionDelegate UpdateDescription { get; set; }


        public GetDescription Description { get; set; }

        public delegate string GetDescription(Window window);

        public delegate void UpdateDescriptionDelegate(Window window, ListBoxItem item);

        private readonly Dictionary<ListBoxItem, Window> _itemsToWindows;


        public PropertiesControl()
        {
            _itemsToWindows = new Dictionary<ListBoxItem, Window>();
            InitializeComponent();
        }

        private ListBoxItem AddNewProperty(CreateNewWindow createWindow)
        {
            var window = createWindow();
            var item = new ListBoxItem{Content = Description(window)};
            UpdateDescription(window, item);
            _itemsToWindows[item] = window;
            item.MouseDoubleClick += (o, args) => _itemsToWindows[item].Show();
            properties.Items.Add(item);
            return item;
        }


        private void RemoveProperty_Click(object sender, RoutedEventArgs e)
        {
            if (!(properties.SelectedItem is ListBoxItem item))
            {
                return;
            }
            _itemsToWindows[item].Hide();
            _itemsToWindows.Remove(item);
            properties.Items.Remove(item);
        }

        public List<Window> Windows {
            get
            {
                return properties.Items.OfType<ListBoxItem>().Select(x => _itemsToWindows[x]).ToList();
            }
        }
        

        private void AddProperty_Click(object sender, RoutedEventArgs e)
        {
            var item = AddNewProperty(WindowGenerator);
            _itemsToWindows[item].Show();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (!(properties.SelectedItem is ListBoxItem item))
            {
                return;
            }

            int index = properties.Items.IndexOf(item);
            if (index == 0)
            {
                return;
            }

            properties.Items.Remove(item);
            properties.Items.Insert(index - 1, item);
        }

        public void SetNewWindows(List<Window> windows)
        {
            properties.Items.Clear();
            _itemsToWindows.Clear();
            windows.ForEach(w => AddNewProperty(() => w));
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (!(properties.SelectedItem is ListBoxItem item))
            {
                return;
            }

            int index = properties.Items.IndexOf(item);
            if (index == properties.Items.Count - 1)
            {
                return;
            }

            properties.Items.Remove(item);
            properties.Items.Insert(index + 1, item);
        }
    }
}
