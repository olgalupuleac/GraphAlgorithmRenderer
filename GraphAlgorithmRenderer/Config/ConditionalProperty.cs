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
        public ConditionalProperty(Condition condition, T property)
        {
            Condition = condition;
            Property = property;
        }

        [JsonProperty] public Condition Condition { get; }
        [JsonProperty] public T Property { get; }
    }
}