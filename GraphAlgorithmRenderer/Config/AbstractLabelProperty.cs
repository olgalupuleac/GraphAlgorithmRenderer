using System;
using System.Text.RegularExpressions;
using EnvDTE;
using GraphAlgorithmRenderer.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;

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
        public double FontSize { get; set; }

        public void ApplyLabel(ILabeledObject graphElement, Debugger debugger, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var expression = global::GraphAlgorithmRenderer.GraphRenderer.GraphRenderer.Substitute(LabelTextExpression, identifier, debugger);
            var label = Regex.Replace(expression, @"{.*?}", delegate (Match match)
            {
                string v = match.ToString();
                v = v.Substring(1, v.Length - 2);
                //Debug.WriteLine(v);
                return debugger.GetExpression(v).Value;
            });

            if (Math.Abs(FontSize) > 0.01)
            {
                graphElement.Label.FontSize = FontSize;
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