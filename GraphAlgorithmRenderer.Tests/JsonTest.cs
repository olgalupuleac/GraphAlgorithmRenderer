// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using GraphAlgorithmRendererLib.Config;
using GraphAlgorithmRendererLib.Serializer;
using Microsoft.Msagl.Drawing;
using NUnit.Framework;

namespace GraphAlgorithmRenderer.Tests
{
    [TestFixture]
    public class JsonSerializerTestClass
    {
        [Test]
        [TestCaseSource(typeof(ConfigSamples), nameof(ConfigSamples.TestCasesSerialize))]
        public string TestSerialize(GraphConfig config)
        {
            return ConfigSerializer.ToJson(config);
        }

        [Test]
        [TestCaseSource(typeof(ConfigSamples), nameof(ConfigSamples.TestCasesDeserialize))]
        public void TestDeserialize(string json)
        {
            var config = ConfigSerializer.FromJson(json);
            Assert.AreEqual(json, ConfigSerializer.ToJson(config));
        }
    }

    public class ConfigSamples
    {
        public static IEnumerable<TestCaseData> TestCasesSerialize
        {
            get
            {
                yield return new TestCaseData(ConfigAllProperties).Returns(JsonAllProperties);
                yield return new TestCaseData(DfsConfig).Returns(JsonDfs);
                yield return new TestCaseData(BridgesConfig).Returns(JsonBridges);
                yield return new TestCaseData(DsuConfig).Returns(JsonDsu);
                yield return new TestCaseData(FlowConfig).Returns(JsonFlow);
                yield return new TestCaseData(TreapConfig).Returns(JsonTreap);
            }
        }

        public static IEnumerable<TestCaseData> TestCasesDeserialize
        {
            get
            {
                yield return new TestCaseData(JsonAllProperties);
                yield return new TestCaseData(JsonDfs);
                yield return new TestCaseData(JsonBridges);
                yield return new TestCaseData(JsonDsu);
                yield return new TestCaseData(JsonFlow);
                yield return new TestCaseData(JsonTreap);
            }
        }

        public static string JsonAllProperties => @"{
  ""Nodes"": [
    {
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""v"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""node label"",
              ""FontSize"": null
            },
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 128,
                ""G"": 128,
                ""B"": 128
              }
            },
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 0,
                ""G"": 0,
                ""B"": 0
              }
            },
            {
              ""Type"": ""Shape"",
              ""Shape"": ""Box""
            },
            {
              ""Type"": ""Style"",
              ""Style"": ""Bold""
            },
            {
              ""Type"": ""LineWidth"",
              ""LineWidth"": 2.9
            }
          ]
        }
      ]
    }
  ],
  ""Edges"": [
    {
      ""Source"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""__a__""
        }
      },
      ""Target"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""g[__a__][__x__]""
        }
      },
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""a"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        },
        {
          ""Name"": ""x"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        }
      ],
      ""ValidationTemplate"": ""__x__ < g[__a__].size()"",
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""p[g[__a__][__x__]].first == __a__ && p[g[__a__][__x__]].second == __x__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""edge label"",
              ""FontSize"": 6.0
            },
            {
              ""Type"": ""Arrow"",
              ""ArrowAtTarget"": true,
              ""ArrowAtSource"": false
            },
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            },
            {
              ""Type"": ""Style"",
              ""Style"": ""Dashed""
            },
            {
              ""Type"": ""LineWidth"",
              ""LineWidth"": 1.2
            }
          ]
        }
      ]
    }
  ]
}";

        public static GraphConfig ConfigAllProperties
        {
            get
            {
                NodeFamily nodes = new NodeFamily(
                    new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("v", "0", "n")
                    }
                );

                EdgeFamily edges = new EdgeFamily(
                        new List<IdentifierPartTemplate>
                        {
                            new IdentifierPartTemplate("a", "0", "n"),
                            new IdentifierPartTemplate("x", "0", "n")
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> {"__a__"}),
                        new EdgeFamily.EdgeEnd(nodes, new List<string> {"g[__a__][__x__]"})
                    )
                    {ValidationTemplate = "__x__ < g[__a__].size()"};

                var edgeProperties = new ConditionalProperty<IEdgeProperty>(
                    new Condition("p[g[__a__][__x__]].first == __a__ && p[g[__a__][__x__]].second == __x__"),
                    new List<IEdgeProperty>
                    {
                        new LabelEdgeProperty("edge label"),
                        new ArrowEdgeProperty(true, false),
                        new LineColorEdgeProperty(Color.Red),
                        new StyleEdgeProperty(Style.Dashed),
                        new LineWidthEdgeProperty(1.2)
                    });
                var nodeProperties = new ConditionalProperty<INodeProperty>(
                    new Condition("true"),
                    new List<INodeProperty>
                    {
                        new LabelNodeProperty("node label"),
                        new FillColorNodeProperty(Color.Gray),
                        new LineColorNodeProperty(Color.Black),
                        new ShapeNodeProperty(Shape.Box),
                        new StyleNodeProperty(Style.Bold),
                        new LineWidthNodeProperty(2.9)
                    }
                );
                edges.ConditionalProperties.Add(edgeProperties);
                nodes.ConditionalProperties.Add(nodeProperties);
                var config = new GraphConfig
                {
                    Edges = new List<EdgeFamily> {edges},
                    Nodes = new List<NodeFamily> {nodes}
                };

                return config;
            }
        }

        public static string JsonDfs => @"{
  ""Nodes"": [
    {
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""v"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""{__v__}"",
              ""FontSize"": null
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__ARG1__ == __v__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""^dfs$""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__ARG1__ == __v__"",
            ""Mode"": ""AllStackFrames"",
            ""FunctionNameRegex"": ""^dfs$""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 128,
                ""G"": 128,
                ""B"": 128
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""visited[__v__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""^dfs$""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 0,
                ""G"": 128,
                ""B"": 0
              }
            }
          ]
        }
      ]
    }
  ],
  ""Edges"": [
    {
      ""Source"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""__a__""
        }
      },
      ""Target"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""g[__a__][__x__]""
        }
      },
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""a"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        },
        {
          ""Name"": ""x"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        }
      ],
      ""ValidationTemplate"": ""__x__ < g[__a__].size()"",
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""p[g[__a__][__x__]].first == __a__ && p[g[__a__][__x__]].second == __x__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Arrow"",
              ""ArrowAtTarget"": true,
              ""ArrowAtSource"": false
            }
          ]
        }
      ]
    }
  ]
}";

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
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> {"__a__"}),
                        new EdgeFamily.EdgeEnd(nodes, new List<string> {"g[__a__][__x__]"})
                    )
                    {ValidationTemplate = "__x__ < g[__a__].size()"};
                var directed = new ConditionalProperty<IEdgeProperty>(
                    new Condition("true"),
                    new ArrowEdgeProperty(true, false));
                var dfsEdges = new ConditionalProperty<IEdgeProperty>(
                    new Condition("p[g[__a__][__x__]].first == __a__ && p[g[__a__][__x__]].second == __x__"),
                    new LineColorEdgeProperty(Color.Red));
                edges.ConditionalProperties.Add(directed);
                edges.ConditionalProperties.Add(dfsEdges);
                nodes.ConditionalProperties.Reverse();
                edges.ConditionalProperties.Reverse();
                var config = new GraphConfig
                {
                    Edges = new List<EdgeFamily> {edges},
                    Nodes = new List<NodeFamily> {nodes}
                };

                return config;
            }
        }

        public static string JsonBridges => @"{
  ""Nodes"": [
    {
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""v"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""components.size() > 0 && components.back().count(__v__) > 0"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""new_comp""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 228,
                ""B"": 196
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__ARG1__ == __v__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""find_bridges""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__ARG1__ == __v__"",
            ""Mode"": ""AllStackFrames"",
            ""FunctionNameRegex"": ""find_bridges""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 128,
                ""G"": 128,
                ""B"": 128
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""{__v__}, tin={tin[__v__]}, up={up[__v__]}"",
              ""FontSize"": null
            }
          ]
        }
      ]
    }
  ],
  ""Edges"": [
    {
      ""Source"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""edges[__e__].from""
        }
      },
      ""Target"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""edges[__e__].to""
        }
      },
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""e"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""m""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""bridges_edge[__e__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""LineWidth"",
              ""LineWidth"": 3.0
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""bridges_edge[__e__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 138,
                ""G"": 43,
                ""B"": 226
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""dfs_edge[__e__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""find_bridges""
          },
          ""Properties"": [
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            }
          ]
        }
      ]
    }
  ]
}";

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
                        new List<string> {"edges[__e__].from"}), new EdgeFamily.EdgeEnd(nodes,
                        new List<string> {"edges[__e__].to"})
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
                    Nodes = new List<NodeFamily> {nodes},
                    Edges = new List<EdgeFamily> {edges}
                };
            }
        }

        public static string JsonDsu => @"{
  ""Nodes"": [
    {
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""v"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""{__v__}, rank={r[__v__]}"",
              ""FontSize"": null
            }
          ]
        }
      ]
    }
  ],
  ""Edges"": [
    {
      ""Source"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""__i__""
        }
      },
      ""Target"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""p[__i__]""
        }
      },
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""i"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Arrow"",
              ""ArrowAtTarget"": true,
              ""ArrowAtSource"": false
            }
          ]
        }
      ]
    }
  ]
}";

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
                    new ArrowEdgeProperty(true, false));
                var edges = new EdgeFamily(new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("i", "0", "n")
                    }, new EdgeFamily.EdgeEnd(nodes, new List<string> {"__i__"}),
                    new EdgeFamily.EdgeEnd(nodes, new List<string> {"p[__i__]"}));
                edges.ConditionalProperties.Add(directed);

                nodes.ConditionalProperties.Reverse();
                edges.ConditionalProperties.Reverse();
                var config = new GraphConfig()
                {
                    Edges = new List<EdgeFamily> {edges},
                    Nodes = new List<NodeFamily> {nodes}
                };

                return config;
            }
        }

        public static string JsonFlow => @"{
  ""Nodes"": [
    {
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""v"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""n""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""__ARG1__ == __v__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""dfs""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__ARG1__ == __v__"",
            ""Mode"": ""AllStackFrames"",
            ""FunctionNameRegex"": ""dfs""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 128,
                ""G"": 128,
                ""B"": 128
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__v__ == t"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""t = {__v__}, d={d[__v__]}"",
              ""FontSize"": null
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__v__ == s"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""s = {__v__}, d={d[__v__]}"",
              ""FontSize"": null
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""{__v__}, d={d[__v__]}"",
              ""FontSize"": null
            }
          ]
        }
      ]
    }
  ],
  ""Edges"": [
    {
      ""Source"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""edges[__e__].from""
        }
      },
      ""Target"": {
        ""FamilyName"": """",
        ""Templates"": {
          ""v"": ""edges[__e__].to""
        }
      },
      ""Name"": """",
      ""Ranges"": [
        {
          ""Name"": ""e"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""edges.size()""
        }
      ],
      ""ValidationTemplate"": ""__e__ < edges.size() && __e__ % 2 == 0"",
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""!flow_edges[__e__] && (edges[__e__].cap == edges[__e__].flow || edges[__e__].cap == -edges[__e__].flow)"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Style"",
              ""Style"": ""Dashed""
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""flow_edges[__e__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""dfs""
          },
          ""Properties"": [
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 138,
                ""G"": 43,
                ""B"": 226
              }
            },
            {
              ""Type"": ""Style"",
              ""Style"": ""Bold""
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""dfs_edges[__e__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""dfs""
          },
          ""Properties"": [
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""c={edges[__e__].cap}, f={edges[__e__].flow}"",
              ""FontSize"": 6.0
            }
          ]
        }
      ]
    }
  ]
}";

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
                    {ValidationTemplate = "__e__ < edges.size() && __e__ % 2 == 0"};
                var edgeLabel = new ConditionalProperty<IEdgeProperty>(
                    new Condition("true"),
                    new LabelEdgeProperty("c={edges[__e__].cap}, f={edges[__e__].flow}")
                        {FontSize = 6}
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
                    new Condition(
                        "!flow_edges[__e__] && (edges[__e__].cap == edges[__e__].flow || edges[__e__].cap == -edges[__e__].flow)"),
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
                    Edges = new List<EdgeFamily> {edges},
                    Nodes = new List<NodeFamily> {nodes}
                };
            }
        }

        public static string JsonTreap => @"{
  ""Nodes"": [
    {
      ""Name"": ""nodes"",
      ""Ranges"": [
        {
          ""Name"": ""v"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""nodes.size()""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""__ARG2__ == nodes[__v__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""(split)|(merge)""
          },
          ""Properties"": [
            {
              ""Type"": ""LineWidth"",
              ""LineWidth"": 6.0
            },
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 0,
                ""G"": 128,
                ""B"": 0
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__ARG3__ == nodes[__v__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""(split)|(merge)""
          },
          ""Properties"": [
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            },
            {
              ""Type"": ""LineWidth"",
              ""LineWidth"": 6.0
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__ARG1__ == nodes[__v__]"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""(split)|(merge)""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 127,
                ""G"": 255,
                ""B"": 212
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""__ARG1__ == nodes[__v__]"",
            ""Mode"": ""AllStackFrames"",
            ""FunctionNameRegex"": ""(split)|(merge)""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 128,
                ""G"": 128,
                ""B"": 128
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""{__v__}, k={nodes[__v__]->key}, p={nodes[__v__]->priority}"",
              ""FontSize"": null
            }
          ]
        }
      ]
    },
    {
      ""Name"": ""rightNull"",
      ""Ranges"": [
        {
          ""Name"": ""v"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""1""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""!__ARG1__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""(split)|(merge)""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 127,
                ""G"": 255,
                ""B"": 212
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""right null"",
              ""FontSize"": null
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""!__ARG3__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""^(split)|(merge)$""
          },
          ""Properties"": [
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 255,
                ""G"": 0,
                ""B"": 0
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""!__ARG3__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""^(split)|(merge)$""
          },
          ""Properties"": [
            {
              ""Type"": ""LineWidth"",
              ""LineWidth"": 6.0
            }
          ]
        }
      ]
    },
    {
      ""Name"": ""leftNull"",
      ""Ranges"": [
        {
          ""Name"": ""v"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""1""
        }
      ],
      ""ValidationTemplate"": null,
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""!__ARG1__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""(split)|(merge)""
          },
          ""Properties"": [
            {
              ""Type"": ""FillColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 127,
                ""G"": 255,
                ""B"": 212
              }
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""left null"",
              ""FontSize"": null
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""!__ARG2__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""^(split)|(merge)$""
          },
          ""Properties"": [
            {
              ""Type"": ""LineWidth"",
              ""LineWidth"": 6.0
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""!__ARG2__"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": ""^(split)|(merge)$""
          },
          ""Properties"": [
            {
              ""Type"": ""LineColor"",
              ""Color"": {
                ""A"": 255,
                ""R"": 0,
                ""G"": 128,
                ""B"": 0
              }
            }
          ]
        }
      ]
    }
  ],
  ""Edges"": [
    {
      ""Source"": {
        ""FamilyName"": ""nodes"",
        ""Templates"": {
          ""v"": ""__a__""
        }
      },
      ""Target"": {
        ""FamilyName"": ""nodes"",
        ""Templates"": {
          ""v"": ""__b__""
        }
      },
      ""Name"": ""rightEdge"",
      ""Ranges"": [
        {
          ""Name"": ""a"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""nodes.size()""
        },
        {
          ""Name"": ""b"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""nodes.size()""
        }
      ],
      ""ValidationTemplate"": ""nodes[__a__]->right == nodes[__b__]"",
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""right"",
              ""FontSize"": 6.0
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Arrow"",
              ""ArrowAtTarget"": true,
              ""ArrowAtSource"": false
            }
          ]
        }
      ]
    },
    {
      ""Source"": {
        ""FamilyName"": ""nodes"",
        ""Templates"": {
          ""v"": ""__i__""
        }
      },
      ""Target"": {
        ""FamilyName"": ""rightNull"",
        ""Templates"": {
          ""v"": ""0""
        }
      },
      ""Name"": ""rightNullEdge"",
      ""Ranges"": [
        {
          ""Name"": ""i"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""nodes.size()""
        }
      ],
      ""ValidationTemplate"": ""!nodes[__i__]->right"",
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""right"",
              ""FontSize"": 6.0
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Arrow"",
              ""ArrowAtTarget"": true,
              ""ArrowAtSource"": false
            }
          ]
        }
      ]
    },
    {
      ""Source"": {
        ""FamilyName"": ""nodes"",
        ""Templates"": {
          ""v"": ""__a__""
        }
      },
      ""Target"": {
        ""FamilyName"": ""nodes"",
        ""Templates"": {
          ""v"": ""__b__""
        }
      },
      ""Name"": ""leftEdge"",
      ""Ranges"": [
        {
          ""Name"": ""a"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""nodes.size()""
        },
        {
          ""Name"": ""b"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""nodes.size()""
        }
      ],
      ""ValidationTemplate"": ""nodes[__a__]->left == nodes[__b__]"",
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""left"",
              ""FontSize"": 6.0
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Arrow"",
              ""ArrowAtTarget"": true,
              ""ArrowAtSource"": false
            }
          ]
        }
      ]
    },
    {
      ""Source"": {
        ""FamilyName"": ""nodes"",
        ""Templates"": {
          ""v"": ""__i__""
        }
      },
      ""Target"": {
        ""FamilyName"": ""leftNull"",
        ""Templates"": {
          ""v"": ""0""
        }
      },
      ""Name"": ""leftNullEdge"",
      ""Ranges"": [
        {
          ""Name"": ""i"",
          ""BeginTemplate"": ""0"",
          ""EndTemplate"": ""nodes.size()""
        }
      ],
      ""ValidationTemplate"": ""!nodes[__i__]->left"",
      ""ConditionalProperties"": [
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Label"",
              ""LabelTextExpression"": ""left"",
              ""FontSize"": 6.0
            }
          ]
        },
        {
          ""Condition"": {
            ""Template"": ""true"",
            ""Mode"": ""CurrentStackFrame"",
            ""FunctionNameRegex"": "".*""
          },
          ""Properties"": [
            {
              ""Type"": ""Arrow"",
              ""ArrowAtTarget"": true,
              ""ArrowAtSource"": false
            }
          ]
        }
      ]
    }
  ]
}";

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
                    {Name = "nodes"};
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
                    new Condition("__ARG1__ == nodes[__v__]", @"(split)|(merge)"),
                    new FillColorNodeProperty(Color.Aquamarine)
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
                        new List<IdentifierPartTemplate> {new IdentifierPartTemplate("v", "0", "1")}
                    )
                    {Name = "leftNull"};
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
                    {Name = "rightNull"};

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
                    new ArrowEdgeProperty(true, false));
                var rightEdges = new EdgeFamily(
                        new List<IdentifierPartTemplate>
                        {
                            new IdentifierPartTemplate("a", "0", "nodes.size()"),
                            new IdentifierPartTemplate("b", "0", "nodes.size()")
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> {"__a__"}),
                        new EdgeFamily.EdgeEnd(nodes, new List<string> {"__b__"})
                    )
                    {ValidationTemplate = "nodes[__a__]->right == nodes[__b__]", Name = "rightEdge"};
                rightEdges.ConditionalProperties.Add(directed);
                var rightLabel = new ConditionalProperty<IEdgeProperty>(new Condition("true"),
                    new LabelEdgeProperty("right")
                );
                var leftEdges = new EdgeFamily(
                        new List<IdentifierPartTemplate>
                        {
                            new IdentifierPartTemplate("a", "0", "nodes.size()"),
                            new IdentifierPartTemplate("b", "0", "nodes.size()")
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> {"__a__"}),
                        new EdgeFamily.EdgeEnd(nodes, new List<string> {"__b__"})
                    )
                    {ValidationTemplate = "nodes[__a__]->left == nodes[__b__]", Name = "leftEdge"};
                leftEdges.ConditionalProperties.Add(directed);
                var leftLabel = new ConditionalProperty<IEdgeProperty>(new Condition("true"),
                    new LabelEdgeProperty("left")
                );
                leftEdges.ConditionalProperties.Add(leftLabel);
                rightEdges.ConditionalProperties.Add(rightLabel);

                var nullRightEdges = new EdgeFamily(new List<IdentifierPartTemplate>
                        {
                            new IdentifierPartTemplate("i", "0", "nodes.size()")
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> {"__i__"}),
                        new EdgeFamily.EdgeEnd(rightNull, new List<string> {"0"})
                    )
                    {ValidationTemplate = "!nodes[__i__]->right", Name = "rightNullEdge"};
                nullRightEdges.ConditionalProperties.Add(directed);
                nullRightEdges.ConditionalProperties.Add(rightLabel);

                var nullLeftEdges = new EdgeFamily(new List<IdentifierPartTemplate>
                        {
                            new IdentifierPartTemplate("i", "0", "nodes.size()")
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> {"__i__"}),
                        new EdgeFamily.EdgeEnd(leftNull, new List<string> {"0"})
                    )
                    {ValidationTemplate = "!nodes[__i__]->left", Name = "leftNullEdge"};
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

                return config;
            }
        }
    }
}