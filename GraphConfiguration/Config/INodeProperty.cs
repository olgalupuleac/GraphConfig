using System;
using EnvDTE;
using GraphConfiguration.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.Shell;

namespace GraphConfiguration.Config
{
    public interface INodeProperty
    {
        void Apply(Node node, Debugger debugger, Identifier identifier);
    }

    public class FillColorNodeProperty : INodeProperty
    {
        public FillColorNodeProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.FillColor = Color;
        }
    }

    public class ShapeNodeProperty : INodeProperty
    {
        public ShapeNodeProperty(Shape shape)
        {
            Shape = shape;
        }

        public Shape Shape { get; }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.Shape = Shape;
        }
    }

    public class LabelNodeProperty : INodeProperty
    {
        public LabelNodeProperty(string label, string labelTextExpression)
        {
            LabelTextExpression = labelTextExpression;
        }

        public bool HighlightIfChanged { get; set; }
        public Color? ColorToHighLight { get; set; }
        public string LabelTextExpression { get; }
        public double FontSize { get; set; }
        public FontStyle? FontStyle { get; set; }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var expression = GraphRenderer.GraphRenderer.Substitute(LabelTextExpression, identifier, debugger);
            var label = debugger.GetExpression(expression).Value;
            node.Label.FontStyle = FontStyle ?? node.Label.FontStyle;
            if (Math.Abs(FontSize) > 0.01)
            {
                node.Label.FontSize = FontSize;
            }

            if (label.Equals(node.LabelText))
            {
                return;
            }

            node.Label.Text = label;
            if (HighlightIfChanged)
            {
                node.Label.FontColor = ColorToHighLight ?? Color.Red;
            }
        }
    }

    public class LineWidthNodeProperty : INodeProperty
    {
        public double LineWidth { get; }

        public LineWidthNodeProperty(double lineWidth)
        {
            LineWidth = lineWidth;
        }

        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.LineWidth = LineWidth;
        }
    }

    public class LineColorNodeProperty : INodeProperty
    {
        public LineColorNodeProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.Color = Color;
        }
    }

    public class StyleNodeProperty : INodeProperty
    {
        public StyleNodeProperty(Style style)
        {
            Style = style;
        }

        public Style Style { get; }
        public void Apply(Node node, Debugger debugger, Identifier identifier)
        {
            node.Attr.AddStyle(Style);
        }
    }
}