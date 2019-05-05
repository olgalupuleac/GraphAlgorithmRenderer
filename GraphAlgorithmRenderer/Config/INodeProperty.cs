using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using GraphAlgorithmRenderer.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using Debugger = EnvDTE.Debugger;

namespace GraphAlgorithmRenderer.Config
{
    public interface INodeProperty
    {
        void Apply(Node node, Debugger debugger, Identifier identifier);
    }

    public class FillColorNodeProperty : INodeProperty
    {
        [JsonConstructor]
        public FillColorNodeProperty(Color color)
        {
            Color = color;
        }

        [JsonProperty] public Color Color { get; }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.FillColor = Color;
        }
    }

    public class ShapeNodeProperty : INodeProperty
    {
        [JsonConstructor]
        public ShapeNodeProperty(Shape shape)
        {
            Shape = shape;
        }

        [JsonProperty] public Shape Shape { get; }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.Shape = Shape;
        }
    }

    public class LabelNodeProperty : INodeProperty
    {
        [JsonConstructor]
        public LabelNodeProperty(string labelTextExpression)
        {
            LabelTextExpression = labelTextExpression;
        }

        public bool HighlightIfChanged { get; set; }
        public Color? ColorToHighLight { get; set; }
        [JsonProperty] public string LabelTextExpression { get; }
        public double FontSize { get; set; }
        public FontStyle? FontStyle { get; set; }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var expression = GraphRenderer.GraphRenderer.Substitute(LabelTextExpression, identifier, debugger);
            var label = Regex.Replace(expression, @"{.*?}", delegate (Match match)
            {
                string v = match.ToString();
                v = v.Substring(1, v.Length - 2);
                //Debug.WriteLine(v);
                ThreadHelper.ThrowIfNotOnUIThread();
                return debugger.GetExpression(v).Value;
            });
            node.Label.FontStyle = FontStyle ?? node.Label.FontStyle;
            if (Math.Abs(FontSize) > 0.01)
            {
                node.Label.FontSize = FontSize;
            }

            if (label.Equals(node.LabelText))
            {
                return;
            }

            node.Label.Text = label;
            if (HighlightIfChanged)
            {
                node.Label.FontColor = ColorToHighLight ?? Color.Red;
            }
        }
    }

    public class LineWidthNodeProperty : INodeProperty
    {
        [JsonProperty] public double LineWidth { get; }

        [JsonConstructor]
        public LineWidthNodeProperty(double lineWidth)
        {
            LineWidth = lineWidth;
        }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.LineWidth = LineWidth;
        }
    }

    public class LineColorNodeProperty : INodeProperty
    {
        [JsonConstructor]
        public LineColorNodeProperty(Color color)
        {
            Color = color;
        }

        [JsonProperty] public Color Color { get; }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.Color = Color;
        }
    }

    public class StyleNodeProperty : INodeProperty
    {
        [JsonConstructor]
        public StyleNodeProperty(Style style)
        {
            Style = style;
        }

        [JsonProperty] public Style Style { get; }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.AddStyle(Style);
        }
    }
}