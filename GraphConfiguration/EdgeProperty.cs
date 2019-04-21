using GraphConfiguration.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;

namespace GraphConfiguration
{
    public abstract class EdgeProperty
    {
        public string SubstitutedCondition(Identifier identifier)
        {
            return identifier.Substitute(Condition);
        }

        public string Condition { get; set; }

        //TODO enum
        public bool AllStackFrames { get; set; }
    }

    public class ValidationEdgeProperty : EdgeProperty
    {
        //TODO on mouse up, etc.
    }


    public class LabelEdgeProperty : EdgeProperty
    {
        public LabelEdgeProperty(string label, string labelTextExpression)
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

    public class LineWidthEdgeProperty : EdgeProperty
    {
        public double LineWidth { get; }

        public LineWidthEdgeProperty(double lineWidth)
        {
            LineWidth = lineWidth;
        }
    }

    public class LineColorEdgeProperty : EdgeProperty
    {
        public LineColorEdgeProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    public class StyleEdgeProperty : EdgeProperty
    {
        public StyleEdgeProperty(Style style)
        {
            Style = style;
        }

        public Style Style { get; }
    }
}