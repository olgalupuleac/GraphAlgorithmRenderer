using Newtonsoft.Json;
using static System.String;

namespace GraphAlgorithmRendererLib.Config
{
    public class IdentifierPartTemplate : IValidatable
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

        public void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException("Index name should not be null");
            }
            if (IsNullOrWhiteSpace(BeginTemplate))
            {
                throw new ValidationException($"{Name}: begin template is null or whitespace");
            }
            if (IsNullOrWhiteSpace(EndTemplate))
            {
                throw new ValidationException($"{Name}: end template is null or whitespace");
            }
        }
    }
}