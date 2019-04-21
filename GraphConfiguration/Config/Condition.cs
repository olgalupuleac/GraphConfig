namespace GraphConfiguration.Config
{
    public class Condition
    {
        public Condition(string conditionExpression, bool allStackFrames = false)
        {
            ConditionExpression = conditionExpression;
            AllStackFrames = allStackFrames;
        }

        public string ConditionExpression { get; }

        //TODO enum
        public bool AllStackFrames { get; }
    }
}
