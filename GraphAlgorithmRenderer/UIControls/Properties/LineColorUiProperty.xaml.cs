using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using GraphAlgorithmRenderer.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    /// <summary>
    /// Interaction logic for LineColorUiProperty.xaml
    /// </summary>
    public partial class LineColorUiProperty : UserControl, INodeUiProperty, IEdgeUiProperty
    {
        public LineColorUiProperty()
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
            ColorPicker.SelectedColor = null;
            ColorPicker.IsEnabled = false;
        }

        public void Enable()
        {
            ColorPicker.IsEnabled = true;
        }

        private Microsoft.Msagl.Drawing.Color ToMsaglColor()
        {
            var color = ColorPicker.SelectedColor;
            if (!color.HasValue)
            {
                throw new ConfigGenerationException("Line color is not specified");
            }

            return new Microsoft.Msagl.Drawing.Color(a: color.Value.A,
                r: color.Value.R, g: color.Value.G, b: color.Value.B);
        }

        private void SetColor(Microsoft.Msagl.Drawing.Color color)
        {
            ColorPicker.SelectedColor = new Color
            {
                A = color.A,
                G = color.G,
                R = color.R,
                B = color.B
            };
        }

        public void FromINodeProperty(INodeProperty property)
        {
            if (!(property is LineColorNodeProperty lineColorNodeProperty)) return;
            CheckBox.IsChecked = true;
            SetColor(lineColorNodeProperty.Color);
        }

        public List<INodeProperty> NodeProperty
        {
            get
            {
                if (CheckBox.IsEnabled != true)
                {
                    return new List<INodeProperty>();
                }

                return new List<INodeProperty>
                {
                    new LineColorNodeProperty(ToMsaglColor())
                };
            }
        }

        public void FromIEdgeProperty(IEdgeProperty property)
        {
            if (!(property is LineColorEdgeProperty lineColorEdgeProperty)) return;
            SetColor(lineColorEdgeProperty.Color);
        }

        public List<IEdgeProperty> EdgeProperty
        {
            get
            {
                if (CheckBox.IsEnabled != true)
                {
                    return new List<IEdgeProperty>();
                }

                return new List<IEdgeProperty>
                {
                    new LineColorEdgeProperty(ToMsaglColor())
                };
            }
        }
    }
}