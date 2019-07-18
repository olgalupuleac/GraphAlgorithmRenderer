using System.Text.RegularExpressions;
using EnvDTE;
using GraphAlgorithmRendererLib.GraphRenderer;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;

namespace GraphAlgorithmRendererLib.Config
{
    public abstract class AbstractLabelProperty
    {
        protected AbstractLabelProperty(string labelTextExpression)
        {
            LabelTextExpression = labelTextExpression;
        }

        [JsonIgnore]
        public bool HighlightIfChanged { get; set; }
        [JsonIgnore]
        public Color? ColorToHighLight { get; set; }
        [JsonProperty] public string LabelTextExpression { get; }
        public double? FontSize { get; set; }

        public void ApplyLabel(ILabeledObject graphElement, DebuggerOperations debuggerOperations, Identifier identifier)
        {
           
            var label = Regex.Replace(LabelTextExpression, @"{.*?}", delegate(Match match)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                string v = match.ToString();
                return debuggerOperations.GetExpressionForIdentifier(v.Substring(1, v.Length - 2), identifier).Value;
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