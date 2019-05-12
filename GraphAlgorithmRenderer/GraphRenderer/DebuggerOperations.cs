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

        static int _numberOfGetExpressionCalls;
        static TimeSpan _timeSpanGetExpressions;

        static int _numberOfCurrentStackFrameCalls;
        static TimeSpan _timeSpanCurrentStackFrame;

        static int _numberOfFunctionNameCalls;
        static TimeSpan _timeSpanFunctionName;

        static int _numberOfSetStackFrameCalls;
        static TimeSpan _timeSpanSetStackFrame;


        private delegate T MakeAction<out T>();

        private static T MeasureTime<T>(MakeAction<T> makeAction, ref TimeSpan totalTime, ref int numberOfCalls)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var res = makeAction();
            var ts = stopwatch.Elapsed;
            totalTime += ts;
            numberOfCalls++;
            return res;
        }

        public static GetExpressionResult GetExpression(string expression, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var res = MeasureTime(() =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return debugger.GetExpression(expression);
            }, ref _timeSpanGetExpressions, ref _numberOfGetExpressionCalls);
            var isValid = res.IsValidValue;
            var value = res.Value;
            if (!isValid)
            {
                Log.OutputString($"Expression {expression} is not a valid value:\n{value}");
            }

            return new GetExpressionResult {IsValid = isValid, Value = value};
        }

        public static GetExpressionResult GetExpressionForIdentifier(string template, Identifier identifier,
            Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return GetExpression(Substitute(template, identifier, CurrentStackFrame(debugger)), debugger);
        }

        public static bool CheckExpression(string expression, Debugger debugger)
        {
            var res = GetExpression("(bool)" + "(" + expression + ")", debugger);
            return res.IsValid && res.Value.Equals("true");
        }

        public static bool CheckExpressionForIdentifier(string template, Identifier identifier, Debugger debugger)
        {
            var res = GetExpressionForIdentifier(template, identifier, debugger);
            return res.IsValid && res.Value.Equals("true");
        }

        public static string FunctionName(StackFrame stackFrame)
        {
            return MeasureTime(() =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return stackFrame.FunctionName;
            }, ref _timeSpanFunctionName, ref _numberOfFunctionNameCalls);
        }

        public static void AddToLog(string log)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Log.OutputString(log);
        }

        public static StackFrame CurrentStackFrame(Debugger debugger)
        {
            var res = MeasureTime(() =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return debugger.CurrentStackFrame;
                }, ref _timeSpanCurrentStackFrame,
                ref _numberOfCurrentStackFrameCalls);
            return res;
        }

        public static string Substitute(string template, Identifier identifier, StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            var result = template;
            if (result.IndexOf("__CURRENT_FUNCTION__", StringComparison.Ordinal) != -1)
            {
                result = result.Replace("__CURRENT_FUNCTION__", FunctionName(stackFrame));
            }
            for (int i = 1; i <= stackFrame.Arguments.Count; i++)
            {
                if (result.IndexOf($"__ARG{i}__", StringComparison.Ordinal) == -1)
                {
                    continue;
                }

                var i1 = i;
                var result1 = result;
                result = MeasureTime(() =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return result1.Replace($"__ARG{i1}__", stackFrame.Arguments.Item(i1).Value);
                }, ref _timeSpanGetExpressions, ref _numberOfGetExpressionCalls);
            }

            return identifier.Substitute(result);
        }

        public static void SetStackFrame(StackFrame stackFrame, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                MeasureTime(() =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    debugger.CurrentStackFrame = stackFrame;
                    return 0;
                }, ref _timeSpanSetStackFrame, ref _numberOfSetStackFrameCalls);
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
            ThreadHelper.ThrowIfNotOnUIThread();
            Debug.WriteLine($"Got {_numberOfGetExpressionCalls} expressions in {_timeSpanGetExpressions}");
            Debug.WriteLine($"Set {_numberOfSetStackFrameCalls} stack frames in {_timeSpanSetStackFrame}");
            Debug.WriteLine(
                $"Get {_numberOfCurrentStackFrameCalls} current stack frames in {_timeSpanCurrentStackFrame}");
            Debug.WriteLine($"Get {_numberOfFunctionNameCalls} function names {_timeSpanFunctionName}");
            
        }

        public static void Reset()
        {
            _numberOfGetExpressionCalls = 0;
            _timeSpanSetStackFrame = TimeSpan.Zero;

            _numberOfSetStackFrameCalls = 0;
            _timeSpanGetExpressions = TimeSpan.Zero;

            _numberOfCurrentStackFrameCalls = 0;
            _timeSpanCurrentStackFrame = TimeSpan.Zero;

            _numberOfFunctionNameCalls = 0;
            _timeSpanFunctionName = TimeSpan.Zero;
        }
    }
}