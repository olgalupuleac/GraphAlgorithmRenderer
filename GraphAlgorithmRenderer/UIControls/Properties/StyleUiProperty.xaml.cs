using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GraphAlgorithmRenderer.Config;
using Style = Microsoft.Msagl.Drawing.Style;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    /// <summary>
    /// Interaction logic for StyleUiProperty.xaml
    /// </summary>
    public partial class StyleUiProperty : UserControl, INodeUiProperty, IEdgeUiProperty
    {
        private static readonly IReadOnlyDictionary<string, Microsoft.Msagl.Drawing.Style> StylesToMsaglTypes =
            new Dictionary<string, Style>
            {
                {"Bold", Microsoft.Msagl.Drawing.Style.Bold},
                {"Dashed", Microsoft.Msagl.Drawing.Style.Dashed},
                {"Diagonals", Microsoft.Msagl.Drawing.Style.Diagonals},
                {"Dotted", Microsoft.Msagl.Drawing.Style.Dotted},
                {"Filled", Microsoft.Msagl.Drawing.Style.Filled},
                {"Invis", Microsoft.Msagl.Drawing.Style.Invis},
                {"Rounded", Microsoft.Msagl.Drawing.Style.Rounded},
                {"Solid", Microsoft.Msagl.Drawing.Style.Solid}
            };

        private readonly List<CheckBox> _styles;

        public StyleUiProperty()
        {
            InitializeComponent();
            _styles = Panel.Children.OfType<CheckBox>().ToList();
            Disable();
            CheckBox.Checked += (sender, args) => Enable();
            CheckBox.Unchecked += (sender, args) => Disable();
        }

        private delegate Style GetStyle();

        private void FromProperty(GetStyle getStyle)
        {
            var styleName = StylesToMsaglTypes.FirstOrDefault(x => x.Value == getStyle()).Key;
            if (styleName == null)
            {
                return;
            }

            CheckBox.IsChecked = true;
            //Enable(); ??
            _styles.Where(r => r.ContentStringFormat == styleName).ToList().ForEach(r => r.IsChecked = true);
        }

        private List<Style> GetStyles()
        {
            return _styles.Where(x => x.IsChecked == true).Select(x => StylesToMsaglTypes[(string)x.Content])
                .ToList();
        }

        public void FromINodeProperty(INodeProperty property)
        {
            if (!(property is StyleNodeProperty))
            {
                return;
            }

            FromProperty(() => ((StyleNodeProperty) property).Style);
        }

        public List<INodeProperty> NodeProperty
        {
            get
            {
                return CheckBox.IsChecked != true ? new List<INodeProperty>() : GetStyles().Select(x => (INodeProperty)new StyleNodeProperty(x)).ToList();
            }
        }

        public void FromIEdgeProperty(IEdgeProperty property)
        {
            if (!(property is StyleEdgeProperty))
            {
                return;
            }

            FromProperty(() => ((StyleEdgeProperty) property).Style);
        }

        public List<IEdgeProperty> EdgeProperty
        {
            get
            {
                return CheckBox.IsChecked != true ? new List<IEdgeProperty>() : GetStyles().Select(x => (IEdgeProperty) new StyleEdgeProperty(x)).ToList();
            }
        }

        public void Reset()
        {
            CheckBox.IsChecked = false; //Disable??
        }

        public void Disable()
        {
            _styles.ForEach(r =>
            {
                r.IsChecked = false;
                r.IsEnabled = false;
            });
        }

        public void Enable()
        {
            _styles.ForEach(r => { r.IsEnabled = true; });
        }
    }
}