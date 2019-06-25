using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GraphAlgorithmRendererLib.Config
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConditionMode
    {
        CurrentStackFrame,
        AllStackFrames,
        AllStackFramesArgsOnly
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

        [JsonIgnore]
        public string WrappedRegex => "^" + FunctionNameRegex + "$";

        [JsonProperty]
        public string FunctionNameRegex { get; set; }
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