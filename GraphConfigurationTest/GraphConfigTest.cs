using System.Collections.Generic;
using GraphConfiguration;
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
            NodeFamily nodes = new NodeFamily(new IdentifierTemplate(
                new List<IdentifierPartTemplate>
                {
                    new IdentifierPartTemplate("v", "0", "n")
                }));
            FillColorNodeProperty dfsNode = new FillColorNodeProperty(Color.Red)
            {
                Condition = "\"__CURRENT_FUNCTION__\" == \"dfs\" && __ARG1__ == __v__"
            };
            nodes.Properties.Add(dfsNode);
            EdgeFamily edges = new EdgeFamily(
                new IdentifierTemplate(
                    new List<IdentifierPartTemplate>
                    {
                        new IdentifierPartTemplate("a", "0", "n"),
                        new IdentifierPartTemplate("b", "0", "n"),
                        new IdentifierPartTemplate("x", "0", "g[__a__].size()")
                    }), "__a__", "__b__");

            ValidationEdgeProperty edgeValidationEdgeProperty = new ValidationEdgeProperty()
            {
                Condition = "__a__ < __b__ && g[__a__][__x__] == __b__"
            };

            LineColorEdgeProperty dfsEdges = new LineColorEdgeProperty(Color.Red)
            {
                Condition = "p[__a__] == __b__ || p[__b__] == __a__"
            };
            edges.Properties.Add(edgeValidationEdgeProperty);
            edges.Properties.Add(dfsEdges);
            GraphConfig config = new GraphConfig
            {
                Edges = new HashSet<EdgeFamily> {edges}, Nodes = new HashSet<NodeFamily> {nodes}
            };
        }
    }
}