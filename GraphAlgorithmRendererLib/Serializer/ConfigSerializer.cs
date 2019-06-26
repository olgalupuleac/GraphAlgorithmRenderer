using System.Collections.Generic;
using System.Diagnostics;
using GraphAlgorithmRendererLib.Config;
using Newtonsoft.Json;

namespace GraphAlgorithmRendererLib.Serializer
{
    public class ConfigSerializer
    {
        private static readonly JsonSerializerSettings _jsonSettings;

        static ConfigSerializer()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.All,
                //TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                MaxDepth = 100,
                Converters = new List<JsonConverter>
                {
                    new EdgePropertyConverter(),
                    new NodePropertyConverter(),
                    //new OldFormatGraphConverter()
                }
            };
        }


        public static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented, _jsonSettings);
        }

        public static GraphConfig FromJson(string json)
        {
            return JsonConvert.DeserializeObject<GraphConfig>(json, _jsonSettings);
        }
    }
}