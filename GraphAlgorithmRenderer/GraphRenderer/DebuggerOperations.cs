using System;
using System.Diagnostics;
using EnvDTE;
using GraphAlgorithmRenderer.GraphElementIdentifier;
using Microsoft.VisualStudio.Shell;
using Debugger = EnvDTE.Debugger;
using StackFrame = EnvDTE.StackFrame;

namespace GraphAlgorithmRenderer.GraphRenderer
{
    public struct GetExpressionResult
    {
        public bool IsValid { get; set; }
        public string Value { get; set; }
    }

    public class DebuggerOperations
    {
        public static OutputWindowPane Log { get; set; }

        public static int NumberOfGetExpressionCalls { get; set; }
        public static TimeSpan TimeSpanGetExpressions { get; set; }

        public static GetExpressionResult GetExpression(string expression, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Stopwatch stopwatch = new Stopwatch();
            var res = debugger.GetExpression(expression);
            var isValid = res.IsValidValue;
            var value = res.Value;
            var ts = stopwatch.Elapsed;
            NumberOfGetExpressionCalls++;
            TimeSpanGetExpressions += ts;
            if (!isValid)
            {
                Log.OutputString($"Expression {expression} is not a valid value:\n{value}");
            }

            return new GetExpressionResult {IsValid = isValid, Value = value};
        }

        

        public static GetExpressionResult GetExpressionForIdentifier(string template, Identifier identifier, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return GetExpression(Substitute(template, identifier, debugger.CurrentStackFrame), debugger);
        }

        public static bool CheckExpression(string expression, Debugger debugger)
        {
            var res = GetExpression(expression, debugger);
            return res.IsValid && res.Value.Equals("true");
        }

        public static bool CheckExpressionForIdentifier(string template, Identifier identifier, Debugger debugger)
        {
            var res = GetExpressionForIdentifier(template, identifier, debugger);
            return res.IsValid && res.Value.Equals("true");
        }

        public static string Substitute(string template, Identifier identifier, StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var result = template.Replace("__CURRENT_FUNCTION__", stackFrame.FunctionName);
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
                NumberOfGetExpressionCalls++;
                TimeSpanGetExpressions += ts;
            }

            return identifier.Substitute(result);
        }
    }
}