using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace GraphAlgorithmRendererLib.Config
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
            public string NodeFamilyName { get; set; }
            public EdgeEnd(NodeFamily node, List<string> templates)
            {
                NamesWithTemplates = GetTemplates(node, templates);
                NodeFamilyName = node.Name;
            }

            [JsonConstructor]
            public EdgeEnd()
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
        public EdgeFamily(List<IdentifierPartTemplate> ranges, EdgeEnd source, EdgeEnd target) :
            base(ranges)
        {
            Source = source;
            Target = target;
        }

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
        public List<EdgeFamily> Edges { get; set; }
        public List<NodeFamily> Nodes { get; set; }

        public GraphConfig()
        {
            Edges = new List<EdgeFamily>();
            Nodes = new List<NodeFamily>();
        }
    }
}