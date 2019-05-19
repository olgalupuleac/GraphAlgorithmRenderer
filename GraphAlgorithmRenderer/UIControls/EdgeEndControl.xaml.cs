using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for EdgeEndControl.xaml
    /// </summary>
    public partial class EdgeEndControl : Window
    {
        public List<EdgeEndIdPart> EdgeEndIdParts;
        private string _nodeName;
        public EdgeEndControl(List<string> nodeIds, string nodeName)
        {
            _nodeName = nodeName;
            InitializeComponent();
            EdgeEndIdParts = new List<EdgeEndIdPart>();
            nodeIds.ForEach(id => EdgeEndIdParts.Add(new EdgeEndIdPart{IdPart = id}));
            dataGrid.ItemsSource = EdgeEndIdParts;
            this.PreviewKeyDown += CloseOnEscape;
        }

        private void CloseOnEscape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
        }

        public EdgeEndControl(EdgeFamily.EdgeEnd edgeEnd)
        {
            _nodeName = edgeEnd.NodeFamilyName;
            InitializeComponent();
            EdgeEndIdParts = new List<EdgeEndIdPart>();
            foreach (var kv in edgeEnd.NamesWithTemplates)
            {
                EdgeEndIdParts.Add(new EdgeEndIdPart{IdPart = kv.Key, Template = kv.Value});
            }

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

        public EdgeFamily.EdgeEnd EdgeEnd
        {
            get
            {
                return new EdgeFamily.EdgeEnd
                {
                    //TODO check for null and throw exception
                    NamesWithTemplates
                        = EdgeEndIdParts.ToDictionary(x => x.IdPart, x => x.Template),
                    NodeFamilyName = _nodeName
                };
            }
        }
    }

    public class EdgeEndIdPart
    {
        public string IdPart { get; set; }
        public string Template { get; set; }
    }
}
