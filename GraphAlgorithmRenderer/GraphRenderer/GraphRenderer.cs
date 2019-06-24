using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using EnvDTE;
using GraphAlgorithmRenderer.Config;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
using static GraphAlgorithmRenderer.GraphRenderer.DebuggerOperations;
using Debugger = EnvDTE.Debugger;
using StackFrame = EnvDTE.StackFrame;

namespace GraphAlgorithmRenderer.GraphRenderer
{
    public class GraphRenderer
    {
        private readonly GraphAlgorithmRenderer.Config.GraphConfig _config;
        private readonly Debugger _debugger;
        private Graph _graph;
        private readonly Dictionary<Identifier, Edge> _edges;
        private readonly Dictionary<Identifier, Node> _nodes;


        public GraphRenderer(GraphAlgorithmRenderer.Config.GraphConfig config, Debugger debugger)
        {
            _config = config;
            _debugger = debugger;
            _graph = new Graph();
            _edges = new Dictionary<Identifier, Edge>();
            _nodes = new Dictionary<Identifier, Node>();
        }

        public Graph RenderGraph()
        {
            _graph = new Graph();
            _edges.Clear();
            _nodes.Clear();
            foreach (var nodeFamily in _config.Nodes)
            {
                void NodeAddition(GraphElementFamily<INodeProperty> family, Identifier identifier) =>
                    AddNode(family as NodeFamily, identifier);

                ProcessGraphElementFamily(nodeFamily, NodeAddition, ApplyNodeProperty);
            }

            foreach (var edgeFamily in _config.Edges)
            {
                void EdgeAddition(GraphElementFamily<IEdgeProperty> family, Identifier identifier) =>
                    AddEdge(family as EdgeFamily, identifier);

                ProcessGraphElementFamily(edgeFamily, EdgeAddition, ApplyEdgeProperty);  
            }
            WriteDebugOutput();
            Reset();

            return _graph;
        }

        // It should add element and identifier to corresponding dictionary.
        private delegate void AddGraphElement<T>(GraphElementFamily<T> family, Identifier identifier);

        private void AddEdge(EdgeFamily edgeFamily,
            Identifier identifier)
        {
            var source = NodeIdentifier(edgeFamily.Source, identifier, $"Cannot identify source node for edge family {edgeFamily.Name}");
            var target = NodeIdentifier(edgeFamily.Target, identifier, $"Cannot identify target node for edge family {edgeFamily.Name}");
           
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

        private void AddNode(NodeFamily nodeFamily, Identifier identifier)
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
                GetIdentifiersForCondition(Identifier.GetIdentifiers(family.Name, family.Ranges, _debugger), family.ValidationTemplate);
            identifiers.ForEach(x => add(family, x));
            return identifiers;
        }

        private delegate void ApplyProperty<in T>(T property, Identifier identifier);

        private void ApplyEdgeProperty(IEdgeProperty property, Identifier identifier)
        {
            var edge = _edges[identifier];
            property.Apply(edge, _debugger,
                identifier);
        }

        private void ApplyNodeProperty(INodeProperty property, Identifier identifier)
        {
            var node = _nodes[identifier];
            property.Apply(node,
                _debugger, identifier);
        }

        private void ApplyPropertyStackFrame<T>(IEnumerable<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            identifiers.Where(id =>
                    CheckExpressionForIdentifier(conditionalProperty.Condition.Template, id, _debugger))
                .ToList().ForEach(id => conditionalProperty.Properties.ForEach(p => applyProperty(p, id)));
        }

        private void ApplyPropertyCurrentStackFrame<T>(IEnumerable<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {

            if (!Regex.IsMatch(FunctionName(CurrentStackFrame(_debugger)),
                conditionalProperty.Condition.WrappedRegex()))
            {
                return;
            }

            ApplyPropertyStackFrame(identifiers, conditionalProperty, applyProperty);
        }

        private void ApplyPropertyAllStackFrames<T>(IReadOnlyCollection<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            var currentStackFrame = CurrentStackFrame(_debugger);
            var stackFrames = GetStackFrames(_debugger);
            foreach (StackFrame stackFrame in stackFrames)
            {
                if (!Regex.IsMatch(FunctionName(stackFrame), conditionalProperty.Condition.WrappedRegex()))
                {
                    continue;
                }

                SetStackFrame(stackFrame, _debugger);
                ApplyPropertyStackFrame(identifiers,
                    conditionalProperty, applyProperty);
            }

            SetStackFrame(currentStackFrame, _debugger);
        }


        private void ApplyPropertyAllStackFramesArgsOnly<T>(IReadOnlyCollection<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            var stackFrames = GetStackFrames(_debugger);
            foreach (StackFrame stackFrame in stackFrames)
            {
                if (!Regex.IsMatch(FunctionName(stackFrame), conditionalProperty.Condition.WrappedRegex()))
                {
                    continue;
                }

                foreach (var id in identifiers)
                {
                    var expressionString = Substitute(conditionalProperty.Condition.Template, id, stackFrame);
                    if (CheckExpression(expressionString, _debugger))
                    {
                        conditionalProperty.Properties.ForEach(p => applyProperty(p, id));
                    }
                }
            }
        }

        private List<Identifier> GetIdentifiersForCondition(List<Identifier> identifiers, string conditionTemplate)
        {
            return String.IsNullOrEmpty(conditionTemplate)
                ? identifiers
                : identifiers.Where(id =>
                    CheckExpressionForIdentifier(conditionTemplate, id, _debugger)).ToList();
        }


        private Identifier NodeIdentifier(EdgeFamily.EdgeEnd edgeEnd, Identifier identifier, string message)
        {
            var templates = edgeEnd.NamesWithTemplates;
            var res = new List<IdentifierPart>();
            foreach (var template in templates)
            {
                var value = Identifier.GetNumber(template.Value ?? "", identifier, _debugger, message);
                res.Add(new IdentifierPart(template.Key, value));
            }

            return new Identifier(edgeEnd.NodeFamilyName, res);
        }
    }
}