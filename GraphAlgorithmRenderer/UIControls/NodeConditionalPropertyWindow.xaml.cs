using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using GraphAlgorithmRenderer.UIControls.Properties;
using Condition = System.Windows.Condition;
using Shape = Microsoft.Msagl.Drawing.Shape;
using Style = Microsoft.Msagl.Drawing.Style;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for NodeConditionalPropertyWindow.xaml
    /// </summary>
    public partial class NodeConditionalPropertyWindow : Window
    {
        private List<INodeUiProperty> _properties;
   
        public int Priority { get; set; }

        public NodeConditionalPropertyWindow(int priority)
        {
            Priority = priority;           
            InitializeComponent();
            _properties = new List<INodeUiProperty>
            {
                LabelUiProperty,
                LineColorUiProperty,
                LineWidthUiProperty,
                StyleUiProperty,
                ShapeUiProperty,
                FillColorUiProperty
            };
        }

  

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true; // cancels the window close    
            Hide(); // Programmatically hides the window
        }

 

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public ConditionalProperty<INodeProperty> ConditionalProperty
        {
            get
            {
                var properties = _properties.SelectMany(x => x.NodeProperty).ToList();
                return new ConditionalProperty<INodeProperty>(ConditionControl.Condition, properties);
            }
        }


        public void FromConditionalProperty(int priority, ConditionalProperty<INodeProperty> conditionalProperty)
        {
            Priority = priority;
            ConditionControl.FromCondition(conditionalProperty.Condition);
            _properties.ForEach(x => x.Reset());
            conditionalProperty.Properties.ForEach(x =>
                _properties.ForEach(p =>
                    p.FromINodeProperty(x)));
        }
    }
}
