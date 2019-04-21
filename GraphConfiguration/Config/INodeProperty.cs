using GraphConfiguration.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;

namespace GraphConfiguration.Config
{
    public interface INodeProperty
    {
    }

    public class FillColorNodeProperty : INodeProperty
    {
        public FillColorNodeProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    public class ShapeNodeProperty : INodeProperty
    {
        public ShapeNodeProperty(Shape shape)
        {
            Shape = shape;
        }

        public Shape Shape { get; }
    }

    public class ValidationNodeProperty : INodeProperty
    {
        //TODO on mouse up, etc.
    }


    public class LabelNodeProperty : INodeProperty
    {
        public LabelNodeProperty(string label, string labelTextExpression)
        {
            LabelTextExpression = labelTextExpression;
        }

        public string SubstitutedLabelTextExpression(Identifier identifier)
        {
            return identifier.Substitute(LabelTextExpression);
        }

        public bool HighlightIfChanged { get; set; }
        public Color ColorToHighLight { get; set; }
        public string LabelTextExpression { get; }
        public double FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
    }

    public class LineWidthNodeProperty : INodeProperty
    {
        public double LineWidth { get; }

        public LineWidthNodeProperty(double lineWidth)
        {
            LineWidth = lineWidth;
        }
    }

    public class LineColorNodeProperty : INodeProperty
    {
        public LineColorNodeProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    public class StyleNodeProperty : INodeProperty
    {
        public StyleNodeProperty(Style style)
        {
            Style = style;
        }

        public Style Style { get; }
    }
}
