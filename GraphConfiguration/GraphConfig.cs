using System.Collections.Generic;

namespace GraphConfiguration
{
    public abstract class GraphElementFamily
    {
        protected GraphElementFamily(List<ScalarIdRange> ranges)
        {
            Ranges = ranges;
        }

        public List<ScalarIdRange> Ranges { get; }

        public List<Property> Properties { get; set; }
    }

    public class EdgeFamily : GraphElementFamily
    {
        public EdgeFamily(List<ScalarIdRange> ranges, string sourceExpression, string targetExpression) :
            base(ranges)
        {
            SourceExpression = sourceExpression;
            TargetExpression = targetExpression;
        }

        public bool IsDirected { get; set; }
        public string SourceExpression { get; }
        public string TargetExpression { get; }
    }

    public class NodeFamily : GraphElementFamily
    {
        public NodeFamily(List<ScalarIdRange> ranges) : base(ranges)
        {
        }
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