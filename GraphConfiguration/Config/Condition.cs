namespace GraphConfiguration.Config
{
    public enum ConditionMode
    {
        CurrentStackFrame,
        AllStackFrames
    }

    public class Condition
    {
        public Condition(string template, string functionNameRegex = @".*",
            ConditionMode mode = ConditionMode.CurrentStackFrame)
        {
            Template = template;
            FunctionNameRegex = functionNameRegex;
            Mode = mode;
        }

        public string Template { get; }

        public ConditionMode Mode { get; }

        public string FunctionNameRegex { get; }
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