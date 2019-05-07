﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using Condition = System.Windows.Condition;
using Shape = Microsoft.Msagl.Drawing.Shape;
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

        private readonly IReadOnlyDictionary<string, Microsoft.Msagl.Drawing.Shape> _shapesDict =
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
                {"Polygon", Shape.Polygon },
                {"Trapezium", Shape.Trapezium},
                {"Triangle", Shape.Triangle}
            };

        private List<RadioButton> _shapeRadioButtons;

        public int Priority { get; set; }

        public NodeConditionalPropertyWindow(int priority)
        {
            Priority = priority;
            _shapeRadioButtons = new List<RadioButton>();
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

            _shapeRadioButtons.AddRange(Shapes1.Children.OfType<RadioButton>());
            _shapeRadioButtons.AddRange(Shapes2.Children.OfType<RadioButton>());

            _shapeRadioButtons.ForEach(r => r.IsEnabled = false);

            shapeCheckBox.Checked += (sender, args) => _shapeRadioButtons.ForEach(r => r.IsEnabled = true);
            shapeCheckBox.Unchecked += (sender, args) => _shapeRadioButtons.ForEach(r =>
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

        public ConditionalProperty<INodeProperty> ConditionalProperty
        {
            get
            {
                var checkedRadioButton = ModePanel.Children.OfType<RadioButton>()
                    .FirstOrDefault(r => r.IsChecked != null && (bool)r.IsChecked);
                var mode = ConditionMode.CurrentStackFrame;
                if (checkedRadioButton == AllSf)
                {
                    mode = ConditionMode.AllStackFrames;
                }

                var condition = new Config.Condition(conditionBox.Text,
                    regexBox.Text, mode);
                var properties = new List<INodeProperty>();
                if (labelCheckBox.IsChecked != null && (bool)labelCheckBox.IsChecked)
                {
                    var labelProperty = new LabelNodeProperty(labelTextBox.Text);
                    if (Single.TryParse(labelFontSizeBox.Text, out var f))
                    {
                        labelProperty.FontSize = f;
                    }

                    properties.Add(labelProperty);
                }

                if (colorCheckBox.IsChecked != null && (bool)colorCheckBox.IsChecked)
                {
                    var color = colorPicker.SelectedColor;
                    properties.Add(new LineColorNodeProperty(
                        new Microsoft.Msagl.Drawing.Color(a: color.Value.A,
                            r: color.Value.R, g: color.Value.G, b: color.Value.B)));
                }

                if (lineWidthCheckBox.IsChecked != null && (bool)lineWidthCheckBox.IsChecked)
                {
                    properties.Add(new LineWidthNodeProperty(Single.Parse(labelFontSizeBox.Text)));
                }

                if (styleCheckBox.IsChecked != null && (bool)styleCheckBox.IsChecked)
                {
                    ModePanel.Children.OfType<CheckBox>()
                        .Where(cb => cb.IsChecked != null && (bool)cb.IsChecked)
                        .ToList().ForEach(cb => properties.Add(new StyleNodeProperty(_styles[cb.ContentStringFormat])));
                }

                if (shapeCheckBox.IsChecked != null && (bool) shapeCheckBox.IsChecked)
                {
                    properties.Add(new ShapeNodeProperty(_shapesDict[_shapeRadioButtons
                        .FirstOrDefault(r => r.IsChecked != null && (bool) r.IsChecked)
                        .ContentStringFormat]));
                }

                if (fillColorCheckBox.IsChecked != null && (bool) fillColorCheckBox.IsChecked)
                {
                    var color = fillColorPicker.SelectedColor;
                    properties.Add(new FillColorNodeProperty(
                        new Microsoft.Msagl.Drawing.Color(a: color.Value.A,
                            r: color.Value.R, g: color.Value.G, b: color.Value.B)));
                }

                return new ConditionalProperty<INodeProperty>(condition, properties);
            }
        }


        private void Reset()
        {
            colorCheckBox.IsChecked = false;
            labelCheckBox.IsChecked = false;
            colorCheckBox.IsChecked = false;
            lineWidthCheckBox.IsChecked = false;
            fillColorCheckBox.IsChecked = false;
            shapeCheckBox.IsChecked = false;
        }


        public void FromConditionalProperty(int priority, ConditionalProperty<INodeProperty> conditionalProperty)
        {
            Priority = priority;
            Reset();
            conditionBox.Text = conditionalProperty.Condition.Template;
            regexBox.Text = conditionalProperty.Condition.FunctionNameRegex;
            if (conditionalProperty.Condition.Mode == ConditionMode.AllStackFrames)
            {
                Debug.WriteLine($"Node set checked all {AllSf.IsChecked}");
                AllSf.IsChecked = true;
                Debug.WriteLine($"{AllSf.IsChecked}");
            }
            else
            {
                Debug.WriteLine($"Node set checked cur {CurSf.IsChecked}");
                CurSf.IsChecked = true;
                Debug.WriteLine($"{CurSf.IsChecked}");
            }

            var labelProperty =
                (LabelNodeProperty)conditionalProperty.Properties.FirstOrDefault(x => x is LabelNodeProperty);
            if (labelProperty != null)
            {
                labelCheckBox.IsChecked = true;
                labelTextBox.Text = labelProperty.LabelTextExpression;
                labelFontSizeBox.Text = $"{labelProperty.FontSize:0.00}";
            }

            var lineWidthEdgeProperty =
                (LineWidthNodeProperty)conditionalProperty.Properties.FirstOrDefault(x => x is LineWidthNodeProperty);
            if (lineWidthEdgeProperty != null)
            {
                lineWidthCheckBox.IsChecked = true;
                lineWidthBox.Text = $"{lineWidthEdgeProperty.LineWidth:0.00}";
            }

            var lineColorEdgeProperty =
                (LineColorNodeProperty)conditionalProperty.Properties.FirstOrDefault(x => x is LineColorNodeProperty);
            if (lineColorEdgeProperty != null)
            {
                colorCheckBox.IsChecked = true;

                colorPicker.SelectedColor = new Color
                {
                    A = lineColorEdgeProperty.Color.A,
                    G = lineColorEdgeProperty.Color.G,
                    R = lineColorEdgeProperty.Color.R,
                    B = lineColorEdgeProperty.Color.B
                };
            }

            var styles = conditionalProperty.Properties
                .Where(p => p is StyleNodeProperty).Select(p => ((StyleNodeProperty)p).Style).ToList();
            if (styles.Count > 0)
            {
                styleCheckBox.IsChecked = true;
                foreach (var style in styles)
                {
                    KeyValuePair<string, Style> content = _styles.FirstOrDefault(kv => kv.Value == style);
                    if (!content.Equals(default(KeyValuePair<string, Style>)))
                    {
                        foreach (var child in Styles.Children)
                        {
                            if (child is CheckBox cb && cb.ContentStringFormat == content.Key)
                            {
                                cb.IsChecked = true;
                            }
                        }
                    }
                }
            }

            var fillColorEdgeProperty =
                (FillColorNodeProperty)conditionalProperty.Properties.FirstOrDefault(x => x is FillColorNodeProperty);
            if (fillColorEdgeProperty != null)
            {
                fillColorCheckBox.IsChecked = true;

                fillColorPicker.SelectedColor = new Color
                {
                    A = fillColorEdgeProperty.Color.A,
                    G = fillColorEdgeProperty.Color.G,
                    R = fillColorEdgeProperty.Color.R,
                    B = fillColorEdgeProperty.Color.B
                };
            }

            var shape = (ShapeNodeProperty)conditionalProperty.Properties.FirstOrDefault(x => x is ShapeNodeProperty);
            if (shape != null)
            {
                shapeCheckBox.IsChecked = true;
                foreach (var kv in _shapesDict)
                {
                    if (kv.Value.Equals(shape.Shape))
                    {
                        _shapeRadioButtons.Where(r => r.ContentStringFormat == kv.Key).ToList().ForEach(r => r.IsChecked = true);
                    }
                }
            }
        }
    }
}