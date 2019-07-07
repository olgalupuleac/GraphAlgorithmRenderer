using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GraphAlgorithmRendererLib.Config;

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
            OnTop.Content = "Draw graph window\non top";

            Nodes.WindowGenerator = () => new NodeFamilyWindow{ NameIsSet = false };
            Nodes.Description = w =>
            {
                var textBox = ((NodeFamilyWindow)w).FamilyName;
                if (!((NodeFamilyWindow)w).NameIsSet)
                {
                    textBox.Text = $"node#{Nodes.properties.Items.Count}";
                }     
                return textBox.Text;
            };
            Nodes.UpdateDescription = (w, i) =>
            {
                ((NodeFamilyWindow) w).ok.Click +=
                    (o, sender) => i.Content = ((NodeFamilyWindow) w).FamilyName.Text;
            };
            Nodes.AddProperty.Click += (sender, args) =>
            {
                foreach (var edgeWindow in Edges.Windows)
                {
                    ((EdgeFamilyWindow) edgeWindow).SetRadioButtons();
                }
            };
            Nodes.RemoveProperty.Click += (sender, args) =>
            {
                foreach (var edgeWindow in Edges.Windows)
                {
                    ((EdgeFamilyWindow)edgeWindow).SetRadioButtons();
                }
            };
            Edges.WindowGenerator = () =>
                new EdgeFamilyWindow(Nodes.ItemsToWindows){ NameIsSet = false };
            Edges.Description = w =>
            {
                var textBox = ((EdgeFamilyWindow) w).FamilyName;
                if (!((EdgeFamilyWindow)w).NameIsSet)
                {
                    textBox.Text = $"edge#{Edges.properties.Items.Count}";
                }
                return textBox.Text;  
            };
            Edges.UpdateDescription = (w, i) =>
            {
                ((EdgeFamilyWindow) w).ok.Click +=
                    (o, sender) => i.Content = ((EdgeFamilyWindow) w).FamilyName.Text;
            };
        }

        public GraphConfig Config
        {
            get
            {
                return new GraphConfig
                {
                    Edges = Edges.Windows.Select(w => ((EdgeFamilyWindow) w).EdgeFamily).ToList(),
                    Nodes = Nodes.Windows.Select(w => ((NodeFamilyWindow) w).NodeFamily).ToList()
                };
            }
        }

        public void FromConfig(GraphConfig config)
        {
            var nodeWindows = config.Nodes.Select(n =>
            {
                var w = new NodeFamilyWindow();
                w.FromNodeFamily(n);
                return (Window) w;
            }).ToList();
            Nodes.SetNewWindows(nodeWindows);
            var edgeWindows = config.Edges.Select(e =>
            {
                var w = new EdgeFamilyWindow(Nodes.ItemsToWindows);
                w.FromEdgeFamily(e);
                return (Window) w;
            }).ToList();
            Edges.SetNewWindows(edgeWindows);
        }
    }
}