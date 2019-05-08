using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            InitializeComponent();
            Nodes.Label.Content = "Node families";
            Edges.Label.Content = "Edge families";
            Nodes.WindowGenerator = () => new NodeFamilyWindow();
            Nodes.Description = w => ((NodeFamilyWindow) w).familyName.Text;
            Nodes.UpdateDescription = (w, i) =>
            {
                ((NodeFamilyWindow) w).ok.Click +=
                    (o, sender) => i.Content = ((NodeFamilyWindow) w).familyName.Text;
            };
            Edges.WindowGenerator = () =>
                new EdgeFamilyWindow(Nodes.WindowsWithDescriptions.ToDictionary(kv => kv.Key,
                    kv => (NodeFamilyWindow) kv.Value));
            Edges.Description = w => ((EdgeFamilyWindow)w).familyName.Text;
            Edges.UpdateDescription = (w, i) =>
            {
                ((EdgeFamilyWindow)w).ok.Click +=
                    (o, sender) => i.Content = ((EdgeFamilyWindow)w).familyName.Text;
            };
        }

        public GraphConfig Config
        {
            get
            {
                return new GraphConfig
                {
                    Edges = Edges.Windows.Select(w => ((EdgeFamilyWindow)w).EdgeFamily).ToList(),
                    Nodes = Nodes.Windows.Select(w => ((NodeFamilyWindow)w).NodeFamily).ToList()
                };
            }
        }

        public void FromConfig(GraphConfig config)
        {
            var nodeWindows = config.Nodes.Select(n =>
            {
                var w = new NodeFamilyWindow();
                w.FromNodeFamily(n);
                return (Window)w;
            }).ToList();
            Nodes.SetNewWindows(nodeWindows);
            var edgeWindows = config.Edges.Select(e =>
            {
                var w = new EdgeFamilyWindow(Nodes.WindowsWithDescriptions.ToDictionary(kv => kv.Key,
                    kv => (NodeFamilyWindow)kv.Value));
                w.FromEdgeFamily(e);
                return (Window)w;
            }).ToList();
            Edges.SetNewWindows(edgeWindows);
        }
    }
}