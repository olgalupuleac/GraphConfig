﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnvDTE;
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
                var nodeIdentifiers =
                    GetIdentifiersForCondition(GetIdentifiers(nodeFamily), nodeFamily.ValidationTemplate);

                foreach (var identifier in nodeIdentifiers)
                {
                    _graph.AddNode(identifier.Id());
                }
            }

            foreach (var edgeFamily in _config.Edges)
            {
                var edgeIdentifiers =
                    GetIdentifiersForCondition(GetIdentifiers(edgeFamily), edgeFamily.ValidationTemplate);
                foreach (var id in edgeIdentifiers)
                {
                    AddEdge(edgeFamily, id);
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

        private string SubstituteStackFrameParameters(string expression, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var stackFrame = _debugger.CurrentStackFrame;
            var result = expression.Replace("__CURRENT_FUNCTION__", stackFrame.FunctionName);
            for (int i = 1; i <= stackFrame.Arguments.Count; i++)
            {
                result = result.Replace($"__ARG{i}__", stackFrame.Arguments.Item(i).Value);
            }

            return identifier.Substitute(result);
        }


        private Expression GetExpression(string template, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string expression = SubstituteStackFrameParameters(template,
                identifier);
            return _debugger.GetExpression(expression);
        }


        bool CheckConditionForIdentifier(string conditionTemplate, Identifier identifier)
        {
            var conditionResult = GetExpression(conditionTemplate, identifier);
            return conditionResult.IsValidValue && conditionResult.Value.Equals("true");
        }


        private void AddEdge(EdgeFamily edgeFamily,
            Identifier identifier)
        {
            //TODO check IsValidValue
            var sourceValue = GetExpression(edgeFamily.Source.ValueTemplate, identifier).Value;
            var targetValue = GetExpression(edgeFamily.Target.ValueTemplate, identifier).Value;
            var source = edgeFamily.Source.Name + " " + sourceValue;
            var target = edgeFamily.Target.Name + " " + targetValue;
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
        }
    }
}