using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphConfiguration
{
    public class SingleIdentifierRange
    {
        public string Name { get; }
        public int FirstValue { get; }
        public int LastValue { get; }

        public SingleIdentifierRange(string name, int firstValue, int lastValue)
        {
            Name = name;
            FirstValue = firstValue;
            LastValue = lastValue;
        }
    }

    public class SingleIdentifier
    {
        public string Name { get; set; }
        public int Value { get; set; }

        private string WrappedName()
        {
            return "__" + Name + "__";
        }

        public string SubstituteValue(string expression)
        {
            return expression.Replace(WrappedName(), Value.ToString());
        }
    }

    public class Identifier
    {
        public Identifier(params SingleIdentifier[] identifiers)
        {
            SingleIdentifiers = new List<SingleIdentifier>();
            SingleIdentifiers.AddRange(identifiers);
        }

        public Identifier(List<SingleIdentifier> identifiers)
        {
            SingleIdentifiers = identifiers;
        }

        public List<SingleIdentifier> SingleIdentifiers { get; set; }

        public string Substitute(string expression, string functionName = "")
        {
            var stringToSubstitute = String.Copy(expression);
            foreach (var identifier in SingleIdentifiers)
            {
                stringToSubstitute = identifier.SubstituteValue(stringToSubstitute);
            }

            return stringToSubstitute.Replace("__CURRENT_FUNCTION__", functionName);
        }

        public string Id()
        {
            return String.Join("#", SingleIdentifiers.Select(i => i.Value.ToString()));
        }

        public static List<Identifier> GetAllIdentifiersInRange(List<SingleIdentifierRange> ranges)
        {
            List<Identifier> result = new List<Identifier>();
            var currentPermutation = new List<SingleIdentifier>();
            foreach (var identifier in ranges)
            {
                currentPermutation.Add(new SingleIdentifier {Name = identifier.Name, Value = identifier.FirstValue});
            }

            while (true)
            {
                //TODO simplify this deep copy
                result.Add(new Identifier
                {
                    SingleIdentifiers = currentPermutation
                        .Select(x => new SingleIdentifier {Name = x.Name, Value = x.Value}).ToList()
                });
                //TODO rename i, j
                for (var i = currentPermutation.Count - 1; i >= -1; i--)
                {
                    if (i == -1)
                    {
                        return result;
                    }

                    if (currentPermutation[i].Value < ranges[i].LastValue - 1)
                    {
                        currentPermutation[i].Value++;
                        for (var j = i + 1; j < currentPermutation.Count; j++)
                        {
                            currentPermutation[j].Value = ranges[j].FirstValue;
                        }
                        break;
                    }

                }
            }
        }
    }
}