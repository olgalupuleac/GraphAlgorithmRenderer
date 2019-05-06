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

        public bool HighlightIfChanged { get; set; }
        public Color? ColorToHighLight { get; set; }
        [JsonProperty] public string LabelTextExpression { get; }
        public double FontSize { get; set; } = 6;

        public void Apply(Edge edge, Debugger debugger, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
           
            var expression = global::GraphAlgorithmRenderer.GraphRenderer.GraphRenderer.Substitute(LabelTextExpression, identifier, debugger);
            var label = Regex.Replace(expression, @"{.*?}", delegate (Match match)
            {
                string v = match.ToString();
                v = v.Substring(1, v.Length - 2);
                //Debug.WriteLine(v);
                return debugger.GetExpression(v).Value;
            });

            edge.LabelText = label;

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