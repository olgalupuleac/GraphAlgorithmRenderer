using GraphAlgorithmRendererLib.GraphRenderer;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using Debugger = EnvDTE.Debugger;

namespace GraphAlgorithmRendererLib.Config
{
    public interface IEdgeProperty
    {
        void Apply(Edge edge, Debugger debugger, Identifier identifier);
    }

    public class LabelEdgeProperty : AbstractLabelProperty, IEdgeProperty
    {
        [JsonConstructor]
        public LabelEdgeProperty(string labelTextExpression) : base(labelTextExpression)
        {
            FontSize = 6;
        }

        public void Apply(Edge edge, Debugger debugger, Identifier identifier)
        {
            if (edge.Label == null)
            {
                edge.LabelText = "";
            }
            ApplyLabel(edge, debugger, identifier);
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

    public class ArrowProperty : IEdgeProperty
    {
        public bool ArrowAtTarget { get; set; }
        public bool ArrowAtSource { get; set; }
        [JsonConstructor]
        public ArrowProperty(bool arrowAtTarget, bool arrowAtSource)
        {
            ArrowAtSource = arrowAtSource;
            ArrowAtTarget = arrowAtTarget;
        }

        public void Apply(Edge edge, Debugger debugger, Identifier identifier)
        {
            edge.Attr.ArrowheadAtTarget = ArrowAtTarget ? ArrowStyle.Normal : ArrowStyle.None;
            edge.Attr.ArrowheadAtSource = ArrowAtSource ? ArrowStyle.Normal : ArrowStyle.None;
        }
    }
}