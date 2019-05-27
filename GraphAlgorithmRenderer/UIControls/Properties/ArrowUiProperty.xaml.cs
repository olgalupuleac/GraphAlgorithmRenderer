using System;
using System.Collections.Generic;
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

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    /// <summary>
    /// Interaction logic for ArrowUiProperty.xaml
    /// </summary>
    public partial class ArrowUiProperty : UserControl, IEdgeUiProperty
    {
        public ArrowUiProperty()
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
            Target.IsChecked = false;
            Target.IsEnabled = false;
            Source.IsChecked = false;
            Source.IsEnabled = false;
        }

        public void Enable()
        {
            Target.IsEnabled = true;
            Target.IsChecked = true;
            Source.IsEnabled = true;
        }

        public void FromIEdgeProperty(IEdgeProperty property)
        {
            if (!(property is ArrowEdgeProperty arrowProperty)) return;
            CheckBox.IsChecked = true;
            Target.IsChecked = arrowProperty.ArrowAtTarget;
            Source.IsChecked = arrowProperty.ArrowAtSource;
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
                    new ArrowEdgeProperty(Target.IsChecked == true, Source.IsChecked == true)
                };
            }
        }
    }
}
