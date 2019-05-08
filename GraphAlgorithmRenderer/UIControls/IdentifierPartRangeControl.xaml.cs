using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for IdentifierPartRangeControl.xaml
    /// </summary>
    public partial class IdentifierPartRangeControl : UserControl
    {
        public ObservableCollection<IdentifierPartTemplate> Ranges { get; set; }
        public IdentifierPartRangeControl()
        {
            InitializeComponent();
            Ranges = new ObservableCollection<IdentifierPartTemplate>();
            identifiers.ItemsSource = Ranges;
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

        public void FromRanges(List<IdentifierPartTemplate> ranges)
        {
            Ranges.Clear();
            ranges.ForEach(x => Ranges.Add(x));
        }
    }
}
