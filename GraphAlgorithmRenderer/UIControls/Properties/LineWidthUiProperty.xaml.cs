using System;
using System.Collections.Generic;
using System.Windows.Controls;
using GraphAlgorithmRenderer.Config;
using static System.Single;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    /// <summary>
    /// Interaction logic for LineWidthUiProperty.xaml
    /// </summary>
    public partial class LineWidthUiProperty : UserControl, INodeUiProperty, IEdgeUiProperty
    {
        public LineWidthUiProperty()
        {
            InitializeComponent();
            Disable();
            CheckBox.Checked += (sender, args) => Enable();
            CheckBox.Unchecked += (sender, args) => Disable();
        }

        public void Reset()
        {
            CheckBox.IsChecked = false;
        }

        public void Disable()
        {
            TextBox.Clear();
            TextBox.IsEnabled = false;
        }

        public void Enable()
        {
            TextBox.IsEnabled = true;
        }

        private float GetLineWidth()
        {
            try
            {
                return Parse(TextBox.Text);
            }
            catch (Exception exception)
            {
                throw new ConfigGenerationException("Line width should be a number", exception);
            }
        }

        public void FromINodeProperty(INodeProperty property)
        {
            if (!(property is LineWidthNodeProperty lineWidthNodeProperty)) return;
            CheckBox.IsChecked = true;
            TextBox.Text = $"{lineWidthNodeProperty.LineWidth:0.00}";
        }

        public List<INodeProperty> NodeProperty
        {
            get
            {
                if (CheckBox.IsChecked != true)
                {
                    return new List<INodeProperty>();
                }

                return new List<INodeProperty>
                {
                    new LineWidthNodeProperty(GetLineWidth())
                };
            }
        }

        public void FromIEdgeProperty(IEdgeProperty property)
        {
            if (!(property is LineWidthEdgeProperty lineWidthEdgeProperty)) return;
            CheckBox.IsChecked = true;
            TextBox.Text = $"{lineWidthEdgeProperty.LineWidth:0.00}";
        }

        public List<IEdgeProperty> EdgeProperty
        {
            get
            {
                if (CheckBox.IsChecked != true)
                {
                    return new List<IEdgeProperty>();
                }

                return new List<IEdgeProperty>
                {
                    new LineWidthEdgeProperty(GetLineWidth())
                };
            }
        }
    }
}