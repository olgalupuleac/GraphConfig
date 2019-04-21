using System;
using System.Collections.Generic;
using System.Diagnostics;
using GraphConfiguration.Config;
using GraphConfiguration.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
using Debugger = EnvDTE.Debugger;

namespace GraphConfiguration.GraphRenderer
{
    public class GraphRenderer
    {
        private readonly GraphConfig _config;
        private readonly Debugger _debugger;
        private readonly Graph _graph;
        private readonly Dictionary<string, Edge> _edges;

        public GraphRenderer(GraphConfig config, Debugger debugger)
        {
            _config = config;
            _debugger = debugger;
            _graph = new Graph();
            _edges = new Dictionary<string, Edge>();
        }

        public Graph RenderGraph()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (var nodeFamily in _config.Nodes)
            {
                var nodeIdentifiers = GetIdentifiers(nodeFamily);
                foreach (var conditionAndProperty in nodeFamily.Properties)
                {
                    var conditionTemplate = conditionAndProperty.Item1;
                    var property = conditionAndProperty.Item2;
                    foreach (var identifier in nodeIdentifiers)
                    {
                        var condition =
                            SubstituteStackFrameParameters(
                                identifier.Substitute(conditionTemplate.ConditionExpression));
                        var conditionExpression = _debugger.GetExpression(condition);
                        if (conditionExpression.IsValidValue &&
                            conditionExpression.Value.Equals("true"))
                        {
                            ApplyNodePropertyToGraph(
                                property, identifier.Id());
                        }
                    }
                }
            }

            return _graph;
            foreach (var edgeFamily in _config.Edges)
            {
                var nodeIdentifiers = GetIdentifiers(edgeFamily);
                if (nodeIdentifiers == null)
                {
                    return null;
                }

                foreach (var conditionAndProperty in edgeFamily.Properties)
                {
                    var conditionTemplate = conditionAndProperty.Item1;
                    var property = conditionAndProperty.Item2;
                    foreach (var identifier in nodeIdentifiers)
                    {
                        var condition =
                            SubstituteStackFrameParameters(
                                identifier.Substitute(conditionTemplate.ConditionExpression));
                        var conditionExpression = _debugger.GetExpression(condition);
                        if (conditionExpression.IsValidValue &&
                            conditionExpression.Value.Equals("true"))
                        {
                            var source = identifier.Substitute(edgeFamily.SourceExpression);
                            var target = identifier.Substitute(edgeFamily.TargetExpression);
                            ApplyEdgePropertyToGraph(
                                property, source: source, target: target, id: identifier.Id());
                        }
                    }
                }
            }

            return _graph;
        }

        private List<Identifier> GetIdentifiers(GraphElementFamily family)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var ranges = new List<IdentifierPartRange>();
            foreach (var partTemplate in family.Ranges)
            {
                var beginString = _debugger.GetExpression(partTemplate.BeginExpression).Value;
                var endString = _debugger.GetExpression(partTemplate.EndExpression).Value;
                Debug.WriteLine(beginString);
                Debug.WriteLine(endString);
                if (Int32.TryParse(beginString, out var begin) &&
                    Int32.TryParse(endString, out var end))
                {
                    ranges.Add(new IdentifierPartRange(partTemplate.Name, begin, end));
                }
                else
                {
                    return null;
                }
            }

            return Identifier.GetAllIdentifiersInRange(ranges);
        }

        private string SubstituteStackFrameParameters(string expression)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var stackFrame = _debugger.CurrentStackFrame;
            var result = expression.Replace("__CURRENT_FUNCTION__", stackFrame.FunctionName);
            for (int i = 1; i <= stackFrame.Arguments.Count; i++)
            {
                result = result.Replace($"__ARG{i}__", stackFrame.Arguments.Item(i).Value);
            }

            return result;
        }

        private void ApplyNodePropertyToGraph(INodeProperty property,
            string id)
        {
            if (property is ValidationNodeProperty)
            {
                _graph.AddNode(id);
                return;
            }

            //TODO other properies
        }


        private void ApplyEdgePropertyToGraph(IEdgeProperty property,
            string target, string source, string id)
        {
            if (property is ValidationEdgeProperty)
            {
                var sourceNode = _graph.FindNode(source);
                var targetNode = _graph.FindNode(target);
                if (targetNode == null || sourceNode == null)
                {
                    //TODO more specific exception
                    throw new SystemException("Target or source nodes do not exist");
                }

                var edge = _graph.AddEdge(source, target);
                _edges[id] = edge;
            }

            //TODO other properies
        }
    }
}