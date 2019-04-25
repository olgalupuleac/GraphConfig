namespace GraphConfiguration.Config
{
    public enum ConditionMode
    {
        CurrentStackFrame,
        AllStackFrames
    }

    public class Condition
    {
        public Condition(string conditionExpression, ConditionMode mode = ConditionMode.CurrentStackFrame)
        {
            ConditionExpression = conditionExpression;
            Mode = mode;
        }

        public string ConditionExpression { get; }

        //TODO enum
        public ConditionMode Mode { get; }
    }

    public class ConditionalProperty<T>
    {
        public ConditionalProperty(Condition condition, T property)
        {
            Condition = condition;
            Property = property;
        }

        public Condition Condition { get; }
        public T Property { get; }
    }
}