using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphConfiguration.GraphElementIdentifier;

namespace GraphConfiguration
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
