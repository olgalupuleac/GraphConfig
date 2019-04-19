using Microsoft.Msagl.Drawing;

namespace GraphConfig
{
    public abstract class Property
    {
        public string SubstitutedCondition(Identifier identifier, string functionName = "")
        {
            return identifier.Substitute(Condition, functionName);
        }

        public string Condition { get; set; }

        //TODO enum
        public bool AllStackFrames { get; set; }

    }

    public class ValidationProperty : Property
    {
        //TODO on mouse up, etc.
    }


    public class LabelProperty : Property
    {
        public LabelProperty(string label, string labelTextExpression)
        {
            LabelTextExpression = labelTextExpression;
        }

        public string SubstitutedLabelTextExpression(Identifier identifier, string functionName = "")
        {
            return identifier.Substitute(LabelTextExpression, functionName);
        }

        public bool HighlightIfChanged { get; set; }
        public Color ColorToHighLight { get; set; }
        public string LabelTextExpression { get; }
        public double FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
    }

    public class LineWidthProperty : Property
    {
        public double LineWidth { get; }

        public LineWidthProperty(double lineWidth)
        {
            LineWidth = lineWidth;
        }
    }

    public class LineColorProperty : Property
    {
        public LineColorProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    public class FillColorProperty : Property
    {
        public FillColorProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    public class ShapeProperty : Property
    {
        public ShapeProperty(Shape shape)
        {
            Shape = shape;
        }

        public Shape Shape { get; }
    }

    public class StyleProperty : Property
    {
        public StyleProperty(Style style)
        {
            Style = style;
        }

        public Style Style { get; }
    }
}
