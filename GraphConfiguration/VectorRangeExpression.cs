using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphConfiguration
{
    public class ScalarRangeExpression
    {
        public string Name { get; }
        public string BeginExpression { get; }
        public string EndExpression { get; }

        public ScalarRangeExpression(string name, string beginExpression, string endExpression)
        {
            Name = name;
            BeginExpression = beginExpression;
            EndExpression = endExpression;
        }
    }

    public class VectorRangeExpression
    {
        private ReadOnlyCollection<ScalarRangeExpression> Ranges { get; }

        public VectorRangeExpression(List<ScalarRangeExpression> ranges)
        {
            Ranges = ranges.AsReadOnly();
        }
    }
}