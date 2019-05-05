using Newtonsoft.Json;

namespace GraphAlgorithmRenderer.Config
{
    public class IdentifierPartTemplate
    {
        public string Name { get; set; }
        public string BeginTemplate { get; set; }
        public string EndTemplate { get; set; }

        public IdentifierPartTemplate()
        {
        }

        [JsonConstructor]
        public IdentifierPartTemplate(string name, string beginTemplate, string endTemplate)
        {
            Name = name;
            BeginTemplate = beginTemplate;
            EndTemplate = endTemplate;
        }
    }
}