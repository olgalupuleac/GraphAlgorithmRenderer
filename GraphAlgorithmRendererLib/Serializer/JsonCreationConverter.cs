using System;
using System.Text.RegularExpressions;
using GraphAlgorithmRendererLib.Config;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphAlgorithmRendererLib.Serializer
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jObject);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                var target = Create(objectType, jObject);
                serializer.Populate(jObject.CreateReader(), target);
                return target;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
        }
    }

    public class EdgePropertyConverter : JsonCreationConverter<IEdgeProperty>
    {
        protected override IEdgeProperty Create(Type objectType, JObject jObject)
        {
            var actualType = jObject["Type"] != null
                ? jObject["Type"].Value<string>()
                : jObject["$type"].Value<string>();
            switch (actualType)
            {
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.LabelEdgeProperty, GraphAlgorithmRenderer)|(Label)$").IsMatch(type):
                    return jObject.ToObject<LabelEdgeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.LineWidthEdgeProperty, GraphAlgorithmRenderer)|(LineWidth)$").IsMatch(type):
                    return jObject.ToObject<LineWidthEdgeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.LineColorEdgeProperty, GraphAlgorithmRenderer)|(LineColor)$").IsMatch(type):
                    return jObject.ToObject<LineColorEdgeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.StyleEdgeProperty, GraphAlgorithmRenderer)|(Style)$").IsMatch(type):
                    return jObject.ToObject<StyleEdgeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.ArrowProperty, GraphAlgorithmRenderer)|(Arrow)$").IsMatch(type):
                    return jObject.ToObject<ArrowEdgeProperty>();
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class NodePropertyConverter : JsonCreationConverter<INodeProperty>
    {
        protected override INodeProperty Create(Type objectType, JObject jObject)
        {
            var actualType = jObject["Type"] != null
                ? jObject["Type"].Value<string>()
                : jObject["$type"].Value<string>();

            switch (actualType)
            {
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.LabelNodeProperty, GraphAlgorithmRenderer)|(Label)$").IsMatch(type):
                    return jObject.ToObject<LabelNodeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.LineWidthNodeProperty, GraphAlgorithmRenderer)|(LineWidth)$").IsMatch(type):
                    return jObject.ToObject<LineWidthNodeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.LineColorNodeProperty, GraphAlgorithmRenderer)|(LineColor)$").IsMatch(type):
                    return jObject.ToObject<LineColorNodeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.StyleNodeProperty, GraphAlgorithmRenderer)|(Style)$").IsMatch(type):
                    return jObject.ToObject<StyleNodeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.FillColorNodeProperty, GraphAlgorithmRenderer)|(FillColor)$").IsMatch(type):
                    return jObject.ToObject<FillColorNodeProperty>();
                case var type when new Regex(@"^(GraphAlgorithmRenderer\.Config\.ShapeNodeProperty, GraphAlgorithmRenderer)|(Shape)$").IsMatch(type):
                    return jObject.ToObject<ShapeNodeProperty>();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}