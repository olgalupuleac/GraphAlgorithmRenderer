using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using GraphAlgorithmRenderer.Config;
using Microsoft.Msagl.Drawing;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    /// <summary>
    /// Interaction logic for ShapeProperty.xaml
    /// </summary>
    public partial class ShapeUiProperty : UserControl, INodeUiProperty
    {
        private static readonly IReadOnlyDictionary<string, Shape> ShapesToMsaglTypes =
            new Dictionary<string, Shape>
            {
                {"Box", Shape.Box},
                {"Circle", Shape.Circle},
                {"Diamond", Shape.Diamond},
                {"Double Circle", Shape.DoubleCircle},
                {"Ellipse", Shape.Ellipse},
                {"House", Shape.House},
                {"Octagon", Shape.Octagon},
                {"Parallelogram", Shape.Parallelogram},
                {"Point", Shape.Point},
                {"Polygon", Shape.Polygon},
                {"Trapezium", Shape.Trapezium},
                {"Triangle", Shape.Triangle}
            };

        private readonly List<RadioButton> _shapeRadioButtons;

        public ShapeUiProperty()
        {
            InitializeComponent();
            _shapeRadioButtons = Panel.Children.OfType<RadioButton>().ToList();
            _shapeRadioButtons.ForEach(r => r.GroupName = $"Shape{GetHashCode()}");
            Disable();
            CheckBox.Checked += (sender, args) => Enable();
            CheckBox.Unchecked += (sender, args) => Disable();
        }

        public void Disable()
        {
            _shapeRadioButtons.ForEach(r =>
            {
                r.IsChecked = false;
                r.IsEnabled = false;
            });
        }

        public void Enable()
        {
            _shapeRadioButtons.ForEach(r => r.IsEnabled = true);
        }

        private Shape? GetSelectedShape()
        {
            var selected = _shapeRadioButtons.FindAll(r => r.IsChecked == true);
            if (selected.Count == 0)
            {
                return null;
            }

            return ShapesToMsaglTypes[(string)selected[0].Content];
        }


        public void FromINodeProperty(INodeProperty property)
        {
            if (!(property is ShapeNodeProperty))
            {
                return;
            }

            var shapeProperty = (ShapeNodeProperty) property;
            var shapeName = ShapesToMsaglTypes.FirstOrDefault(x => x.Value == shapeProperty.Shape).Key;
            if (shapeName == null)
            {
                return;
            }

            CheckBox.IsChecked = true;
            //Enable(); ??
            _shapeRadioButtons.Where(r => r.ContentStringFormat == shapeName).ToList().ForEach(r => r.IsChecked = true);
        }

        public List<INodeProperty> NodeProperty
        {
            get
            {
                if (CheckBox.IsChecked != true)
                {
                    return new List<INodeProperty>();
                }

                var shapeOr = GetSelectedShape();
                if (shapeOr == null)
                {
                    throw new ConfigGenerationException("Shape property is enabled but shape is not defined");
                }

                return new List<INodeProperty> {new ShapeNodeProperty(shapeOr.Value)};
            }
        }

        public void Reset()
        {
            CheckBox.IsChecked = false;
            //Disable(); ??
        }
    }
}