using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using GraphAlgorithmRenderer.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    /// <summary>
    /// Interaction logic for FillColorUiProperty.xaml
    /// </summary>
    public partial class FillColorUiProperty : UserControl, INodeUiProperty
    {
        public FillColorUiProperty()
        {
            InitializeComponent();
            Disable();
            CheckBox.Checked += (sender, args) => Enable();
            CheckBox.Unchecked += (sender, args) => Disable();
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

        public void FromINodeProperty(INodeProperty property)
        {
            if (!(property is FillColorNodeProperty fillColorNodeProperty)) return;
            SetColor(fillColorNodeProperty.Color);
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
                    new FillColorNodeProperty(ToMsaglColor())
                };
            }
        }
    }
}
