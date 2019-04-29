using System.Collections.Generic;
using GraphConfig.Config;
using Microsoft.Msagl.Drawing;

namespace GraphAlgorithmRenderer
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using GraphConfig = GraphConfig.Config.GraphConfig;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("52a96019-a6a1-4390-a42a-d6f3b1e160cc")]
    public class SettingsWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        public SettingsWindow() : base(null)
        {
            this.Caption = "SettingsWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new SettingsWindowControl();
            _config = CreateGraphConfig();
        }

        private GraphConfig _config;

        public GraphConfig CreateGraphConfig()
        {
            var visitedNode =
                new ConditionalProperty<INodeProperty>(
                    new Condition("visited[__v__]", @"^dfs$"),
                    new FillColorNodeProperty(Color.Green));
            ;
            var dfsNode = new ConditionalProperty<INodeProperty>(
                new Condition("__ARG1__ == __v__", @"^dfs$",
                    ConditionMode.AllStackFrames), new FillColorNodeProperty(Color.Gray));

            var currentNode = new ConditionalProperty<INodeProperty>(
                new Condition("__ARG1__ == __v__", @"^dfs$"),
                new FillColorNodeProperty(Color.Red));

            NodeFamily nodes = new NodeFamily(
                new List<IdentifierPartTemplate>()
                {
                    new IdentifierPartTemplate("v", "0", "n")
                }
            );
            nodes.ConditionalProperties.Add(visitedNode);
            nodes.ConditionalProperties.Add(dfsNode);
            nodes.ConditionalProperties.Add(currentNode);

            EdgeFamily edges = new EdgeFamily(
                    new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("a", "0", "n"),
                        new IdentifierPartTemplate("x", "0", "n")
                    }, new EdgeFamily.EdgeEnd(nodes, new List<string> { "__a__" }),
                    new EdgeFamily.EdgeEnd(nodes, new List<string> { "g[__a__][__x__]" }), true
                )
                { ValidationTemplate = "__x__ < g[__a__].size()" };

            var dfsEdges = new ConditionalProperty<IEdgeProperty>(
                new Condition("p[g[__a__][__x__]].first == __a__ && p[g[__a__][__x__]].second == __x__"),
                new LineColorEdgeProperty(Color.Red));

            edges.ConditionalProperties.Add(dfsEdges);
            return new GraphConfig
            {
                Edges = new HashSet<EdgeFamily> { edges },
                Nodes = new HashSet<NodeFamily> { nodes }
            };
        }
    }
}
