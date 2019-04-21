namespace GraphConfiguration.Config
{
    public class IdentifierPartTemplate
    {
        public string Name { get; }
        public string BeginExpression { get; }
        public string EndExpression { get; }

        public IdentifierPartTemplate(string name, string beginExpression, string endExpression)
        {
            Name = name;
            BeginExpression = beginExpression;
            EndExpression = endExpression;
        }
    }
}