using System;
using System.Collections.Generic;
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

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for EdgeEndControl.xaml
    /// </summary>
    public partial class EdgeEndControl : Window
    {
        public List<EdgeEndIdPart> EdgeEndIdParts;
        public EdgeEndControl(List<string> nodeIds)
        {
            InitializeComponent();
            EdgeEndIdParts = new List<EdgeEndIdPart>();
            nodeIds.ForEach(id => EdgeEndIdParts.Add(new EdgeEndIdPart{IdPart = id}));
            dataGrid.ItemsSource = EdgeEndIdParts;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            Hide();      // Programmatically hides the window
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }

    public class EdgeEndIdPart
    {
        public string IdPart { get; set; }
        public string Template { get; set; }
    }
}
