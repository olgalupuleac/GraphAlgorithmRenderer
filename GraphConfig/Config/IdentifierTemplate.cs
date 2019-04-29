using Newtonsoft.Json;

namespace GraphConfig.Config
{
    public class IdentifierPartTemplate
    {
        public string Name { get; }
        public string BeginTemplate { get; }
        public string EndTemplate { get; }

        [JsonConstructor]
        public IdentifierPartTemplate(string name, string beginTemplate, string endTemplate)
        {
            Name = name;
            BeginTemplate = beginTemplate;
            EndTemplate = endTemplate;
        }
    }
}