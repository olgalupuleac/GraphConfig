using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphConfiguration.GraphElementIdentifier
{
    public class ScalarRange
    {
        public string Name { get; }
        public int Begin { get; }
        public int End { get; }

        public ScalarRange(string name, int begin, int end)
        {
            Name = name;
            Begin = begin;
            End = end;
        }
    }

    public class ScalarId
    {
        public ScalarId(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
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
        public Identifier(params ScalarId[] identifiers)
        {
            SingleIdentifiers = new List<ScalarId>();
            SingleIdentifiers.AddRange(identifiers);
        }

        public Identifier(List<ScalarId> identifiers)
        {
            SingleIdentifiers = identifiers;
        }

        public List<ScalarId> SingleIdentifiers { get; set; }

        public string Substitute(string expression)
        {
            var stringToSubstitute = String.Copy(expression);
            foreach (var identifier in SingleIdentifiers)
            {
                stringToSubstitute = identifier.SubstituteValue(stringToSubstitute);
            }

            return stringToSubstitute;
        }

        public string Id()
        {
            return String.Join("#", SingleIdentifiers.Select(i => i.Name + "$" + i.Value.ToString()));
        }

        public static List<Identifier> GetAllIdentifiersInRange(List<ScalarRange> ranges)
        {
            var result = new List<Identifier>();
            var currentPermutation = new List<ScalarId>();
            foreach (var identifier in ranges)
            {
                currentPermutation.Add(new ScalarId(identifier.Name, identifier.Begin));
            }

            //TODO use delegate function instead
            while (true)
            {
                //TODO simplify this deep copy (or change type of current permutation to List<int>)
                result.Add(new Identifier
                {
                    SingleIdentifiers = currentPermutation
                        .Select(x => new ScalarId(x.Name, x.Value)).ToList()
                });
                for (var indexToIncrease = currentPermutation.Count - 1; indexToIncrease >= -1; indexToIncrease--)
                {
                    if (indexToIncrease == -1)
                    {
                        return result;
                    }

                    if (currentPermutation[indexToIncrease].Value < ranges[indexToIncrease].End - 1)
                    {
                        currentPermutation[indexToIncrease].Value++;
                        for (var trailingIndex = indexToIncrease + 1;
                            trailingIndex < currentPermutation.Count;
                            trailingIndex++)
                        {
                            currentPermutation[trailingIndex].Value = ranges[trailingIndex].Begin;
                        }

                        break;
                    }
                }
            }
        }
    }
}