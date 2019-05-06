using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GraphAlgorithmRenderer.Config;
using static System.Single;
using Condition = GraphAlgorithmRenderer.Config.Condition;
using Style = Microsoft.Msagl.Drawing.Style;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for EdgeConditionalPropertyWindow.xaml
    /// </summary>
    public partial class EdgeConditionalPropertyWindow : Window
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

        public EdgeConditionalPropertyWindow()
        {
            InitializeComponent();
            labelTextBox.IsEnabled = false;
            labelFontSizeBox.IsEnabled = false;
            lineWidthBox.IsEnabled = false;
            colorPicker.IsEnabled = false;
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

        public ConditionalProperty<IEdgeProperty> ConditionalProperty
        {
            get
            {
                var checkedRadioButton = ModePanel.Children.OfType<RadioButton>()
                    .FirstOrDefault(r => r.IsChecked != null && (bool) r.IsChecked);
                var mode = ConditionMode.CurrentStackFrame;
                if (checkedRadioButton == AllSf)
                {
                    mode = ConditionMode.AllStackFrames;
                }

                var condition = new Condition(conditionBox.Text,
                    regexBox.Text, mode);
                var properties = new List<IEdgeProperty>();
                if (labelCheckBox.IsChecked != null && (bool) labelCheckBox.IsChecked)
                {
                    var labelProperty = new LabelEdgeProperty(labelTextBox.Text);
                    if (TryParse(labelFontSizeBox.Text, out var f))
                    {
                        labelProperty.FontSize = f;
                    }

                    properties.Add(labelProperty);
                }

                if (colorCheckBox.IsChecked != null && (bool) colorCheckBox.IsChecked)
                {
                    var color = colorPicker.SelectedColor;
                    properties.Add(new LineColorEdgeProperty(
                        new Microsoft.Msagl.Drawing.Color(a: color.Value.A,
                            r: color.Value.R, g: color.Value.G, b: color.Value.B)));
                }

                if (lineWidthCheckBox.IsChecked != null && (bool) lineWidthCheckBox.IsChecked)
                {
                    properties.Add(new LineWidthEdgeProperty(Parse(labelFontSizeBox.Text)));
                }

                if (styleCheckBox.IsChecked != null && (bool) styleCheckBox.IsChecked)
                {
                    ModePanel.Children.OfType<CheckBox>()
                        .Where(cb => cb.IsChecked != null && (bool) cb.IsChecked)
                        .ToList().ForEach(cb => properties.Add(new StyleEdgeProperty(_styles[cb.ContentStringFormat])) );
                }

                return new ConditionalProperty<IEdgeProperty>(condition, properties);
            }
        }
    }
}