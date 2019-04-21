using System;
using System.Collections.Generic;
using GraphConfiguration;
using GraphConfiguration.Config;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphmapsWithMesh;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuple = System.Tuple;

namespace GraphConfigurationTest
{
    [TestClass]
    public class GraphConfigTest
    {
        //TODO test something
        [TestMethod]
        public void CreateGraphConfigForDfs()
        {
            NodeFamily nodes = new NodeFamily(
                new List<IdentifierPartTemplate>
                {
                    new IdentifierPartTemplate("v", "0", "n")
                });
            var dfsNode = System.Tuple.Create(
                new Condition("\"__CURRENT_FUNCTION__\" == \"dfs\" && __ARG1__ == __v__"),
                (INodeProperty) new FillColorNodeProperty(Color.Red));

            nodes.Properties.Add(dfsNode);
            EdgeFamily edges = new EdgeFamily(
                new List<IdentifierPartTemplate>
                {
                    new IdentifierPartTemplate("a", "0", "n"),
                    new IdentifierPartTemplate("b", "0", "n"),
                    new IdentifierPartTemplate("x", "0", "g[__a__].size()")
                }, "__a__", "__b__");


            var edgeValidationEdgeProperty = Tuple.Create(
                new Condition("__a__ < __b__ && g[__a__][__x__] == __b__"),
                (IEdgeProperty) new ValidationEdgeProperty());

            var dfsEdges = Tuple.Create(new Condition("p[__a__] == __b__ || p[__b__] == __a__"),
                (IEdgeProperty) new LineColorEdgeProperty(Color.Red));

            edges.Properties.Add(edgeValidationEdgeProperty);
            edges.Properties.Add(dfsEdges);
            GraphConfig config = new GraphConfig
            {
                Edges = new HashSet<EdgeFamily> {edges}, Nodes = new HashSet<NodeFamily> {nodes}
            };
        }
    }
}