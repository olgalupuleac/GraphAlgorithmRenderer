using System.Collections.Generic;
using System.Windows.Controls;
using GraphAlgorithmRendererLib.Config;
using static System.Single;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    /// <summary>
    /// Interaction logic for LabelUiProperty.xaml
    /// </summary>
    public partial class LabelUiProperty : UserControl, IEdgeUiProperty, INodeUiProperty
    {
        public LabelUiProperty()
        {
            InitializeComponent();
            Disable();
            CheckBox.Checked += (sender, args) => Enable();
            CheckBox.Unchecked += (sender, args) => Disable();
        }

        public void Reset()
        {
            CheckBox.IsChecked = false; //??
        }

        public void Disable()
        {
            LabelTextBox.Clear();
            LabelTextBox.IsEnabled = false;
            FontSizeTextBox.Clear();
            FontSizeTextBox.IsEnabled = false;
        }

        public void Enable()
        {
            LabelTextBox.IsEnabled = true;
            FontSizeTextBox.IsEnabled = true;
        }

        private void FromAbstractLabelProperty(AbstractLabelProperty abstractLabelProperty)
        {
            CheckBox.IsChecked = true; //Enable
            LabelTextBox.Text = abstractLabelProperty.LabelTextExpression;
            if (abstractLabelProperty.FontSize.HasValue)
            {
                FontSizeTextBox.Text = $"{abstractLabelProperty.FontSize.Value:0.00}";
            }
        }

        public void FromIEdgeProperty(IEdgeProperty property)
        {
            if (!(property is LabelEdgeProperty))
            {
                return;
            }

            FromAbstractLabelProperty((LabelEdgeProperty) property);
        }

        private void AddFontSizeToAbstractLabelProperty(AbstractLabelProperty abstractLabelProperty)
        {
            if (TryParse(FontSizeTextBox.Text, out var f))
            {
                abstractLabelProperty.FontSize = f;
            }
        }

        public List<IEdgeProperty> EdgeProperty
        {
            get
            {
                if (CheckBox.IsChecked != true)
                {
                    return new List<IEdgeProperty>();
                }

                LabelEdgeProperty property = new LabelEdgeProperty(LabelTextBox.Text);
                AddFontSizeToAbstractLabelProperty(property);
                return new List<IEdgeProperty> {property};
            }
        }

        public void FromINodeProperty(INodeProperty property)
        {
            if (!(property is LabelNodeProperty))
            {
                return;
            }

            FromAbstractLabelProperty((LabelNodeProperty) property);
        }

        public List<INodeProperty> NodeProperty
        {
            get
            {
                if (CheckBox.IsChecked != true)
                {
                    return new List<INodeProperty>();
                }

                LabelNodeProperty property = new LabelNodeProperty(LabelTextBox.Text);
                AddFontSizeToAbstractLabelProperty(property);
                return new List<INodeProperty> {property};
            }
        }
    }
}