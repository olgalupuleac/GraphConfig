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
        private Graph _graph;
        private readonly Dictionary<string, Edge> _edges;
        private TimeSpan _getExpressionTimeSpan = new TimeSpan();
        private TimeSpan _setCurrentStackFrameTimeSpan = new TimeSpan();
        private int _getExpressionCallsNumber = 0;
        private int _setCurrentStackFrameNumber = 0;

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
            _graph = new Graph();
            foreach (var nodeFamily in _config.Nodes)
            {
                var nodeIdentifiers =
                    GetIdentifiersForCondition(GetIdentifiers(nodeFamily), nodeFamily.ValidationTemplate);

                foreach (var identifier in nodeIdentifiers)
                {
                    var node = _graph.AddNode(identifier.Id());
                    foreach (var property in nodeFamily.Properties)
                    {
                        if (!property.Item1.Mode)
                        {
                            ApplyNodePropertyIfTrue(property, identifier, node);
                        }
                        else
                        {
                            var currentStackframe = _debugger.CurrentStackFrame;
                            var stackframes = _debugger.CurrentThread.StackFrames;
                            foreach (StackFrame stackframe in stackframes)
                            {
                                SetStackFrame(stackframe);

                                if (ApplyNodePropertyIfTrue(property, identifier, node))
                                {
                                    break;
                                }
                            }

                            SetStackFrame(currentStackframe);
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
                        if (!property.Item1.Mode)
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
                                SetStackFrame(stackframe);

                                if (ApplyEdgePropertyIfTrue(property, identifier, edge))
                                    break;
                            }

                            SetStackFrame(currentStackframe);
                        }
                    }
                }
            }
            string getExpressionTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                _getExpressionTimeSpan.Hours, _getExpressionTimeSpan.Minutes, _getExpressionTimeSpan.Seconds,
                _getExpressionTimeSpan.Milliseconds / 10);
            Debug.WriteLine($"got {_getExpressionCallsNumber} expressions in {getExpressionTime}");

            string setStackFrameTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                _setCurrentStackFrameTimeSpan.Hours, _setCurrentStackFrameTimeSpan.Minutes, _setCurrentStackFrameTimeSpan.Seconds,
                _setCurrentStackFrameTimeSpan.Milliseconds / 10);
            Debug.WriteLine($"set {_setCurrentStackFrameNumber} stackframes in {setStackFrameTime}");
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
                var beginString = GetExpression(partTemplate.BeginTemplate, null).Value;
                var endString = GetExpression(partTemplate.EndTemplate, null).Value;
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

            return identifier.Substitute(result);
        }


        private Expression GetExpression(string template, Identifier identifier)
        {
            string expression = identifier == null
                ? template
                : Substitute(template,
                    identifier, _debugger);
            ThreadHelper.ThrowIfNotOnUIThread();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = _debugger.GetExpression(expression);
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Debug.WriteLine($"get expression {_getExpressionCallsNumber++} in {elapsedTime}");
            _getExpressionTimeSpan += ts;
            return result;
        }


        bool CheckConditionForIdentifier(string conditionTemplate, Identifier identifier)
        {
            var conditionResult = GetExpression(conditionTemplate, identifier);
            return conditionResult.IsValidValue && conditionResult.Value.Equals("true");
        }


        private Identifier NodeIdentifier(EdgeFamily.EdgeEnd edgeEnd, Identifier identifier)
        {
            var templates = edgeEnd.GetTemplates();
            var res = new List<IdentifierPart>();
            foreach (var template in templates)
            {
                //TODO safe
                var value = Int32.Parse(GetExpression(template.Item2, identifier).Value);
                res.Add(new IdentifierPart(template.Item1, value));
            }

            return new Identifier(res);
        }

        private void SetStackFrame(StackFrame stackFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                _debugger.CurrentStackFrame = stackFrame;
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Debug.WriteLine($"set stackframe {_setCurrentStackFrameNumber++} {stackFrame.FunctionName} in {elapsedTime}");
                _setCurrentStackFrameTimeSpan += ts;
            }
            catch (Exception)
            {
            }
        }


        private Edge AddEdge(EdgeFamily edgeFamily,
            Identifier identifier)
        {
            //TODO check IsValidValue
            var source = NodeIdentifier(edgeFamily.Source, identifier).Id();
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