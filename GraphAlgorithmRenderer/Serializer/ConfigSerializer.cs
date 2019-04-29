using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using static Newtonsoft.Json.JsonConvert;

namespace GraphAlgorithmRenderer.Serializer
{
    public class ConfigSerializer
    {
        private static readonly JsonSerializerSettings _jsonSettings;

        static ConfigSerializer()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                //TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                MaxDepth = 10
            };
            Debug.WriteLine("Initialized");
        }


        public static string ToJson(object data)
        {
            return SerializeObject(data, Formatting.Indented, _jsonSettings);
        }

        public static Config.GraphConfig FromJson(string json)
        {
            return DeserializeObject<Config.GraphConfig>(json, _jsonSettings);
        }
    }
}