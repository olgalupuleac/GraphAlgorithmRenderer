using System;
using EnvDTE;
using GraphConfig.GraphElementIdentifier;
using GraphConfiguration.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;

namespace GraphConfig.Config
{
    public interface IEdgeProperty
    {
        void Apply(Edge edge, Debugger debugger, Identifier identifier);
    }

    public class LabelEdgeProperty : IEdgeProperty
    {
        [JsonConstructor]
        public LabelEdgeProperty(string labelTextExpression)
        {
            LabelTextExpression = labelTextExpression;
        }

        public string SubstitutedLabelTextExpression(Identifier identifier)
        {
            return identifier.Substitute(LabelTextExpression);
        }

        public bool HighlightIfChanged { get; set; }
        public Color? ColorToHighLight { get; set; }
        [JsonProperty] public string LabelTextExpression { get; }
        public double FontSize { get; set; }
        public FontStyle? FontStyle { get; set; }

        public void Apply(Edge edge, Debugger debugger, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var expression = GraphRenderer.GraphRenderer.Substitute(LabelTextExpression, identifier, debugger);
            var label = debugger.GetExpression(expression).Value;
            if (edge.Label == null)
            {
                edge.Label = new Label(label);
            }

            edge.Label.FontStyle = FontStyle ?? edge.Label.FontStyle;
            if (Math.Abs(FontSize) > 0.01)
            {
                edge.Label.FontSize = FontSize;
            }

            if (label.Equals(edge.Label.Text))
            {
                return;
            }

            edge.Label.Text = label;
            if (HighlightIfChanged)
            {
                edge.Label.FontColor = ColorToHighLight ?? Color.Red;
            }
        }
    }

    public class LineWidthEdgeProperty : IEdgeProperty
    {
        [JsonProperty] public double LineWidth { get; }

        [JsonConstructor]
        public LineWidthEdgeProperty(double lineWidth)
        {
            LineWidth = lineWidth;
        }

        public void Apply(Edge edge, Debugger debugger, Identifier identifier)
        {
            edge.Attr.LineWidth = LineWidth;
        }
    }

    public class LineColorEdgeProperty : IEdgeProperty
    {
        [JsonConstructor]
        public LineColorEdgeProperty(Color color)
        {
            Color = color;
        }

        [JsonProperty] public Color Color { get; }

        public void Apply(Edge edge, Debugger debugger, Identifier identifier)
        {
            edge.Attr.Color = Color;
        }
    }

    public class StyleEdgeProperty : IEdgeProperty
    {
        [JsonConstructor]
        public StyleEdgeProperty(Style style)
        {
            Style = style;
        }

        [JsonProperty] public Style Style { get; }

        public void Apply(Edge edge, Debugger debugger, Identifier identifier)
        {
            edge.Attr.AddStyle(Style);
        }
    }
}