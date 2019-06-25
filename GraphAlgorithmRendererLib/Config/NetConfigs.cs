using System.Collections.Generic;
using System.Diagnostics;
using GraphAlgorithmRendererLib.Serializer;
using Microsoft.Msagl.Drawing;

namespace GraphAlgorithmRendererLib.Config
{
    public class ConfigCreator
    {
        public static GraphConfig DfsConfig
        {
            get
            {
                var visitedNode =
                    new ConditionalProperty<INodeProperty>(
                        new Condition("visited[__v__]", @"^dfs$"),
                        new FillColorNodeProperty(Color.Green));
                var dfsNode = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG1__ == __v__", @"^dfs$",
                        ConditionMode.AllStackFrames), new FillColorNodeProperty(Color.Gray));

                var currentNode = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG1__ == __v__", @"^dfs$"),
                    new FillColorNodeProperty(Color.Red));
                var label = new ConditionalProperty<INodeProperty>(new Condition("true"),
                    new LabelNodeProperty("{__v__}"));

                NodeFamily nodes = new NodeFamily(
                    new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("v", "0", "n")
                    }
                );
                nodes.ConditionalProperties.Add(visitedNode);
                nodes.ConditionalProperties.Add(dfsNode);
                nodes.ConditionalProperties.Add(currentNode);
                nodes.ConditionalProperties.Add(label);

                EdgeFamily edges = new EdgeFamily(
                        new List<IdentifierPartTemplate>
                        {
                            new IdentifierPartTemplate("a", "0", "n"),
                            new IdentifierPartTemplate("x", "0", "n")
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> { "__a__" }),
                        new EdgeFamily.EdgeEnd(nodes, new List<string> { "g[__a__][__x__]" })
                    )
                { ValidationTemplate = "__x__ < g[__a__].size()" };
                var directed = new ConditionalProperty<IEdgeProperty>(
                    new Condition("true"), 
                    new ArrowProperty(true, false));
                var dfsEdges = new ConditionalProperty<IEdgeProperty>(
                    new Condition("p[g[__a__][__x__]].first == __a__ && p[g[__a__][__x__]].second == __x__"),
                    new LineColorEdgeProperty(Color.Red));
                edges.ConditionalProperties.Add(directed);
                edges.ConditionalProperties.Add(dfsEdges);
                nodes.ConditionalProperties.Reverse();
                edges.ConditionalProperties.Reverse();
                var config =  new GraphConfig
                {
                    Edges = new List<EdgeFamily> { edges },
                    Nodes = new List<NodeFamily> { nodes }
                };
                
                return config;
            }
        }

        public static GraphConfig FlowConfig
        {
            get
            {
                NodeFamily nodes = new NodeFamily(new List<IdentifierPartTemplate>
                {
                    new IdentifierPartTemplate("v", "0", "n")
                });
                var nodeLabel = new ConditionalProperty<INodeProperty>(
                    new Condition("true"),
                    new LabelNodeProperty("{__v__}, d={d[__v__]}")
                );
                var sNodeLabel = new ConditionalProperty<INodeProperty>(
                    new Condition("__v__ == s"),
                    new LabelNodeProperty("s = {__v__}, d={d[__v__]}")
                );
                var tNodeLabel = new ConditionalProperty<INodeProperty>(
                    new Condition("__v__ == t"),
                    new LabelNodeProperty("t = {__v__}, d={d[__v__]}")
                );
                var dfsNodes = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG1__ == __v__", "dfs",
                        ConditionMode.AllStackFrames), new FillColorNodeProperty(Color.Gray));
                var currentDfsNode = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG1__ == __v__", "dfs"),
                    new FillColorNodeProperty(Color.Red));
                nodes.ConditionalProperties.Add(nodeLabel);
                nodes.ConditionalProperties.Add(sNodeLabel);
                nodes.ConditionalProperties.Add(tNodeLabel);
                nodes.ConditionalProperties.Add(dfsNodes);
                nodes.ConditionalProperties.Add(currentDfsNode);
                EdgeFamily edges = new EdgeFamily(new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("e", "0", "edges.size()")
                    },
                    new EdgeFamily.EdgeEnd(nodes, new List<string>
                    {
                        "edges[__e__].from"
                    }),
                    new EdgeFamily.EdgeEnd(nodes,
                        new List<string>
                        {
                            "edges[__e__].to"
                        })
                )
                { ValidationTemplate = "__e__ < edges.size() && __e__ % 2 == 0" };
                var edgeLabel = new ConditionalProperty<IEdgeProperty>(
                    new Condition("true"),
                    new LabelEdgeProperty("c={edges[__e__].cap}, f={edges[__e__].flow}")
                    { FontSize = 6 }
                );

                var dfsEdge = new ConditionalProperty<IEdgeProperty>(
                    new Condition("dfs_edges[__e__]", @"dfs"),
                    new LineColorEdgeProperty(Color.Red)
                );

                var flowEdge = new ConditionalProperty<IEdgeProperty>(
                    new Condition("flow_edges[__e__]", @"dfs"),
                    new List<IEdgeProperty>
                    {
                        new LineColorEdgeProperty(Color.BlueViolet),
                        new StyleEdgeProperty(Style.Bold)
                    }
                    
                );

                var fullEdge = new ConditionalProperty<IEdgeProperty>(
                    new Condition("!flow_edges[__e__] && (edges[__e__].cap == edges[__e__].flow || edges[__e__].cap == -edges[__e__].flow)"),
                    new StyleEdgeProperty(Style.Dashed)
                );

                edges.ConditionalProperties.Add(edgeLabel);
                edges.ConditionalProperties.Add(dfsEdge);
                edges.ConditionalProperties.Add(flowEdge);
                edges.ConditionalProperties.Add(fullEdge);

                nodes.ConditionalProperties.Reverse();
                edges.ConditionalProperties.Reverse();
                return new GraphConfig
                {
                    Edges = new List<EdgeFamily> { edges },
                    Nodes = new List<NodeFamily> { nodes }
                };
            }
        }

        public static GraphConfig BridgesConfig
        {
            get
            {
                var nodes = new NodeFamily(new List<IdentifierPartTemplate>
                {
                    new IdentifierPartTemplate("v", "0", "n")
                });
                var nodeLabel = new ConditionalProperty<INodeProperty>(
                    new Condition("true"), new LabelNodeProperty(
                        "{__v__}, tin={tin[__v__]}, up={up[__v__]}"));

                var dfsNode = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG1__ == __v__", @"find_bridges",
                        ConditionMode.AllStackFrames),
                    new FillColorNodeProperty(Color.Gray)
                );
                var curDfsNode = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG1__ == __v__", @"find_bridges"),
                    new FillColorNodeProperty(Color.Red)
                );
                var lastComponent = new ConditionalProperty<INodeProperty>(
                    new Condition("components.size() > 0 && components.back().count(__v__) > 0", @"new_comp"),
                    new FillColorNodeProperty(Color.Bisque)
                );
                nodes.ConditionalProperties.Add(nodeLabel);
                nodes.ConditionalProperties.Add(dfsNode);
                nodes.ConditionalProperties.Add(curDfsNode);
                nodes.ConditionalProperties.Add(lastComponent);
                var edges = new EdgeFamily(new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("e", "0", "m"),


                    }, new EdgeFamily.EdgeEnd(nodes,
                        new List<string> { "edges[__e__].from" }), new EdgeFamily.EdgeEnd(nodes,
                        new List<string> { "edges[__e__].to" })
                );

                var dfsEdges = new ConditionalProperty<IEdgeProperty>(new Condition("dfs_edge[__e__]",
                        @"find_bridges"),
                    new LineColorEdgeProperty(Color.Red));
                var isBridge = new ConditionalProperty<IEdgeProperty>(new Condition(
                        "bridges_edge[__e__]"
                        ), new LineColorEdgeProperty(Color.BlueViolet)
                    );
                var isBridgeLine = new ConditionalProperty<IEdgeProperty>(new Condition(
                        "bridges_edge[__e__]"
                    ), new LineWidthEdgeProperty(3)
                );
                edges.ConditionalProperties.Add(dfsEdges);
                edges.ConditionalProperties.Add(isBridge);
                edges.ConditionalProperties.Add(isBridgeLine);

                nodes.ConditionalProperties.Reverse();
                edges.ConditionalProperties.Reverse();
                return new GraphConfig
                {
                    Nodes = new List<NodeFamily> { nodes },
                    Edges = new List<EdgeFamily> { edges }
                };
            }
        }

        public static GraphConfig DsuConfig
        {
            get
            {
                var nodes = new NodeFamily(
                    new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("v", "0", "n")
                    }
                );
                var label = new ConditionalProperty<INodeProperty>(
                    new Condition("true"),
                    new LabelNodeProperty("{__v__}, rank={r[__v__]}")
                );
                nodes.ConditionalProperties.Add(label);
                var directed = new ConditionalProperty<IEdgeProperty>(
                    new Condition("true"),
                    new ArrowProperty(true, false));
                var edges = new EdgeFamily(new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("i", "0", "n")
                    }, new EdgeFamily.EdgeEnd(nodes, new List<string> { "__i__" }),
                    new EdgeFamily.EdgeEnd(nodes, new List<string> { "p[__i__]" }));
                edges.ConditionalProperties.Add(directed);

                nodes.ConditionalProperties.Reverse();
                edges.ConditionalProperties.Reverse();
                var config = new GraphConfig()
                {
                    Edges = new List<EdgeFamily> { edges },
                    Nodes = new List<NodeFamily> { nodes }
                };
                Debug.WriteLine("DSU");
                Debug.WriteLine(ConfigSerializer.ToJson(config));
                return config;
            }
        }


        public static GraphConfig TreapConfig
        {
            get
            {
                var nodes = new NodeFamily(
                    new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("v", "0", "nodes.size()")
                    }
                    )
                { Name = "nodes" };
                var label = new ConditionalProperty<INodeProperty>(
                    new Condition("true"),
                    new LabelNodeProperty("{__v__}, k={nodes[__v__]->key}, p={nodes[__v__]->priority}")
                );
                nodes.ConditionalProperties.Add(label);

                var tNode = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG1__ == nodes[__v__]", @"(split)|(merge)",
                        ConditionMode.AllStackFrames), new FillColorNodeProperty(Color.Gray)
                );

                var curTNode = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG1__ == nodes[__v__]", @"(split)|(merge)"), new FillColorNodeProperty(Color.Aquamarine)
                );


                var curLeftNodeLine = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG2__ == nodes[__v__]", @"(split)|(merge)"),
                    new List<INodeProperty>
                    {
                        new LineWidthNodeProperty(6),
                        new LineColorNodeProperty(Color.Green)
                    }
                );

                var curRightNode = new ConditionalProperty<INodeProperty>(
                    new Condition("__ARG3__ == nodes[__v__]", @"(split)|(merge)"), new List<INodeProperty>
                    {
                        new LineColorNodeProperty(Color.Red),
                        new LineWidthNodeProperty(6)
                    }
                );

                var curTNullNode = new ConditionalProperty<INodeProperty>(
                    new Condition("!__ARG1__", @"(split)|(merge)"), new FillColorNodeProperty(Color.Aquamarine)
                );



                nodes.ConditionalProperties.Add(tNode);
                nodes.ConditionalProperties.Add(curTNode);
                nodes.ConditionalProperties.Add(curRightNode);
                nodes.ConditionalProperties.Add(curLeftNodeLine);
                var leftNull = new NodeFamily(
                        new List<IdentifierPartTemplate> { new IdentifierPartTemplate("v", "0", "1") }
                    )
                { Name = "leftNull" };
                var nullLeftNodeLine = new ConditionalProperty<INodeProperty>(
                    new Condition("!__ARG2__", @"^(split)|(merge)$"), new LineWidthNodeProperty(6)
                );

                var nullLeftNode = new ConditionalProperty<INodeProperty>(
                    new Condition("!__ARG2__", @"^(split)|(merge)$"), new LineColorNodeProperty(Color.Green)
                );
                leftNull.ConditionalProperties.Add(nullLeftNode);
                leftNull.ConditionalProperties.Add(nullLeftNodeLine);
                var rightNullLabel = new ConditionalProperty<INodeProperty>(
                    new Condition("true"), new LabelNodeProperty("right null"));
                var leftNullLabel = new ConditionalProperty<INodeProperty>(
                    new Condition("true"), new LabelNodeProperty("left null"));
                var rightNull = new NodeFamily(new List<IdentifierPartTemplate>
                    {new IdentifierPartTemplate("v", "0", "1")})
                { Name = "rightNull" };

                var nullRightNode = new ConditionalProperty<INodeProperty>(
                    new Condition("!__ARG3__", @"^(split)|(merge)$"), new LineColorNodeProperty(Color.Red)
                );

                var nullRightNodeLine = new ConditionalProperty<INodeProperty>(
                    new Condition("!__ARG3__", @"^(split)|(merge)$"), new LineWidthNodeProperty(6)
                );
                rightNull.ConditionalProperties.Add(nullRightNodeLine);
                rightNull.ConditionalProperties.Add(nullRightNode);
                leftNull.ConditionalProperties.Add(leftNullLabel);
                rightNull.ConditionalProperties.Add(rightNullLabel);
                leftNull.ConditionalProperties.Add(curTNullNode);
                rightNull.ConditionalProperties.Add(curTNullNode);
                var directed = new ConditionalProperty<IEdgeProperty>(
                    new Condition("true"),
                    new ArrowProperty(true, false));
                var rightEdges = new EdgeFamily(
                        new List<IdentifierPartTemplate>
                        {
                            new IdentifierPartTemplate("a", "0", "nodes.size()"),
                            new IdentifierPartTemplate("b", "0", "nodes.size()")
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> { "__a__" }),
                        new EdgeFamily.EdgeEnd(nodes, new List<string> { "__b__" })
                    )
                { ValidationTemplate = "nodes[__a__]->right == nodes[__b__]", Name = "rightEdge" };
                rightEdges.ConditionalProperties.Add(directed);
                var rightLabel = new ConditionalProperty<IEdgeProperty>(new Condition("true"), new LabelEdgeProperty("right")
                    );
                var leftEdges = new EdgeFamily(
                    new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("a", "0", "nodes.size()"),
                        new IdentifierPartTemplate("b", "0", "nodes.size()")
                    }, new EdgeFamily.EdgeEnd(nodes, new List<string> { "__a__" }),
                    new EdgeFamily.EdgeEnd(nodes, new List<string> { "__b__" })
                    )
                { ValidationTemplate = "nodes[__a__]->left == nodes[__b__]", Name = "leftEdge" };
                leftEdges.ConditionalProperties.Add(directed);
                var leftLabel = new ConditionalProperty<IEdgeProperty>(new Condition("true"), new LabelEdgeProperty("left")
                );
                leftEdges.ConditionalProperties.Add(leftLabel);
                rightEdges.ConditionalProperties.Add(rightLabel);

                var nullRightEdges = new EdgeFamily(new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("i", "0", "nodes.size()")
                    }, new EdgeFamily.EdgeEnd(nodes, new List<string> { "__i__" }), new EdgeFamily.EdgeEnd(rightNull, new List<string> { "0" })

                    )
                { ValidationTemplate = "!nodes[__i__]->right", Name = "rightNullEdge" };
                nullRightEdges.ConditionalProperties.Add(directed);
                nullRightEdges.ConditionalProperties.Add(rightLabel);

                var nullLeftEdges = new EdgeFamily(new List<IdentifierPartTemplate>
                        {
                            new IdentifierPartTemplate("i", "0", "nodes.size()")
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> { "__i__" }),
                        new EdgeFamily.EdgeEnd(leftNull, new List<string> { "0" })

                    )
                { ValidationTemplate = "!nodes[__i__]->left", Name = "leftNullEdge" };
                nullLeftEdges.ConditionalProperties.Add(directed);
                nullLeftEdges.ConditionalProperties.Add(leftLabel);
                nodes.ConditionalProperties.Reverse();
                rightNull.ConditionalProperties.Reverse();
                leftNull.ConditionalProperties.Reverse();
                rightEdges.ConditionalProperties.Reverse();
                nullRightEdges.ConditionalProperties.Reverse();
                leftEdges.ConditionalProperties.Reverse();
                nullLeftEdges.ConditionalProperties.Reverse();
                
                var config = new GraphConfig
                {
                    Nodes = new List<NodeFamily>
                    {
                        nodes,
                        rightNull,
                        leftNull
                    },
                    Edges = new List<EdgeFamily>
                    {
                        rightEdges,
                        nullRightEdges,
                        leftEdges,
                        nullLeftEdges
                    }
                };
                Debug.WriteLine("\nTreap");
                Debug.WriteLine(ConfigSerializer.ToJson(config));
                return config;
            }
        }
    }
}