using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using GraphAlgorithmRenderer.Config;
using GraphAlgorithmRenderer.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
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
        private TimeSpan _getExpressionTimeSpan;
        private TimeSpan _setCurrentStackFrameTimeSpan;
        private int _getExpressionCallsNumber;
        private int _setCurrentStackFrameNumber;

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
            ThreadHelper.ThrowIfNotOnUIThread();
            _graph = new Graph();
            _edges.Clear();
            _nodes.Clear();
            _getExpressionCallsNumber = 0;
            _getExpressionTimeSpan = new TimeSpan();
            _setCurrentStackFrameNumber = 0;
            _setCurrentStackFrameTimeSpan = new TimeSpan();
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

            var getExpressionTime =
                $"{_getExpressionTimeSpan.Hours:00}:{_getExpressionTimeSpan.Minutes:00}:{_getExpressionTimeSpan.Seconds:00}.{_getExpressionTimeSpan.Milliseconds / 10:00}";
            Debug.WriteLine($"got {_getExpressionCallsNumber} expressions in {getExpressionTime}");

            var setStackFrameTime =
                $"{_setCurrentStackFrameTimeSpan.Hours:00}:{_setCurrentStackFrameTimeSpan.Minutes:00}:{_setCurrentStackFrameTimeSpan.Seconds:00}.{_setCurrentStackFrameTimeSpan.Milliseconds / 10:00}";
            Debug.WriteLine($"set {_setCurrentStackFrameNumber} stack frames in {setStackFrameTime}");
            return _graph;
        }

        // It should add element and identifier to corresponding dictionary.
        private delegate void AddGraphElement<T>(GraphElementFamily<T> family, Identifier identifier);

        private void AddEdge(EdgeFamily edgeFamily,
            Identifier identifier)
        {
            //TODO check IsValidValue
            var source = NodeIdentifier(edgeFamily.Source, identifier).Id();
            var target = NodeIdentifier(edgeFamily.Target, identifier).Id();
            var sourceNode = _graph.FindNode(source);
            var targetNode = _graph.FindNode(target);
            if (targetNode == null || sourceNode == null)
            {
                //TODO more specific exception
                throw new SystemException("Target or source nodes do not exist");
            }

            var edge = _graph.AddEdge(source, target);
            _edges[identifier] = edge;
            if (!edgeFamily.IsDirected)
            {
                edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
            }
        }

        private void AddNode(NodeFamily nodeFamily, Identifier identifier)
        {
            _nodes[identifier] = _graph.AddNode(identifier.Id());
        }

        private void ProcessGraphElementFamily<T>(GraphElementFamily<T> family,
            AddGraphElement<T> add, ApplyProperty<T> applyProperty)
        {
            var identifiers = Create(family, add);
            foreach (var conditionalProperty in family.ConditionalProperties)
            {
                switch (conditionalProperty.Condition.Mode)
                {
                    case ConditionMode.AllStackFrames:
                        ApplyPropertyForAllStackFrames(identifiers, conditionalProperty, applyProperty);
                        break;
                    case ConditionMode.CurrentStackFrame:
                        ApplyConditionalProperty(identifiers, conditionalProperty, applyProperty);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private List<Identifier> Create<T>(GraphElementFamily<T> family, AddGraphElement<T> add)
        {
            var identifiers =
                GetIdentifiersForCondition(GetIdentifiers(family), family.ValidationTemplate);
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


        private void ApplyConditionalProperty<T>(IEnumerable<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            if (!Regex.IsMatch(_debugger.CurrentStackFrame.FunctionName,
                conditionalProperty.Condition.FunctionNameRegex))
            {
                return;
            }

            identifiers.Where(id =>
                    CheckConditionForIdentifier(conditionalProperty.Condition.Template, id))
                .ToList().ForEach(id => conditionalProperty.Properties.ForEach(p => applyProperty(p, id)));
        }

        private void ApplyPropertyForAllStackFrames<T>(IReadOnlyCollection<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var currentStackFrame = _debugger.CurrentStackFrame;
            var stackFrames = _debugger.CurrentThread.StackFrames;
            //Debug.WriteLine("\n\nStack frames section");
            foreach (StackFrame stackFrame in stackFrames)
            {
                if (!Regex.IsMatch(stackFrame.FunctionName, conditionalProperty.Condition.FunctionNameRegex))
                {
                    continue;
                }

                SetStackFrame(stackFrame);
                ApplyConditionalProperty(identifiers,
                    conditionalProperty, applyProperty);
            }

            SetStackFrame(currentStackFrame);
        }

        private List<Identifier> GetIdentifiers<T>(GraphElementFamily<T> family)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var ranges = new List<IdentifierPartRange>();
            foreach (var partTemplate in family.Ranges)
            {
                var beginString = GetExpression(partTemplate.BeginTemplate, null).Value;
                var endString = GetExpression(partTemplate.EndTemplate, null).Value;
                if (Int32.TryParse(beginString, out var begin) &&
                    Int32.TryParse(endString, out var end))
                {
                    ranges.Add(new IdentifierPartRange(partTemplate.Name, begin, end));
                }
                else
                {
                    //TODO change return type to Option, or throw exception
                    return null;
                }
            }

            return Identifier.GetAllIdentifiersInRange(family.Name, ranges);
        }

        private List<Identifier> GetIdentifiersForCondition(List<Identifier> identifiers, string conditionTemplate)
        {
            return conditionTemplate == null
                ? identifiers
                : identifiers.Where(id => CheckConditionForIdentifier(conditionTemplate, id)).ToList();
        }

        public static string Substitute(string expression, Identifier identifier, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var stackFrame = debugger.CurrentStackFrame;
            var result = expression.Replace("__CURRENT_FUNCTION__", stackFrame.FunctionName);
            for (int i = 1; i <= stackFrame.Arguments.Count; i++)
            {
                result = result.Replace($"__ARG{i}__", stackFrame.Arguments.Item(i).Value);
            }

            return identifier.Substitute(result);
        }


        private Expression GetExpression(string template, Identifier identifier)
        {
            string expression = identifier == null
                ? template
                : Substitute(template,
                    identifier, _debugger);
            //Debug.WriteLine($"{expression}");
            ThreadHelper.ThrowIfNotOnUIThread();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = _debugger.GetExpression(expression);
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            // Debug.WriteLine($"get expression {_getExpressionCallsNumber} in {elapsedTime}");
            _getExpressionCallsNumber++;
            _getExpressionTimeSpan += ts;
            return result;
        }


        private bool CheckConditionForIdentifier(string conditionTemplate, Identifier identifier)
        {
            var conditionResult = GetExpression(conditionTemplate, identifier);
            return conditionResult.IsValidValue && conditionResult.Value.Equals("true");
        }


        private Identifier NodeIdentifier(EdgeFamily.EdgeEnd edgeEnd, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var templates = edgeEnd.NamesWithTemplates;
            var res = new List<IdentifierPart>();
            foreach (var template in templates)
            {
                //TODO safe
                var debuggerRes = GetExpression(template.Value, identifier).Value;
                //Debug.WriteLine($"should be int {debuggerRes}");
                var value = Int32.Parse(debuggerRes);
                res.Add(new IdentifierPart(template.Key, value));
            }

            return new Identifier(edgeEnd.NodeFamilyName, res);
        }

        private void SetStackFrame(StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                _debugger.CurrentStackFrame = stackFrame;
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
                // Debug.WriteLine(
                //    $"set stackframe {_setCurrentStackFrameNumber} {stackFrame.FunctionName} in {elapsedTime}");
                _setCurrentStackFrameTimeSpan += ts;
                _setCurrentStackFrameNumber++;
            }
            catch (Exception)
            {
                Debug.WriteLine("Caught exception");
            }
        }
    }
}