using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace GraphConfiguration.Config
{
    public abstract class GraphElementFamily<T>
    {
        protected GraphElementFamily(List<IdentifierPartTemplate> ranges)
        {
            Ranges = ranges.AsReadOnly();
            ConditionalProperties = new List<ConditionalProperty<T>>();
        }

        public ReadOnlyCollection<IdentifierPartTemplate> Ranges { get; }
        public string ValidationTemplate { get; set; }
        public List<ConditionalProperty<T>> ConditionalProperties { get; set; }

        public List<ConditionalProperty<T>> GetCurrentStackFrameProperties()
        {
            return ConditionalProperties.Where(conditionalProperty =>
                conditionalProperty.Condition.Mode == ConditionMode.CurrentStackFrame).ToList();
        }

        public List<ConditionalProperty<T>> GetAllStackFramesProperties()
        {
            return ConditionalProperties.Where(conditionalProperty =>
                conditionalProperty.Condition.Mode == ConditionMode.AllStackFrames).ToList();
        }
    }

    public class EdgeFamily : GraphElementFamily<IEdgeProperty>
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

            public List<Tuple<string, string>> GetTemplates()
            {
                // TODO throw exception
                Debug.Assert(_templates.Count == _node.Ranges.Count);
                return _templates.Select((t, i) => Tuple.Create(_node.Ranges[i].Name, t)).ToList();
            }
        }

        public EdgeFamily(List<IdentifierPartTemplate> ranges, EdgeEnd source, EdgeEnd target,
            bool isDirected = false) :
            base(ranges)
        {
            Source = source;
            Target = target;
            IsDirected = isDirected;
        }

        public bool IsDirected { get; }
        public EdgeEnd Source { get; }
        public EdgeEnd Target { get; }
    }

    public class NodeFamily : GraphElementFamily<INodeProperty>
    {
        public NodeFamily(List<IdentifierPartTemplate> ranges) : base(ranges)
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