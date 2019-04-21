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
        public string ValidationTemplate { get; set; }
    }

    public class EdgeFamily : GraphElementFamily
    {
        public class EdgeEnd
        {
            public EdgeEnd(string valueTemplate, string name)
            {
                ValueTemplate = valueTemplate;
                Name = name;
            }

            public string Name { get; }
            public string ValueTemplate { get; }
        }

        public EdgeFamily(List<IdentifierPartTemplate> ranges, EdgeEnd source, EdgeEnd target) :
            base(ranges)
        {
            Source = source;
            Target = target;
            Properties = new List<Tuple<Condition, IEdgeProperty>>();
        }

        public bool IsDirected { get; set; }
        public EdgeEnd Source { get; }
        public EdgeEnd Target { get; }
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