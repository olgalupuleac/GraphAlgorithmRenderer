using GraphAlgorithmRendererLib.GraphRenderer;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GraphAlgorithmRendererLib.Config
{
    public interface IEdgeProperty
    {
        string Type { get; }
        void Apply(Edge edge, DebuggerOperations debuggerOperations, Identifier identifier);
    }

    public class LabelEdgeProperty : AbstractLabelProperty, IEdgeProperty
    {
        [JsonConstructor]
        public LabelEdgeProperty(string labelTextExpression) : base(labelTextExpression)
        {
            FontSize = 6;
        }

        [JsonProperty(Order = -1)]
        public string Type { get; } = "Label";

        public void Apply(Edge edge, DebuggerOperations debuggerOperations, Identifier identifier)
        {
            if (edge.Label == null)
            {
                edge.LabelText = "";
            }
            ApplyLabel(edge, debuggerOperations, identifier);
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

        [JsonProperty(Order = -2)]
        public string Type { get; } = "LineWidth";

        public void Apply(Edge edge, DebuggerOperations debuggerOperations, Identifier identifier)
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

        [JsonProperty(Order = -1)]
        public string Type { get; } = "LineColor";

        [JsonProperty] public Color Color { get; }

        public void Apply(Edge edge, DebuggerOperations debuggerOperations, Identifier identifier)
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

        [JsonProperty(Order = -1)]
        public string Type { get; } = "Style";

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Style Style { get; }

        public void Apply(Edge edge, DebuggerOperations debuggerOperations, Identifier identifier)
        {
            edge.Attr.AddStyle(Style);
        }
    }

    public class ArrowEdgeProperty : IEdgeProperty
    {
        public bool ArrowAtTarget { get; set; }
        public bool ArrowAtSource { get; set; }

        [JsonProperty(Order = -2)]
        public string Type { get; } = "Arrow";

        [JsonConstructor]
        public ArrowEdgeProperty(bool arrowAtTarget, bool arrowAtSource)
        {
            ArrowAtSource = arrowAtSource;
            ArrowAtTarget = arrowAtTarget;
        }

        public void Apply(Edge edge, DebuggerOperations debuggerOperations, Identifier identifier)
        {
            edge.Attr.ArrowheadAtTarget = ArrowAtTarget ? ArrowStyle.Normal : ArrowStyle.None;
            edge.Attr.ArrowheadAtSource = ArrowAtSource ? ArrowStyle.Normal : ArrowStyle.None;
        }
    }
}