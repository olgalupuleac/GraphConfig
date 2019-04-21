using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraphConfiguration.Config
{
    public abstract class GraphElementFamily
    {
        protected GraphElementFamily(List<IdentifierPartTemplate> ranges)
        {
            Ranges = ranges.AsReadOnly();
        }

        public ReadOnlyCollection<IdentifierPartTemplate> Ranges { get; }
    }

    public class EdgeFamily : GraphElementFamily
    {
        public EdgeFamily(List<IdentifierPartTemplate> ranges, string sourceExpression, string targetExpression) :
            base(ranges)
        {
            SourceExpression = sourceExpression;
            TargetExpression = targetExpression;
            Properties = new List<Tuple<Condition, IEdgeProperty>>();
        }

        public bool IsDirected { get; set; }
        public string SourceExpression { get; }
        public string TargetExpression { get; }
        public List<Tuple<Condition, IEdgeProperty>> Properties { get; set; }
    }

    public class NodeFamily : GraphElementFamily
    {
        public NodeFamily(List<IdentifierPartTemplate> ranges) : base(ranges)
        {
            Properties = new List<Tuple<Condition, INodeProperty>>();
        }

        public List<Tuple<Condition, INodeProperty>> Properties { get; set; }
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