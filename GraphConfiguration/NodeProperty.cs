using GraphConfiguration.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;

namespace GraphConfiguration
{
    public abstract class NodeProperty
    {
        public string SubstitutedCondition(Identifier identifier)
        {
            return identifier.Substitute(Condition);
        }

        public string Condition { get; set; }

        //TODO enum
        public bool AllStackFrames { get; set; }
    }

    public class FillColorNodeProperty : NodeProperty
    {
        public FillColorNodeProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    public class ShapeNodeProperty : NodeProperty
    {
        public ShapeNodeProperty(Shape shape)
        {
            Shape = shape;
        }

        public Shape Shape { get; }
    }

    public class ValidationNodeProperty : NodeProperty
    {
        //TODO on mouse up, etc.
    }


    public class LabelNodeProperty : NodeProperty
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

    public class LineWidthNodeProperty : NodeProperty
    {
        public double LineWidth { get; }

        public LineWidthNodeProperty(double lineWidth)
        {
            LineWidth = lineWidth;
        }
    }

    public class LineColorNodeProperty : NodeProperty
    {
        public LineColorNodeProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    public class StyleNodeProperty : NodeProperty
    {
        public StyleNodeProperty(Style style)
        {
            Style = style;
        }

        public Style Style { get; }
    }
}
