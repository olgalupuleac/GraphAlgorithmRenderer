using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GraphAlgorithmRenderer.Config;
using GraphAlgorithmRenderer.UIControls.Properties;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for NodeConditionalPropertyWindow.xaml
    /// </summary>
    public partial class NodeConditionalPropertyWindow : Window
    {
        private readonly List<INodeUiProperty> _properties;

        public NodeConditionalPropertyWindow()
        {
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
            this.PreviewKeyDown += CloseOnEscape;
        }

        private void CloseOnEscape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
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

    
        public void FromConditionalProperty(ConditionalProperty<INodeProperty> conditionalProperty)
        {
            ConditionControl.FromCondition(conditionalProperty.Condition);
            _properties.ForEach(x => x.Reset());
            conditionalProperty.Properties.ForEach(x =>
                _properties.ForEach(p =>
                    p.FromINodeProperty(x)));
        }
    }
}