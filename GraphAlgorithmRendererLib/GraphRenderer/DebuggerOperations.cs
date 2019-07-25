using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Debugger = EnvDTE.Debugger;
using StackFrame = EnvDTE.StackFrame;

namespace GraphAlgorithmRendererLib.GraphRenderer
{
    public struct GetExpressionResult
    {
        public bool IsValid { get; set; }
        public string Value { get; set; }
    }

    public class DebuggerOperations
    {
        private readonly OutputWindowPane _log;

        private int _numberOfGetExpressionCalls;
        private TimeSpan _timeSpanGetExpressions;

        private int _numberOfCurrentStackFrameCalls;
        private TimeSpan _timeSpanCurrentStackFrame;

        private int _numberOfFunctionNameCalls;
        private TimeSpan _timeSpanFunctionName;

        private int _numberOfSetStackFrameCalls;
        private TimeSpan _timeSpanSetStackFrame;

        private readonly Debugger _debugger;
        private StringBuilder _buffer = new StringBuilder();


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

        public DebuggerOperations(Debugger debugger, OutputWindowPane log)
        {
            _debugger = debugger;
            _log = log;
        }

        public GetExpressionResult GetExpression(string expression)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var res = MeasureTime(() =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var expr = _debugger.GetExpression(expression);
                return new GetExpressionResult {IsValid = expr.IsValidValue, Value = expr.Value};
            }, ref _timeSpanGetExpressions, ref _numberOfGetExpressionCalls);

            if (!res.IsValid)
            {
                _buffer.Append($"Expression {expression} is not a valid value:\n{res.Value}\n");
            }

            return res;
        }

        public GetExpressionResult GetExpressionForIdentifier(string template, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return GetExpression(Substitute(template, identifier, CurrentStackFrame()));
        }

        private bool CheckExpression(string expression)
        {
            var res = GetExpression("(bool)" + "(" + expression + ")");
            return res.IsValid && res.Value.Equals("true");
        }

        public List<Identifier> CheckExpression(string template, string functionRegex, IEnumerable<Identifier> identifiers)
        {
            return !Regex.IsMatch(FunctionName(CurrentStackFrame()), 
                functionRegex) ? new List<Identifier>() : identifiers.Where(id => CheckExpressionForIdentifier(template, id)).ToList();
        }

        public bool CheckExpressionForIdentifier(string template, Identifier identifier)
        {
            var res = GetExpressionForIdentifier(template, identifier);
            return res.IsValid && res.Value.Equals("true");
        }

        public List<Identifier> CheckExpressionAllStackFrames(string template, string functionRegex, IReadOnlyCollection<Identifier> identifiers)
        {
            var currentStackFrame = CurrentStackFrame();
            var res = new List<Identifier>();
            foreach (StackFrame stackFrame in _debugger.CurrentThread.StackFrames)
            {
                if (!Regex.IsMatch(FunctionName(stackFrame), functionRegex))
                {
                    continue;
                }
                SetStackFrame(stackFrame);
                res.AddRange(identifiers.Where(x => !res.Contains(x)).Where(id => CheckExpressionForIdentifier(template, id)));
            }
            SetStackFrame(currentStackFrame);
            return res;
        }

        public List<Identifier> CheckExpressionAllStackFramesArgsOnly(string template, string functionRegex, IReadOnlyCollection<Identifier> identifiers)
        {
            var res = new List<Identifier>();
            foreach (StackFrame stackFrame in _debugger.CurrentThread.StackFrames)
            {
                if (!Regex.IsMatch(FunctionName(stackFrame), functionRegex))
                {
                    continue;
                }

                res.AddRange(identifiers.Where(id => !res.Contains(id) && CheckExpression(Substitute(template, id, stackFrame))));
            }

            return res;
        }

        private string FunctionName(StackFrame stackFrame)
        {
            return MeasureTime(() =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return stackFrame.FunctionName;
            }, ref _timeSpanFunctionName, ref _numberOfFunctionNameCalls);
        }

        public void AddToLog(string log)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _log.OutputString(log);
        }

        private StackFrame CurrentStackFrame()
        {
            var res = MeasureTime(() =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return _debugger.CurrentStackFrame;
                }, ref _timeSpanCurrentStackFrame,
                ref _numberOfCurrentStackFrameCalls);
            return res;
        }

        private string Substitute(string template, Identifier identifier, StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var result = Regex.Replace(template, @"__ARG([0-9]*)__", delegate (Match match)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                string v = match.ToString();
                if (!Int32.TryParse(v.Substring(5, v.Length - 7), out var index))
                {
                    return v;
                }

                //TODO measure time
                var args = stackFrame.Arguments;
                if (args.Count < index)
                {
                    _buffer.AppendLine($"{v}: argument is out of bounds");
                    return v;
                }
                var expr = args.Item(index);
                var exprResult = new GetExpressionResult { IsValid = expr.IsValidValue, Value = expr.Value };
                if (!exprResult.IsValid)
                {
                    _buffer.Append($"Argument {v} is invalid:\n {exprResult.Value}\n");
                }
                return exprResult.Value;

            });
            
            if (result.IndexOf("__CURRENT_FUNCTION__", StringComparison.Ordinal) != -1)
            {
                result = result.Replace("__CURRENT_FUNCTION__", FunctionName(stackFrame));
            }

           

            return identifier.Substitute(result);
        }

        private void SetStackFrame(StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                MeasureTime(() =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    _debugger.CurrentStackFrame = stackFrame;
                    return 0;
                }, ref _timeSpanSetStackFrame, ref _numberOfSetStackFrameCalls);
            }
            catch (Exception)
            {
                Debug.WriteLine("Caught exception");
            }
        }

        private StackFrames GetStackFrames()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return _debugger.CurrentThread.StackFrames;
        }

        public bool IsActive => CurrentStackFrame() != null;
        

        public void WriteDebugOutput()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!String.IsNullOrWhiteSpace(_buffer.ToString()))
            {
                _log.OutputString(_buffer.ToString());
            }
            _buffer.Clear();
            Debug.WriteLine($"Got {_numberOfGetExpressionCalls} expressions in {_timeSpanGetExpressions}");
            Debug.WriteLine($"Set {_numberOfSetStackFrameCalls} stack frames in {_timeSpanSetStackFrame}");
            Debug.WriteLine(
                $"Get {_numberOfCurrentStackFrameCalls} current stack frames in {_timeSpanCurrentStackFrame}");
            Debug.WriteLine($"Get {_numberOfFunctionNameCalls} function names {_timeSpanFunctionName}");
        }

        public void ClearOutput()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _log.Clear();
        }

        public void Reset()
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