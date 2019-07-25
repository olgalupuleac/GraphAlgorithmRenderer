using System.Diagnostics;
using GraphAlgorithmRendererLib.Serializer;
using NUnit.Framework;

namespace GraphAlgorithmRenderer.Tests
{
    [TestFixture]
    public class JsonOldFormatTest
    {
        [Test]
        public void TestMethod()
        {
            ConfigSerializer.FromJson(OldFormatSamples.JsonNoProperties);
            var res = ConfigSerializer.FromJson(OldFormatSamples.JsonDfsOldFormat);
            ConfigSerializer.FromJson(OldFormatSamples.JsonOldFormatAllProperties);
            Debug.WriteLine(res);
        }
    }

    public class OldFormatSamples
    {
        public static string JsonNoProperties => @"{
  ""$type"": ""GraphAlgorithmRenderer.Config.GraphConfig, GraphAlgorithmRenderer"",
  ""Edges"": {
    ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.EdgeFamily, GraphAlgorithmRenderer]], mscorlib"",
    ""$values"": [
      {
        ""$type"": ""GraphAlgorithmRenderer.Config.EdgeFamily, GraphAlgorithmRenderer"",
        ""Source"": {
          ""$type"": ""GraphAlgorithmRenderer.Config.EdgeFamily+EdgeEnd, GraphAlgorithmRenderer"",
          ""NodeFamilyName"": ""node#0"",
          ""NamesWithTemplates"": {
            ""$type"": ""System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.String, mscorlib]], mscorlib"",
            ""v"": ""a""
          }
        },
        ""Target"": {
          ""$type"": ""GraphAlgorithmRenderer.Config.EdgeFamily+EdgeEnd, GraphAlgorithmRenderer"",
          ""NodeFamilyName"": ""node#0"",
          ""NamesWithTemplates"": {
            ""$type"": ""System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.String, mscorlib]], mscorlib"",
            ""v"": ""b""
          }
        },
        ""Name"": ""edge#0"",
        ""Ranges"": {
          ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer]], mscorlib"",
          ""$values"": [
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer"",
              ""Name"": ""e"",
              ""BeginTemplate"": ""0"",
              ""EndTemplate"": ""m""
            }
          ]
        },
        ""ValidationTemplate"": """",
        ""ConditionalProperties"": {
          ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer]], mscorlib"",
          ""$values"": []
        }
      }
    ]
  },
  ""Nodes"": {
    ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.NodeFamily, GraphAlgorithmRenderer]], mscorlib"",
    ""$values"": [
      {
        ""$type"": ""GraphAlgorithmRenderer.Config.NodeFamily, GraphAlgorithmRenderer"",
        ""Name"": ""node#0"",
        ""Ranges"": {
          ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer]], mscorlib"",
          ""$values"": [
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer"",
              ""Name"": ""v"",
              ""BeginTemplate"": ""0"",
              ""EndTemplate"": ""n""
            }
          ]
        },
        ""ValidationTemplate"": ""a"",
        ""ConditionalProperties"": {
          ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer]], mscorlib"",
          ""$values"": []
        }
      }
    ]
  }
}";
        public static string JsonDfsOldFormat => @"{
  ""$type"": ""GraphAlgorithmRenderer.Config.GraphConfig, GraphAlgorithmRenderer"",
  ""Edges"": {
    ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.EdgeFamily, GraphAlgorithmRenderer]], mscorlib"",
    ""$values"": [
      {
        ""$type"": ""GraphAlgorithmRenderer.Config.EdgeFamily, GraphAlgorithmRenderer"",
        ""Source"": {
          ""$type"": ""GraphAlgorithmRenderer.Config.EdgeFamily+EdgeEnd, GraphAlgorithmRenderer"",
          ""NodeFamilyName"": ""node#0"",
          ""NamesWithTemplates"": {
            ""$type"": ""System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.String, mscorlib]], mscorlib"",
            ""v"": ""__a__""
          }
        },
        ""Target"": {
          ""$type"": ""GraphAlgorithmRenderer.Config.EdgeFamily+EdgeEnd, GraphAlgorithmRenderer"",
          ""NodeFamilyName"": ""node#0"",
          ""NamesWithTemplates"": {
            ""$type"": ""System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.String, mscorlib]], mscorlib"",
            ""v"": ""g[__a__][__x__].to""
          }
        },
        ""Name"": ""edge#0"",
        ""Ranges"": {
          ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer]], mscorlib"",
          ""$values"": [
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer"",
              ""Name"": ""a"",
              ""BeginTemplate"": ""0"",
              ""EndTemplate"": ""n""
            },
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer"",
              ""Name"": ""x"",
              ""BeginTemplate"": ""0"",
              ""EndTemplate"": ""g[__a__].size()""
            }
          ]
        },
        ""ValidationTemplate"": ""__a__ <= g[__a__][__x__].to"",
        ""ConditionalProperties"": {
          ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer]], mscorlib"",
          ""$values"": [
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer"",
              ""Condition"": {
                ""$type"": ""GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer"",
                ""Template"": ""used_edges[g[__a__][__x__].id]"",
                ""Mode"": 0,
                ""FunctionNameRegex"": ""dfs""
              },
              ""Properties"": {
                ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], mscorlib"",
                ""$values"": [
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.LineWidthEdgeProperty, GraphAlgorithmRenderer"",
                    ""LineWidth"": 1.2000000476837158
                  },
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.StyleEdgeProperty, GraphAlgorithmRenderer"",
                    ""Style"": 1
                  }
                ]
              }
            },
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer"",
              ""Condition"": {
                ""$type"": ""GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer"",
                ""Template"": ""g[v][i].id == g[__a__][__x__].id"",
                ""Mode"": 0,
                ""FunctionNameRegex"": ""dfs""
              },
              ""Properties"": {
                ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], mscorlib"",
                ""$values"": [
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.LineColorEdgeProperty, GraphAlgorithmRenderer"",
                    ""Color"": {
                      ""$type"": ""Microsoft.Msagl.Drawing.Color, Microsoft.Msagl.Drawing"",
                      ""A"": 255,
                      ""R"": 255,
                      ""G"": 0,
                      ""B"": 255
                    }
                  },
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.LineWidthEdgeProperty, GraphAlgorithmRenderer"",
                    ""LineWidth"": 2.0
                  }
                ]
              }
            }
          ]
        }
      }
    ]
  },
  ""Nodes"": {
    ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.NodeFamily, GraphAlgorithmRenderer]], mscorlib"",
    ""$values"": [
      {
        ""$type"": ""GraphAlgorithmRenderer.Config.NodeFamily, GraphAlgorithmRenderer"",
        ""Name"": ""node#0"",
        ""Ranges"": {
          ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer]], mscorlib"",
          ""$values"": [
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer"",
              ""Name"": ""v"",
              ""BeginTemplate"": ""0"",
              ""EndTemplate"": ""n""
            }
          ]
        },
        ""ValidationTemplate"": """",
        ""ConditionalProperties"": {
          ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer]], mscorlib"",
          ""$values"": [
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer"",
              ""Condition"": {
                ""$type"": ""GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer"",
                ""Template"": ""vertex_component[__v__] != -1"",
                ""Mode"": 0,
                ""FunctionNameRegex"": "".*""
              },
              ""Properties"": {
                ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], mscorlib"",
                ""$values"": [
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.LabelNodeProperty, GraphAlgorithmRenderer"",
                    ""HighlightIfChanged"": false,
                    ""ColorToHighLight"": null,
                    ""LabelTextExpression"": ""{__v__ + 1}, comp={vertex_component[__v__]}, size={components_size[vertex_component[__v__]]}"",
                    ""FontSize"": null
                  }
                ]
              }
            },
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer"",
              ""Condition"": {
                ""$type"": ""GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer"",
                ""Template"": ""true"",
                ""Mode"": 0,
                ""FunctionNameRegex"": "".*""
              },
              ""Properties"": {
                ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], mscorlib"",
                ""$values"": [
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.LabelNodeProperty, GraphAlgorithmRenderer"",
                    ""HighlightIfChanged"": false,
                    ""ColorToHighLight"": null,
                    ""LabelTextExpression"": ""{__v__ + 1}"",
                    ""FontSize"": null
                  }
                ]
              }
            },
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer"",
              ""Condition"": {
                ""$type"": ""GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer"",
                ""Template"": ""__v__ == v"",
                ""Mode"": 0,
                ""FunctionNameRegex"": ""dfs""
              },
              ""Properties"": {
                ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], mscorlib"",
                ""$values"": [
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.FillColorNodeProperty, GraphAlgorithmRenderer"",
                    ""Color"": {
                      ""$type"": ""Microsoft.Msagl.Drawing.Color, Microsoft.Msagl.Drawing"",
                      ""A"": 255,
                      ""R"": 255,
                      ""G"": 0,
                      ""B"": 0
                    }
                  }
                ]
              }
            },
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer"",
              ""Condition"": {
                ""$type"": ""GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer"",
                ""Template"": ""__v__ == __ARG1__"",
                ""Mode"": 2,
                ""FunctionNameRegex"": ""dfs""
              },
              ""Properties"": {
                ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], mscorlib"",
                ""$values"": [
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.FillColorNodeProperty, GraphAlgorithmRenderer"",
                    ""Color"": {
                      ""$type"": ""Microsoft.Msagl.Drawing.Color, Microsoft.Msagl.Drawing"",
                      ""A"": 255,
                      ""R"": 169,
                      ""G"": 169,
                      ""B"": 169
                    }
                  }
                ]
              }
            },
            {
              ""$type"": ""GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer"",
              ""Condition"": {
                ""$type"": ""GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer"",
                ""Template"": ""used_vertexes[__v__]"",
                ""Mode"": 0,
                ""FunctionNameRegex"": "".*""
              },
              ""Properties"": {
                ""$type"": ""System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], mscorlib"",
                ""$values"": [
                  {
                    ""$type"": ""GraphAlgorithmRenderer.Config.FillColorNodeProperty, GraphAlgorithmRenderer"",
                    ""Color"": {
                      ""$type"": ""Microsoft.Msagl.Drawing.Color, Microsoft.Msagl.Drawing"",
                      ""A"": 255,
                      ""R"": 34,
                      ""G"": 139,
                      ""B"": 34
                    }
                  }
                ]
              }
            }
          ]
        }
      }
    ]
  }
}
";

        public static string JsonOldFormatAllProperties =>
            "{\r\n  \"$type\": \"GraphAlgorithmRenderer.Config.GraphConfig, GraphAlgorithmRenderer\",\r\n  \"Edges\": {\r\n    \"$type\": \"System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.EdgeFamily, GraphAlgorithmRenderer]], mscorlib\",\r\n    \"$values\": [\r\n      {\r\n        \"$type\": \"GraphAlgorithmRenderer.Config.EdgeFamily, GraphAlgorithmRenderer\",\r\n        \"Source\": {\r\n          \"$type\": \"GraphAlgorithmRenderer.Config.EdgeFamily+EdgeEnd, GraphAlgorithmRenderer\",\r\n          \"NodeFamilyName\": \"node#0\",\r\n          \"NamesWithTemplates\": {\r\n            \"$type\": \"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.String, mscorlib]], mscorlib\",\r\n            \"v\": \"a\"\r\n          }\r\n        },\r\n        \"Target\": {\r\n          \"$type\": \"GraphAlgorithmRenderer.Config.EdgeFamily+EdgeEnd, GraphAlgorithmRenderer\",\r\n          \"NodeFamilyName\": \"node#0\",\r\n          \"NamesWithTemplates\": {\r\n            \"$type\": \"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.String, mscorlib]], mscorlib\",\r\n            \"v\": \"b\"\r\n          }\r\n        },\r\n        \"Name\": \"edge#0\",\r\n        \"Ranges\": {\r\n          \"$type\": \"System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer]], mscorlib\",\r\n          \"$values\": [\r\n            {\r\n              \"$type\": \"GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer\",\r\n              \"Name\": \"e\",\r\n              \"BeginTemplate\": \"0\",\r\n              \"EndTemplate\": \"m\"\r\n            }\r\n          ]\r\n        },\r\n        \"ValidationTemplate\": \"\",\r\n        \"ConditionalProperties\": {\r\n          \"$type\": \"System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer]], mscorlib\",\r\n          \"$values\": [\r\n            {\r\n              \"$type\": \"GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer\",\r\n              \"Condition\": {\r\n                \"$type\": \"GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer\",\r\n                \"Template\": \"true\",\r\n                \"Mode\": 0,\r\n                \"FunctionNameRegex\": \".*\"\r\n              },\r\n              \"Properties\": {\r\n                \"$type\": \"System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IEdgeProperty, GraphAlgorithmRenderer]], mscorlib\",\r\n                \"$values\": [\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.LabelEdgeProperty, GraphAlgorithmRenderer\",\r\n                    \"HighlightIfChanged\": false,\r\n                    \"ColorToHighLight\": null,\r\n                    \"LabelTextExpression\": \"s\",\r\n                    \"FontSize\": 6.0\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.LineColorEdgeProperty, GraphAlgorithmRenderer\",\r\n                    \"Color\": {\r\n                      \"$type\": \"Microsoft.Msagl.Drawing.Color, Microsoft.Msagl.Drawing\",\r\n                      \"A\": 255,\r\n                      \"R\": 0,\r\n                      \"G\": 206,\r\n                      \"B\": 209\r\n                    }\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.LineWidthEdgeProperty, GraphAlgorithmRenderer\",\r\n                    \"LineWidth\": 2.0\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.StyleEdgeProperty, GraphAlgorithmRenderer\",\r\n                    \"Style\": 7\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.ArrowProperty, GraphAlgorithmRenderer\",\r\n                    \"ArrowAtTarget\": true,\r\n                    \"ArrowAtSource\": false\r\n                  }\r\n                ]\r\n              }\r\n            }\r\n          ]\r\n        }\r\n      }\r\n    ]\r\n  },\r\n  \"Nodes\": {\r\n    \"$type\": \"System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.NodeFamily, GraphAlgorithmRenderer]], mscorlib\",\r\n    \"$values\": [\r\n      {\r\n        \"$type\": \"GraphAlgorithmRenderer.Config.NodeFamily, GraphAlgorithmRenderer\",\r\n        \"Name\": \"node#0\",\r\n        \"Ranges\": {\r\n          \"$type\": \"System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer]], mscorlib\",\r\n          \"$values\": [\r\n            {\r\n              \"$type\": \"GraphAlgorithmRenderer.Config.IdentifierPartTemplate, GraphAlgorithmRenderer\",\r\n              \"Name\": \"v\",\r\n              \"BeginTemplate\": \"0\",\r\n              \"EndTemplate\": \"n\"\r\n            }\r\n          ]\r\n        },\r\n        \"ValidationTemplate\": \"a\",\r\n        \"ConditionalProperties\": {\r\n          \"$type\": \"System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer]], mscorlib\",\r\n          \"$values\": [\r\n            {\r\n              \"$type\": \"GraphAlgorithmRenderer.Config.ConditionalProperty`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], GraphAlgorithmRenderer\",\r\n              \"Condition\": {\r\n                \"$type\": \"GraphAlgorithmRenderer.Config.Condition, GraphAlgorithmRenderer\",\r\n                \"Template\": \"true\",\r\n                \"Mode\": 0,\r\n                \"FunctionNameRegex\": \".*\"\r\n              },\r\n              \"Properties\": {\r\n                \"$type\": \"System.Collections.Generic.List`1[[GraphAlgorithmRenderer.Config.INodeProperty, GraphAlgorithmRenderer]], mscorlib\",\r\n                \"$values\": [\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.LabelNodeProperty, GraphAlgorithmRenderer\",\r\n                    \"HighlightIfChanged\": false,\r\n                    \"ColorToHighLight\": null,\r\n                    \"LabelTextExpression\": \"q\",\r\n                    \"FontSize\": 1.2000000476837158\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.LineColorNodeProperty, GraphAlgorithmRenderer\",\r\n                    \"Color\": {\r\n                      \"$type\": \"Microsoft.Msagl.Drawing.Color, Microsoft.Msagl.Drawing\",\r\n                      \"A\": 255,\r\n                      \"R\": 47,\r\n                      \"G\": 79,\r\n                      \"B\": 79\r\n                    }\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.LineWidthNodeProperty, GraphAlgorithmRenderer\",\r\n                    \"LineWidth\": 1.2000000476837158\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.StyleNodeProperty, GraphAlgorithmRenderer\",\r\n                    \"Style\": 4\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.StyleNodeProperty, GraphAlgorithmRenderer\",\r\n                    \"Style\": 1\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.ShapeNodeProperty, GraphAlgorithmRenderer\",\r\n                    \"Shape\": 0\r\n                  },\r\n                  {\r\n                    \"$type\": \"GraphAlgorithmRenderer.Config.FillColorNodeProperty, GraphAlgorithmRenderer\",\r\n                    \"Color\": {\r\n                      \"$type\": \"Microsoft.Msagl.Drawing.Color, Microsoft.Msagl.Drawing\",\r\n                      \"A\": 255,\r\n                      \"R\": 0,\r\n                      \"G\": 255,\r\n                      \"B\": 255\r\n                    }\r\n                  }\r\n                ]\r\n              }\r\n            }\r\n          ]\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}";
    }
}