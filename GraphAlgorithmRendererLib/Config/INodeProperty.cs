using GraphAlgorithmRendererLib.GraphRenderer;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Debugger = EnvDTE.Debugger;

namespace GraphAlgorithmRendererLib.Config
{
    public interface INodeProperty
    {
        string Type { get; }
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

        [JsonProperty(Order = -2)]
        public string Type { get; } = "FillColor";

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

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Shape Shape { get; }

        [JsonProperty(Order = -2)]
        public string Type { get; } = "Shape";

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.Shape = Shape;
        }
    }

    public class LabelNodeProperty : AbstractLabelProperty, INodeProperty
    {
        [JsonConstructor]
        public LabelNodeProperty(string labelTextExpression) : base(labelTextExpression)
        {
        }

        [JsonProperty(Order = -1)]
        public string Type { get; } = "Label";

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            ApplyLabel(node, debugger, identifier);
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

        [JsonProperty(Order = -2)]
        public string Type { get; } = "LineWidth";

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

        [JsonProperty]
        public Color Color { get; }

        [JsonProperty(Order = -2)]
        public string Type { get; } = "LineColor";

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

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Style Style { get; }

        [JsonProperty(Order = -2)]
        public string Type { get; } = "Style";

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.AddStyle(Style);
        }
    }
}