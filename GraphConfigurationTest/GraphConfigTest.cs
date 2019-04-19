using System.Collections.Generic;
using GraphConfig;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphConfigurationTest
{
    [TestClass]
    public class GraphConfigTest
    {
        //TODO test something
        [TestMethod]
        public void CreateGraphConfigForDfs()
        {
            NodeFamily nodes = new NodeFamily(new List<ScalarIdRange>
            {
                new ScalarIdRange("v", 0, 10)
            });
            FillColorProperty dfsNode = new FillColorProperty(Color.Red)
            {
                Condition = "\"__CURRENT_FUNCTION__\" == \"dfs\" && v == __v__"
            };
            nodes.Properties.Add(dfsNode);
            EdgeFamily edges = new EdgeFamily(new List<ScalarIdRange>()
            {
                new ScalarIdRange("i", 0, 10),
                new ScalarIdRange("j", 0, 10),
                new ScalarIdRange("x", 0, 10),
            }, "__i__", "__j__");

            ValidationProperty edgeValidationProperty = new ValidationProperty()
            {
                Condition = "g[__i__][__x__] == __j__"
            };

            FillColorProperty dfsEdges = new FillColorProperty(Color.Red)
            {
                Condition = "p[__i__] == __j__ || p[__j__] == __i__"
            };
            edges.Properties.Add(edgeValidationProperty);
            edges.Properties.Add(dfsEdges);
            Config config = new Config
            {
                Edges = new HashSet<EdgeFamily> {edges}, Nodes = new HashSet<NodeFamily> {nodes}
            };
        }
    }
}