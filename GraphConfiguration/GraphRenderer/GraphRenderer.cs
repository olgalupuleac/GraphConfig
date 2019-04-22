using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnvDTE;
using GraphConfiguration.Config;
using GraphConfiguration.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;
using Debugger = EnvDTE.Debugger;
using StackFrame = EnvDTE.StackFrame;

namespace GraphConfiguration.GraphRenderer
{
    public class GraphRenderer
    {
        private readonly GraphConfig _config;
        private Debugger _debugger;
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
                var nodeIdentifiers =
                    GetIdentifiersForCondition(GetIdentifiers(nodeFamily), nodeFamily.ValidationTemplate);

                foreach (var identifier in nodeIdentifiers)
                {
                    var node = _graph.AddNode(identifier.Id());
                    foreach (var property in nodeFamily.Properties)
                    {
                        if (!property.Item1.AllStackFrames)
                        {
                            ApplyNodePropertyIfTrue(property, identifier, node);
                        }
                        else
                        {
                            var currentStackframe = _debugger.CurrentStackFrame;
                            var stackframes = _debugger.CurrentThread.StackFrames;
                            foreach (StackFrame stackframe in stackframes)
                            {
                                _debugger.CurrentStackFrame = stackframe;
                                if (ApplyNodePropertyIfTrue(property, identifier, node))
                                {
                                    break;
                                }
                            }

                            _debugger.CurrentStackFrame = currentStackframe;
                        }
                        
                    }
                }
            }

            foreach (var edgeFamily in _config.Edges)
            {
                var edgeIdentifiers =
                    GetIdentifiersForCondition(GetIdentifiers(edgeFamily), edgeFamily.ValidationTemplate);
                foreach (var identifier in edgeIdentifiers)
                {
                    var edge = AddEdge(edgeFamily, identifier);
                    foreach (var property in edgeFamily.Properties)
                    {
                        if (!property.Item1.AllStackFrames)
                        {
                            if (CheckConditionForIdentifier(property.Item1.ConditionExpression,
                                identifier))
                            {
                                property.Item2.Apply(edge, _debugger, identifier);
                            }
                        }
                        else
                        {
                            var currentStackframe = _debugger.CurrentStackFrame;
                            var stackframes = _debugger.CurrentThread.StackFrames;
                            foreach (StackFrame stackframe in stackframes)
                            {
                                _debugger.CurrentStackFrame = stackframe;
                                if(ApplyEdgePropertyIfTrue(property, identifier, edge))
                                    break;
                            }

                            _debugger.CurrentStackFrame = currentStackframe;
                        }
                       
                    }
                }
            }

            return _graph;
        }


        private bool ApplyNodePropertyIfTrue(Tuple<Condition, INodeProperty> property,
            Identifier identifier, Node node)
        {
            if (CheckConditionForIdentifier(property.Item1.ConditionExpression,
                identifier))
            {
                property.Item2.Apply(node, _debugger, identifier);
                return true;
            }

            return false;
        }

        private bool ApplyEdgePropertyIfTrue(Tuple<Condition, IEdgeProperty> property,
            Identifier identifier, Edge edge)
        {
            if (CheckConditionForIdentifier(property.Item1.ConditionExpression,
                identifier))
            {
                property.Item2.Apply(edge, _debugger, identifier);
                return true;
            }

            return false;
        }

        private List<Identifier> GetIdentifiers(GraphElementFamily family)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var ranges = new List<IdentifierPartRange>();
            foreach (var partTemplate in family.Ranges)
            {
                var beginString = _debugger.GetExpression(partTemplate.BeginTemplate).Value;
                var endString = _debugger.GetExpression(partTemplate.EndTemplate).Value;
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

        private List<Identifier> GetIdentifiersForCondition(List<Identifier> identifiers, string conditionTemplate)
        {
            if (conditionTemplate == null)
            {
                return identifiers;
            }

            var validIdentifiers = new List<Identifier>();
            foreach (var identifier in identifiers)
            {
                if (CheckConditionForIdentifier(conditionTemplate, identifier))
                {
                    validIdentifiers.Add(identifier);
                }
            }

            return validIdentifiers;
        }

        public static string Substitute(string expression, Identifier identifier, Debugger debugger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var stackFrame = debugger.CurrentStackFrame;
            var result = expression.Replace("__CURRENT_FUNCTION__", stackFrame.FunctionName);
            for (int i = 1; i <= stackFrame.Arguments.Count; i++)
            {
                result = result.Replace($"__ARG{i}__", stackFrame.Arguments.Item(i).Value);
            }
            Debug.WriteLine(identifier.Substitute(result));
            return identifier.Substitute(result);
        }


        private Expression GetExpression(string template, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string expression = Substitute(template,
                identifier, _debugger);
            return _debugger.GetExpression(expression);
        }


        bool CheckConditionForIdentifier(string conditionTemplate, Identifier identifier)
        {
            var conditionResult = GetExpression(conditionTemplate, identifier);
            Debug.WriteLine(conditionTemplate);
            Debug.WriteLine(conditionResult.Value);
            return conditionResult.IsValidValue && conditionResult.Value.Equals("true");
        }


        private Identifier NodeIdentifier(EdgeFamily.EdgeEnd edgeEnd, Identifier identifier)
        {
            var templates = edgeEnd.GeTemplates();
            var res = new List<IdentifierPart>();
            foreach (var template in templates)
            {
                //TODO safe
                var value = Int32.Parse(GetExpression(template.Item2, identifier).Value);
                res.Add(new IdentifierPart(template.Item1, value));
            }
            return new Identifier(res);
        }


        private Edge AddEdge(EdgeFamily edgeFamily,
            Identifier identifier)
        {
            //TODO check IsValidValue
            var source  = NodeIdentifier(edgeFamily.Source, identifier).Id();
            var target = NodeIdentifier(edgeFamily.Target, identifier).Id();
            var sourceNode = _graph.FindNode(source);
            var targetNode = _graph.FindNode(target);
            if (targetNode == null || sourceNode == null)
            {
                //TODO more specific exception
                throw new SystemException("Target or source nodes do not exist");
            }

            var edge = _graph.AddEdge(source, target);
            _edges[identifier.Id()] = edge;
            if (!edgeFamily.IsDirected)
            {
                edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
            }

            return edge;
        }
    }
}