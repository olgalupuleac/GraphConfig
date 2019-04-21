using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphConfiguration
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