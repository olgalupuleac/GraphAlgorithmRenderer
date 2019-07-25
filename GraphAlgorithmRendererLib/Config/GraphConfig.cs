using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Msagl.Core.DataStructures;
using Newtonsoft.Json;

namespace GraphAlgorithmRendererLib.Config
{
    public abstract class GraphElementFamily<T> : IValidatable
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

        public void Validate()
        {
            Ranges.ForEach(r => r.Validate());
            ConditionalProperties.ForEach(x => x.Condition.Validate());
            if (Ranges.Select(x => x.Name).Distinct().Count() < Ranges.Count)
            {
                throw new ValidationException($"{Name}: indices must have different names");
            }
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

    public class GraphConfig : IValidatable
    {
        [JsonProperty(Order = 1)]
        public List<EdgeFamily> Edges { get; set; }
        [JsonProperty]
        public List<NodeFamily> Nodes { get; set; }

        public GraphConfig()
        {
            Edges = new List<EdgeFamily>();
            Nodes = new List<NodeFamily>();
        }

        public void Validate()
        {
            Nodes.ForEach(x => x.Validate());
            Edges.ForEach(x => x.Validate());
            if (Nodes.Select(x => x.Name).Distinct().Count() < Nodes.Count)
            {
                throw new ValidationException("Node family names must be unique");
            }
            if (Edges.Select(x => x.Name).Distinct().Count() < Edges.Count)
            {
                throw new ValidationException("Edge family names must be unique");
            }

           foreach (var edgeFamily in Edges)
           {
               var sourceNode = Nodes.FirstOrDefault(x => x.Name.Equals(edgeFamily.Source.NodeFamilyName));
                if (sourceNode == null)
                {
                    throw new ValidationException($"Cannot define a source of edge family {edgeFamily.Name}: node family with name {edgeFamily.Source.NodeFamilyName} doesn't exist");
                }
                var targetNode = Nodes.FirstOrDefault(x => x.Name.Equals(edgeFamily.Target.NodeFamilyName));
                if (targetNode == null)
                {
                    throw new ValidationException($"Cannot define a target of edge family {edgeFamily.Name}: node family with name {edgeFamily.Target.NodeFamilyName} doesn't exist");
                }

                TwoSetsEquals(edgeFamily.Source.NamesWithTemplates.Keys.ToList(), 
                    sourceNode.Ranges.Select(x => x.Name).ToList(),
                    $"Source indices in edge family {edgeFamily.Name} differ with indices of node family {sourceNode.Name}");

                TwoSetsEquals(edgeFamily.Target.NamesWithTemplates.Keys.ToList(),
                    targetNode.Ranges.Select(x => x.Name).ToList(),
                    $"Target indices in edge family {edgeFamily.Name} differ with indices of node family {targetNode.Name}");
            }
        }


        private void TwoSetsEquals(IReadOnlyCollection<string> a, IReadOnlyCollection<string> b, string errorMessage)
        {
            if (a.Count != b.Count)
            {
                throw new ValidationException(errorMessage);
            }

            var intersect = a.Intersect(b);
            if (intersect.Count() != a.Count)
            {
                throw new ValidationException(errorMessage);
            }
        }
    }
}