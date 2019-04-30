using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GraphConfig.Config;
using Newtonsoft.Json;

namespace GraphAlgorithmRenderer.Config
{
    public abstract class GraphElementFamily<T>
    {
        public string Name { get; set; } = "";
        protected GraphElementFamily(List<IdentifierPartTemplate> ranges)
        {
            Ranges = ranges;
            ConditionalProperties = new List<ConditionalProperty<T>>();
        }

        [JsonConstructor]
        protected GraphElementFamily()
        {
        }

        [JsonProperty] public List<IdentifierPartTemplate> Ranges { get; set; }
        //public ReadOnlyCollection<IdentifierPartTemplate> Ranges => _Ranges.AsReadOnly();

        [JsonProperty] public string ValidationTemplate { get; set; }
        [JsonProperty] public List<ConditionalProperty<T>> ConditionalProperties { get; set; }

        public List<ConditionalProperty<T>> GetCurrentStackFrameProperties()
        {
            return ConditionalProperties.Where(conditionalProperty =>
                conditionalProperty.Condition.Mode == ConditionMode.CurrentStackFrame).ToList();
        }

        public List<ConditionalProperty<T>> GetAllStackFramesProperties()
        {
            return ConditionalProperties.Where(conditionalProperty =>
                conditionalProperty.Condition.Mode == ConditionMode.AllStackFrames).ToList();
        }
    }

    public class EdgeFamily : GraphElementFamily<IEdgeProperty>
    {
        public class EdgeEnd
        {
            public EdgeEnd(NodeFamily node, List<string> templates)
            {
                NamesWithTemplates = GetTemplates(node, templates);
            }

            [JsonConstructor]
            private EdgeEnd()
            {
            }

            [JsonProperty] public Dictionary<string, string> NamesWithTemplates { get; set; }

            private Dictionary<string, string> GetTemplates(NodeFamily node, List<string> templates)
            {
                // TODO throw exception
                Debug.Assert(templates.Count == node.Ranges.Count);

                return templates.Select((t, i) => Tuple.Create(node.Ranges[i].Name, t))
                    .ToDictionary(x => x.Item1, x => x.Item2);
            }
        }

        [JsonConstructor]
        public EdgeFamily(List<IdentifierPartTemplate> ranges, EdgeEnd source, EdgeEnd target,
            bool isDirected = false) :
            base(ranges)
        {
            Source = source;
            Target = target;
            IsDirected = isDirected;
        }

        [JsonProperty] public bool IsDirected { get; }

        [JsonProperty] public EdgeEnd Source { get; }
        [JsonProperty] public EdgeEnd Target { get; }
    }

    public class NodeFamily : GraphElementFamily<INodeProperty>
    {
        [JsonConstructor]
        public NodeFamily(List<IdentifierPartTemplate> ranges) : base(ranges)
        {
        }
    }

    public class GraphConfig
    {
        public HashSet<EdgeFamily> Edges { get; set; }
        public HashSet<NodeFamily> Nodes { get; set; }

        public GraphConfig()
        {
            Edges = new HashSet<EdgeFamily>();
            Nodes = new HashSet<NodeFamily>();
        }
    }
}