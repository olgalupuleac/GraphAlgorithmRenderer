using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;
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
        private static TimeSpan _getExpressionTimeSpan;
        private static TimeSpan _setCurrentStackFrameTimeSpan;
        private static int _getExpressionCallsNumber;
        private static int _setCurrentStackFrameNumber;
        private readonly OutputWindowPane _outputWindowPane;


        public GraphRenderer(GraphAlgorithmRenderer.Config.GraphConfig config, Debugger debugger,
            OutputWindowPane outputWindowPane)
        {
            _outputWindowPane = outputWindowPane;
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
            Dispatcher.CurrentDispatcher.VerifyAccess();
            var source = NodeIdentifier(edgeFamily.Source, identifier);
            var target = NodeIdentifier(edgeFamily.Target, identifier);
            if (source == null)
            {
                _outputWindowPane.OutputString("Cannot identify source node");

                return;
            }

            if (target == null)
            {
                _outputWindowPane.OutputString("Cannot identify target node");
                return;
            }

            var sourceNode = _graph.FindNode(source.Id());
            var targetNode = _graph.FindNode(target.Id());
            if (targetNode == null)
            {
                _outputWindowPane.OutputString($"Target node {target.Id()} does not exist");
                return;
            }

            if (sourceNode == null)
            {
                _outputWindowPane.OutputString($"Source node {source.Id()} does not exist");
            }

            var edge = _graph.AddEdge(source.Id(), target.Id());
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

        private void ApplyPropertyStackFrame<T>(IEnumerable<Identifier> identifiers, string template,
            List<T> properties,
            ApplyProperty<T> applyProperty)
        {   
            identifiers.Where(id =>
                    CheckConditionForIdentifier(template, id))
                .ToList().ForEach(id => properties.ForEach(p => applyProperty(p, id)));
        }

        private void ApplyPropertyCurrentStackFrame<T>(IEnumerable<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!Regex.IsMatch(_debugger.CurrentStackFrame.FunctionName,
                conditionalProperty.Condition.WrappedRegex()))
            {
                return;
            }
            ApplyPropertyStackFrame(identifiers, conditionalProperty.Condition.Template,
                conditionalProperty.Properties, applyProperty);
        }

        private void ApplyPropertyAllStackFrames<T>(IReadOnlyCollection<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var currentStackFrame = _debugger.CurrentStackFrame;
            var stackFrames = _debugger.CurrentThread.StackFrames;
            //Debug.WriteLine("\n\nStack frames section");
            foreach (StackFrame stackFrame in stackFrames)
            {
                if (!Regex.IsMatch(stackFrame.FunctionName, conditionalProperty.Condition.WrappedRegex()))
                {
                    continue;
                }

                SetStackFrame(stackFrame);
                ApplyPropertyStackFrame(identifiers,
                    conditionalProperty.Condition.Template, conditionalProperty.Properties, applyProperty);
            }

            SetStackFrame(currentStackFrame);
        }


        private void ApplyPropertyAllStackFramesArgsOnly<T>(IReadOnlyCollection<Identifier> identifiers,
            ConditionalProperty<T> conditionalProperty,
            ApplyProperty<T> applyProperty)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var currentStackFrame = _debugger.CurrentStackFrame;
            var stackFrames = _debugger.CurrentThread.StackFrames;
            //Debug.WriteLine("\n\nStack frames section");
            foreach (StackFrame stackFrame in stackFrames)
            {
                if (!Regex.IsMatch(stackFrame.FunctionName, conditionalProperty.Condition.WrappedRegex()))
                {
                    continue;
                }

                foreach (var id in identifiers)
                {
                    var expressionString = Substitute(conditionalProperty.Condition.Template, id, stackFrame);
                    var expression = GetExpression(expressionString, _debugger);
                    if (CheckExpression(expression))
                    {
                        conditionalProperty.Properties.ForEach(p => applyProperty(p, id));
                    }
                }
                
            }

            SetStackFrame(currentStackFrame);
        }


        private List<Identifier> GetIdentifiers<T>(GraphElementFamily<T> family)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var ranges = new List<IdentifierPartRange>();
            foreach (var partTemplate in family.Ranges)
            {
                var beginExpr = GetExpression(partTemplate.BeginTemplate, null, _debugger);
                if (!beginExpr.IsValidValue)
                {
                    _outputWindowPane.OutputString(
                        $"Begin template for {partTemplate.Name} {partTemplate.BeginTemplate} is not a valid value: {beginExpr.Value}");
                    continue;
                }

                var beginString = beginExpr.Value;
                var endExpr = GetExpression(partTemplate.EndTemplate, null, _debugger);

                if (!endExpr.IsValidValue)
                {
                    _outputWindowPane.OutputString(
                        $"End template for {partTemplate.Name} {partTemplate.EndTemplate} is not a valid value: {endExpr.Value}");
                    continue;
                }

                var endString = endExpr.Value;
                var beginParseResult = Int32.TryParse(beginString, out var begin);
                var endParseResult = Int32.TryParse(endString, out var end);
                if (beginParseResult && endParseResult)
                {
                    ranges.Add(new IdentifierPartRange(partTemplate.Name, begin, end));
                }
                else
                {
                    _outputWindowPane.OutputString(
                        $"Cannot parse {partTemplate.Name} ranges: {partTemplate.BeginTemplate} = {beginString}, {partTemplate.EndTemplate} = {endString}");
                }
            }

            return Identifier.GetAllIdentifiersInRange(family.Name, ranges);
        }

        private List<Identifier> GetIdentifiersForCondition(List<Identifier> identifiers, string conditionTemplate)
        {
            return String.IsNullOrEmpty(conditionTemplate)
                ? identifiers
                : identifiers.Where(id => CheckConditionForIdentifier(conditionTemplate, id)).ToList();
        }

        public static string Substitute(string expression, Identifier identifier, StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var result = expression.Replace("__CURRENT_FUNCTION__", stackFrame.FunctionName);
            for (int i = 1; i <= stackFrame.Arguments.Count; i++)
            {
                if (result.IndexOf($"__ARG{i}__", StringComparison.Ordinal) == -1)
                {
                    continue;
                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                result = result.Replace($"__ARG{i}__", stackFrame.Arguments.Item(i).Value);
                var ts = stopwatch.Elapsed;
                _getExpressionCallsNumber++;
                _getExpressionTimeSpan += ts;
            }

            return identifier.Substitute(result);
        }


        public static Expression GetExpression(string template, Identifier identifier, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
          
            string expression = identifier == null
                ? template
                : Substitute(template,
                    identifier, debugger.CurrentStackFrame);     
            //Debug.WriteLine($"{expression}");
            return GetExpression(expression, debugger);
        }


        private static Expression GetExpression(string expression, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = debugger.GetExpression(expression);
            TimeSpan ts = stopWatch.Elapsed;
            _getExpressionCallsNumber++;
            _getExpressionTimeSpan += ts;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            // Debug.WriteLine($"get expression {_getExpressionCallsNumber} in {elapsedTime}");
            return result;
        }

        private bool CheckConditionForIdentifier(string conditionTemplate, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var conditionResult = GetExpression(conditionTemplate, identifier, _debugger);
            return CheckExpression(conditionResult);
        }

        private bool CheckExpression(Expression expression)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (expression.IsValidValue)
            {
                return expression.Value.Equals("true");
            }
            _outputWindowPane.OutputString($"Expression is invalid. Expected true or false, found {expression.Value}");
            return false;
        }

        private Identifier NodeIdentifier(EdgeFamily.EdgeEnd edgeEnd, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var templates = edgeEnd.NamesWithTemplates;
            var res = new List<IdentifierPart>();
            foreach (var template in templates)
            {
                var debuggerResExpr = GetExpression(template.Value, identifier, _debugger);

                if (!debuggerResExpr.IsValidValue)
                {
                    _outputWindowPane.OutputString(
                        $"Template {template.Value} is not a valid value:\n {debuggerResExpr.Value}");
                    return null;
                }

                var value = Int32.Parse(debuggerResExpr.Value);
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