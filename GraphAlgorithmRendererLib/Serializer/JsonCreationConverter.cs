using System;
using GraphAlgorithmRendererLib.Config;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphAlgorithmRendererLib.Serializer
{
    public abstract  class JsonCreationConverter<T> : JsonConverter
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
            switch (jObject["Type"].Value<string>())
            {
                case "Label":
                    return new LabelEdgeProperty(jObject["LabelTextExpression"].Value<string>())
                    {
                        FontSize = jObject["FontSize"].Value<double?>()
                    };
                case "LineWidth":
                    return new LineWidthEdgeProperty(jObject["LineWidth"].Value<double>());
                case "LineColor":
                    return new LineColorEdgeProperty(new Color(jObject["Color"]["A"].Value<byte>(),
                        jObject["Color"]["R"].Value<byte>(), jObject["Color"]["G"].Value<byte>(), jObject["Color"]["B"].Value<byte>()));
                case "Style":
                    return new StyleEdgeProperty((Style) Enum.Parse(typeof(Style), jObject["Style"].Value<string>()));
                case "Arrow":
                    return new ArrowEdgeProperty(jObject["ArrowAtTarget"].Value<bool>(), jObject["ArrowAtSource"].Value<bool>());
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class NodePropertyConverter : JsonCreationConverter<INodeProperty>
    {
        protected override INodeProperty Create(Type objectType, JObject jObject)
        {
            switch (jObject["Type"].Value<string>())
            {
                case "Label":
                    return new LabelNodeProperty(jObject["LabelTextExpression"].Value<string>())
                    {
                        FontSize = jObject["FontSize"].Value<double?>()
                    };
                case "LineWidth":
                    return new LineWidthNodeProperty(jObject["LineWidth"].Value<double>());
                case "LineColor":
                    return new LineColorNodeProperty(new Color(jObject["Color"]["A"].Value<byte>(),
                        jObject["Color"]["R"].Value<byte>(), jObject["Color"]["G"].Value<byte>(), jObject["Color"]["B"].Value<byte>()));
                case "Style":
                    return new StyleNodeProperty((Style)Enum.Parse(typeof(Style), jObject["Style"].Value<string>()));
                case "FillColor":
                    return new FillColorNodeProperty(new Color(jObject["Color"]["A"].Value<byte>(),
                        jObject["Color"]["R"].Value<byte>(), jObject["Color"]["G"].Value<byte>(), jObject["Color"]["B"].Value<byte>()));
                case "Shape":
                    return new ShapeNodeProperty((Shape)Enum.Parse(typeof(Shape), jObject["Shape"].Value<string>()));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}