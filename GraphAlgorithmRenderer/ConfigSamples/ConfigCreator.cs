using System.Collections.Generic;
using GraphAlgorithmRenderer.Config;
using GraphConfig.Config;
using Microsoft.Msagl.Drawing;

namespace GraphAlgorithmRenderer.ConfigSamples
{
    public class ConfigCreator
    {
        public static Config.GraphConfig DfsConfig
        {
            get
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
                return new Config.GraphConfig
                {
                    Edges = new HashSet<EdgeFamily> { edges },
                    Nodes = new HashSet<NodeFamily> { nodes }
                };
            }
        }
    }
}