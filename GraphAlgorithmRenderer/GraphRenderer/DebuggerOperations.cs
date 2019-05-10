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


        public static int NumberOfSetStackFrameCalls { get; set; }
        public static TimeSpan TimeSpanSetStackFrame { get; set; }

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

        public static string FunctionName(StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return stackFrame.FunctionName;
        }

        public static void AddToLog(string log)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Log.OutputString(log);
        }

        public static StackFrame CurrentStackFrame(Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return debugger.CurrentStackFrame;
        }

        public static string Substitute(string template, Identifier identifier, StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var result = template;
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

        public static void SetStackFrame(StackFrame stackFrame, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                debugger.CurrentStackFrame = stackFrame;
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                TimeSpanSetStackFrame += ts;
                NumberOfSetStackFrameCalls++;
            }
            catch (Exception)
            {
                Debug.WriteLine("Caught exception");
            }
        }

        public static StackFrames GetStackFrames(Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return debugger.CurrentThread.StackFrames;
        }

        public static void WriteDebugOutput()
        {
            var getExpressionTime =
                $"{TimeSpanGetExpressions.Hours:00}:{TimeSpanGetExpressions.Minutes:00}:{TimeSpanGetExpressions.Seconds:00}.{TimeSpanGetExpressions.Milliseconds / 10:00}";
            Debug.WriteLine($"Got {NumberOfGetExpressionCalls} expressions in {getExpressionTime}");

            var setStackFrameTime =
                $"{TimeSpanSetStackFrame.Hours:00}:{TimeSpanSetStackFrame.Minutes:00}:{TimeSpanSetStackFrame.Seconds:00}.{TimeSpanSetStackFrame.Milliseconds / 10:00}";
            Debug.WriteLine($"Set {NumberOfSetStackFrameCalls} stack frames in {setStackFrameTime}");
        }

        public static void Reset()
        {
            NumberOfGetExpressionCalls = 0;
            TimeSpanSetStackFrame = TimeSpan.Zero;
            NumberOfSetStackFrameCalls = 0;
            TimeSpanGetExpressions = TimeSpan.Zero;
        }
    }
}