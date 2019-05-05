using System.Collections.Generic;
using Newtonsoft.Json;

namespace GraphConfig.Config
{
    public enum ConditionMode
    {
        CurrentStackFrame,
        AllStackFrames
    }

    public class Condition
    {
        [JsonConstructor]
        public Condition(string template, string functionNameRegex = @".*",
            ConditionMode mode = ConditionMode.CurrentStackFrame)
        {
            Template = template;
            FunctionNameRegex = functionNameRegex;
            Mode = mode;
        }
        [JsonProperty] public string Template { get; }
        [JsonProperty] public ConditionMode Mode { get; }
        [JsonProperty] public string FunctionNameRegex { get; }
    }

    public class ConditionalProperty<T>
    {
        [JsonConstructor]
        public ConditionalProperty(Condition condition, List<T> properties)
        {
            Condition = condition;
            Properties = properties;
        }

        public ConditionalProperty(Condition condition, T property)
        {
            Condition = condition;
            Properties = new List<T> { property };
        }

        [JsonProperty] public Condition Condition { get; }
        [JsonProperty] public List<T> Properties { get; set; }
    }
}