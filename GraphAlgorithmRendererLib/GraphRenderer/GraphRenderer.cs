using System;
using System.Collections.Generic;
using System.Linq;
using GraphAlgorithmRendererLib.Config;
using Microsoft.Msagl.Drawing;

namespace GraphAlgorithmRendererLib.GraphRenderer
{
    public class GraphRenderer
    {
        private GraphConfig _config;
        private Graph _graph;
        private readonly Dictionary<Identifier, Edge> _edges;
        private readonly Dictionary<Identifier, Node> _nodes;
        private readonly DebuggerOperations _debuggerOperations;


        public GraphRenderer(DebuggerOperations debuggerOperations)
        {
            _graph = new Graph();
            _edges = new Dictionary<Identifier, Edge>();
            _nodes = new Dictionary<Identifier, Node>();
            _debuggerOperations = debuggerOperations;
        }

        public Graph RenderGraph(GraphConfig config)
        {
            _config = config;
            _graph = new Graph();
            _edges.Clear();
            _nodes.Clear();
            foreach (var nodeFamily in _config.Nodes)
            {
                void NodeAddition(GraphElementFamily<INodeProperty> family, Identifier identifier) =>
                    AddNode(identifier);

                ProcessGraphElementFamily(nodeFamily, NodeAddition, ApplyNodeProperty);
            }

            foreach (var edgeFamily in _config.Edges)
            {
                void EdgeAddition(GraphElementFamily<IEdgeProperty> family, Identifier identifier) =>
                    AddEdge(family as EdgeFamily, identifier);

                ProcessGraphElementFamily(edgeFamily, EdgeAddition, ApplyEdgeProperty);
            }

            _debuggerOperations.WriteDebugOutput();
            _debuggerOperations.Reset();

            return _graph;
        }

        // It should add element and identifier to corresponding dictionary.
        private delegate void AddGraphElement<T>(GraphElementFamily<T> family, Identifier identifier);

        private void AddEdge(EdgeFamily edgeFamily,
            Identifier identifier)
        {
            var source = NodeIdentifier(edgeFamily.Source, identifier,
                $"Cannot identify source node for edge family {edgeFamily.Name}");
            var target = NodeIdentifier(edgeFamily.Target, identifier,
                $"Cannot identify target node for edge family {edgeFamily.Name}");

            var sourceNode = _graph.FindNode(source.Id());
            var targetNode = _graph.FindNode(target.Id());
            if (targetNode == null)
            {
                throw new GraphRenderException($"Target node {source.Id()} does not exist");
            }

            if (sourceNode == null)
            {
                throw new GraphRenderException($"Source node {source.Id()} does not exist");
            }

            var edge = _graph.AddEdge(source.Id(), target.Id());
            _edges[identifier] = edge;
            edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
        }

        private void AddNode(Identifier identifier)
        {
            _nodes[identifier] = _graph.AddNode(identifier.Id());
        }

        private void ProcessGraphElementFamily<T>(GraphElementFamily<T> family,
            AddGraphElement<T> add, ApplyProperty<T> applyProperty)
        {
            var identifiers = Create(family, add);
            var conditionalProperties = family.ConditionalProperties.Select(x => x).ToList();
            conditionalProperties.Reverse();
            foreach (var conditionalProperty in conditionalProperties)
            {
                switch (conditionalProperty.Condition.Mode)
                {
                    case ConditionMode.AllStackFrames:
                        ApplyPropertyAllStackFrames(identifiers, conditionalProperty, applyProperty);
                        break;
                    case ConditionMode.CurrentStackFrame:
                        ApplyPropertyCurrentStackFrame(identifiers, conditionalProperty, applyProperty);
                        break;
                    case ConditionMode.AllStackFramesArgsOnly:
                        ApplyPropertyAllStackFramesArgsOnly(identifiers, conditionalProperty, applyProperty);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private List<Identifier> Create<T>(GraphElementFamily<T> family, AddGraphElement<T> add)
        {
            var identifiers =
                GetIdentifiersForCondition(Identifier.GetIdentifiers(family.Name, family.Ranges, _debuggerOperations),
                    family.ValidationTemplate);
            identifiers.ForEach(x => add(family, x));
            return identifiers;
        }

        private delegate void ApplyProperty<in T>(T property, Identifier identifier);

        private void ApplyEdgeProperty(IEdgeProperty property, Identifier identifier)
        {
            var edge = _edges[identifier];
            property.Apply(edge, _debuggerOperations,
                identifier);
        }

        private void ApplyNodeProperty(INodeProperty property, Identifier identifier)
        {
            var node = _nodes[identifier];
            property.Apply(node,
                _debuggerOperations, identifier);
        }

        private void ApplyPropertyStackFrame<T>(List<Identifier> identifiers,
            List<T> properties,
            ApplyProperty<T> applyProperty)
        {
            identifiers.ForEach(id => properties.ForEach(p => applyProperty(p, id)));
        }

        private void ApplyPropertyCurrentStackFrame<T>(IEnumerable<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            ApplyPropertyStackFrame(
                _debuggerOperations.CheckExpression(conditionalProperty.Condition.Template,
                    conditionalProperty.Condition.FunctionNameRegex, identifiers), conditionalProperty.Properties,
                applyProperty);
        }

        private void ApplyPropertyAllStackFrames<T>(IReadOnlyCollection<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            ApplyPropertyStackFrame(
                _debuggerOperations.CheckExpressionAllStackFrames(conditionalProperty.Condition.Template,
                    conditionalProperty.Condition.FunctionNameRegex, identifiers), conditionalProperty.Properties,
                applyProperty);
        }


        private void ApplyPropertyAllStackFramesArgsOnly<T>(IReadOnlyCollection<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            ApplyPropertyStackFrame(
                _debuggerOperations.CheckExpressionAllStackFramesArgsOnly(conditionalProperty.Condition.Template,
                    conditionalProperty.Condition.FunctionNameRegex, identifiers), conditionalProperty.Properties,
                applyProperty);
        }

        private List<Identifier> GetIdentifiersForCondition(List<Identifier> identifiers, string conditionTemplate)
        {
            return String.IsNullOrEmpty(conditionTemplate)
                ? identifiers
                : identifiers.Where(id =>
                    _debuggerOperations.CheckExpressionForIdentifier(conditionTemplate, id)).ToList();
        }


        private Identifier NodeIdentifier(EdgeFamily.EdgeEnd edgeEnd, Identifier identifier, string message)
        {
            var templates = edgeEnd.NamesWithTemplates;
            var res = new List<IdentifierPart>();
            foreach (var template in templates)
            {
                var value = Identifier.GetNumber(template.Value ?? "", identifier, _debuggerOperations, message);
                res.Add(new IdentifierPart(template.Key, value));
            }

            return new Identifier(edgeEnd.NodeFamilyName, res);
        }
    }
}