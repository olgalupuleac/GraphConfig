﻿using System.Collections.Generic;

namespace GraphConfiguration
{
    public abstract class GraphElementFamily
    {
        protected GraphElementFamily(IdentifierTemplate ranges)
        {
            Ranges = ranges;
        }

        public IdentifierTemplate Ranges { get; }
    }

    public class EdgeFamily : GraphElementFamily
    {
        public EdgeFamily(IdentifierTemplate ranges, string sourceExpression, string targetExpression) :
            base(ranges)
        {
            SourceExpression = sourceExpression;
            TargetExpression = targetExpression;
            Properties = new List<EdgeProperty>();
        }

        public bool IsDirected { get; set; }
        public string SourceExpression { get; }
        public string TargetExpression { get; }
        public List<EdgeProperty> Properties { get; set; }
    }

    public class NodeFamily : GraphElementFamily
    {
        public NodeFamily(IdentifierTemplate ranges) : base(ranges)
        {
            Properties = new List<NodeProperty>();
        }
        public List<NodeProperty> Properties { get; set; }
    }

    public class GraphConfig
    {
        public HashSet<EdgeFamily> Edges { get; set; }
        public HashSet<NodeFamily> Nodes { get; set; }

        public GraphConfig()
        {
            Edges = new HashSet<EdgeFamily>();
            Nodes = new HashSet<NodeFamily>();
        }
    }
}