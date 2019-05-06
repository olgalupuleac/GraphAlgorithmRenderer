using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Style = Microsoft.Msagl.Drawing.Style;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for NodeConditionalPropertyWindow.xaml
    /// </summary>
    public partial class NodeConditionalPropertyWindow : Window
    {
        private readonly IReadOnlyDictionary<string, Microsoft.Msagl.Drawing.Style> _styles = new Dictionary<string, Style>
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

        private List<RadioButton> _shapes;

        public int Priority { get; set; }

        public NodeConditionalPropertyWindow(int priority)
        {
            Priority = priority;
            _shapes = new List<RadioButton>();
            InitializeComponent();
            labelTextBox.IsEnabled = false;
            labelFontSizeBox.IsEnabled = false;
            lineWidthBox.IsEnabled = false;
            colorPicker.IsEnabled = false;
            fillColorPicker.IsEnabled = false;
            fillColorCheckBox.Checked += (sender, args) => fillColorPicker.IsEnabled = true;
            fillColorCheckBox.Unchecked += (sender, args) =>
            {
                fillColorPicker.SelectedColor = new Color();
                fillColorPicker.IsEnabled = false;
            };
            colorCheckBox.Checked += (sender, args) => colorPicker.IsEnabled = true;
            colorCheckBox.Unchecked += (sender, args) =>
            {
                colorPicker.SelectedColor = new Color();
                colorPicker.IsEnabled = false;
            };
            foreach (var elem in Styles.Children)
            {
                if (elem is CheckBox box)
                {
                    box.IsEnabled = false;
                }
            }

            _shapes.AddRange(Shapes1.Children.OfType<RadioButton>());
            _shapes.AddRange(Shapes2.Children.OfType<RadioButton>());

            _shapes.ForEach(r => r.IsEnabled = false);

            shapeCheckBox.Checked += (sender, args) => _shapes.ForEach(r => r.IsEnabled = true);
            shapeCheckBox.Unchecked += (sender, args) => _shapes.ForEach(r =>
            {
                r.IsChecked = false;
                r.IsEnabled = false;
            });


            styleCheckBox.Checked += StyleCheckBoxOnChecked;
            styleCheckBox.Unchecked += StyleCheckBoxOnUnchecked;
            lineWidthCheckBox.Checked += (sender, args) => lineWidthBox.IsEnabled = true;
            lineWidthCheckBox.Unchecked += (sender, args) =>
            {
                lineWidthBox.Clear();
                lineWidthBox.IsEnabled = false;
            };
            labelCheckBox.Checked += LabelCheckBoxOnChecked;
            labelCheckBox.Unchecked += LabelCheckBoxOnUnchecked;
        }

        private void StyleCheckBoxOnUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (var elem in Styles.Children)
            {
                if (!(elem is CheckBox box)) continue;
                box.IsChecked = false;
                box.IsEnabled = false;
            }
        }

        private void StyleCheckBoxOnChecked(object sender, RoutedEventArgs e)
        {
            foreach (var elem in Styles.Children)
            {
                if (elem is CheckBox box)
                {
                    box.IsEnabled = true;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true; // cancels the window close    
            Hide(); // Programmatically hides the window
        }

        private void LabelCheckBoxOnUnchecked(object sender, RoutedEventArgs e)
        {
            labelTextBox.Clear();
            labelFontSizeBox.Clear();
            labelTextBox.IsEnabled = false;
            labelFontSizeBox.IsEnabled = false;
        }

        private void LabelCheckBoxOnChecked(object sender, RoutedEventArgs e)
        {
            labelTextBox.IsEnabled = true;
            labelFontSizeBox.IsEnabled = true;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
