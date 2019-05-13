using System;
using System.Text.RegularExpressions;
using EnvDTE;
using GraphAlgorithmRenderer.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using static GraphAlgorithmRenderer.GraphRenderer.DebuggerOperations;

namespace GraphAlgorithmRenderer.Config
{
    public abstract class AbstractLabelProperty
    {
        protected AbstractLabelProperty(string labelTextExpression)
        {
            LabelTextExpression = labelTextExpression;
        }

        public bool HighlightIfChanged { get; set; }
        public Color? ColorToHighLight { get; set; }
        [JsonProperty] public string LabelTextExpression { get; }
        public double? FontSize { get; set; }

        public void ApplyLabel(ILabeledObject graphElement, Debugger debugger, Identifier identifier)
        {
           
            var label = Regex.Replace(LabelTextExpression, @"{.*?}", delegate(Match match)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                string v = match.ToString();
                var expression = Substitute(v.Substring(1, v.Length - 2), identifier, CurrentStackFrame(debugger));
                return GetExpressionForIdentifier(expression, identifier, debugger).Value;
            });

            if (FontSize.HasValue)
            {
                graphElement.Label.FontSize = FontSize.Value;
            }

            if (label.Equals(graphElement.Label.Text))
            {
                return;
            }

            graphElement.Label.Text = label;
            if (HighlightIfChanged)
            {
                graphElement.Label.FontColor = ColorToHighLight ?? Color.Red;
            }
        }
    }
}