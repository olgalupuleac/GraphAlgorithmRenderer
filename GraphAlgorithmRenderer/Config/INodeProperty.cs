using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using GraphAlgorithmRenderer.GraphRenderer;
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

    public class LabelNodeProperty : AbstractLabelProperty, INodeProperty
    {
        [JsonConstructor]
        public LabelNodeProperty(string labelTextExpression) : base(labelTextExpression)
        {
        }

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