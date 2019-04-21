using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GraphConfiguration.GraphElementIdentifier;

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
            public EdgeEnd(NodeFamily node, List<string> templates)
            {
                _node = node;
                _templates = templates.AsReadOnly();
            }

            private readonly NodeFamily _node;
            private readonly ReadOnlyCollection<string> _templates;

            public List<Tuple<string, string>> GeTemplates()
            {
               // TODO throw exception
               Debug.Assert(_templates.Count == _node.Ranges.Count);
               var result = new List<Tuple<string, string>>();
               for (int i = 0; i < _templates.Count; i++)
               {
                   result.Add(Tuple.Create(_node.Ranges[i].Name, _templates[i]));
               }

               return result;
            }
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