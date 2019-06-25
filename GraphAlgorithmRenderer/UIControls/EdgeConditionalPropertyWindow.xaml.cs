using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GraphAlgorithmRenderer.UIControls.Properties;
using GraphAlgorithmRendererLib.Config;
using static System.Single;
using Condition = GraphAlgorithmRendererLib.Config.Condition;
using Style = Microsoft.Msagl.Drawing.Style;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for EdgeConditionalPropertyWindow.xaml
    /// </summary>
    public partial class EdgeConditionalPropertyWindow : Window
    {
        private readonly List<IEdgeUiProperty> _properties;

        public EdgeConditionalPropertyWindow()
        {
            InitializeComponent();
            _properties = new List<IEdgeUiProperty>
            {
                LabelUiProperty,
                LineColorUiProperty,
                LineWidthUiProperty,
                StyleUiProperty,
                ArrowUiProperty
            };
            this.PreviewKeyDown += CloseOnEscape;
        }

        private void CloseOnEscape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Hide();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true; 
            Hide();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public ConditionalProperty<IEdgeProperty> ConditionalProperty
        {
            get
            {
                var properties = _properties.SelectMany(x => x.EdgeProperty).ToList();
                return new ConditionalProperty<IEdgeProperty>(ConditionControl.Condition, properties);
            }
        }


        public void FromConditionalProperty(ConditionalProperty<IEdgeProperty> conditionalProperty)
        {
            ConditionControl.FromCondition(conditionalProperty.Condition);
            _properties.ForEach(x => x.Reset());
            conditionalProperty.Properties.ForEach(x => 
                _properties.ForEach(p =>
                    p.FromIEdgeProperty(x)));
        }
    }
}