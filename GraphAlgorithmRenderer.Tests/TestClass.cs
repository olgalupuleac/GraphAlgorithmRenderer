// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GraphAlgorithmRendererLib.Config;
using GraphAlgorithmRendererLib.Serializer;
using Microsoft.Msagl.Drawing;
using NUnit.Framework;

namespace GraphAlgorithmRenderer.Tests
{
    [TestFixture]
    public class JsonSerializerTestClass
    {
        public static GraphConfig ConfigNoProperties
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
                        }, new EdgeFamily.EdgeEnd(nodes, new List<string> { "__a__" }),
                        new EdgeFamily.EdgeEnd(nodes, new List<string> { "g[__a__][__x__]" })
                    )
                { ValidationTemplate = "__x__ < g[__a__].size()" };

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
                    Edges = new List<EdgeFamily> { edges },
                    Nodes = new List<NodeFamily> { nodes }
                };

                return config;
            }
        }

        [Test]
        public void TestMethod()
        {
            var json = ConfigSerializer.ToJson(ConfigNoProperties);
            Debug.WriteLine(json);
            ConfigSerializer.FromJson(json);
        }
    }
}
