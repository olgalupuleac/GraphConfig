using GraphConfiguration.GraphElementIdentifier;
using Microsoft.Msagl.Drawing;

namespace GraphConfiguration.Config
{
    public interface IEdgeProperty
    {
    }

    public class LabelEdgeProperty : IEdgeProperty
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

    public class LineWidthEdgeProperty : IEdgeProperty
    {
        public double LineWidth { get; }

        public LineWidthEdgeProperty(double lineWidth)
        {
            LineWidth = lineWidth;
        }
    }

    public class LineColorEdgeProperty : IEdgeProperty
    {
        public LineColorEdgeProperty(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    public class StyleEdgeProperty : IEdgeProperty
    {
        public StyleEdgeProperty(Style style)
        {
            Style = style;
        }

        public Style Style { get; }
    }
}