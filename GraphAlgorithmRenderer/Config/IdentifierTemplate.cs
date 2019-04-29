using Newtonsoft.Json;

namespace GraphAlgorithmRenderer.Config
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