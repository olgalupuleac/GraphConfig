﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphConfiguration
{
    public class ScalarIdRange
    {
        public string Name { get; }
        public int FirstValue { get; }
        public int LastValue { get; }

        public ScalarIdRange(string name, int firstValue, int lastValue)
        {
            Name = name;
            FirstValue = firstValue;
            LastValue = lastValue;
        }
    }

    public class ScalarId
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

        public static List<Identifier> GetAllIdentifiersInRange(List<ScalarIdRange> ranges)
        {
            var result = new List<Identifier>();
            var currentPermutation = new List<ScalarId>();
            foreach (var identifier in ranges)
            {
                currentPermutation.Add(new ScalarId {Name = identifier.Name, Value = identifier.FirstValue});
            }

            while (true)
            {
                //TODO simplify this deep copy (or change type of current permutation to List<int>)
                result.Add(new Identifier
                {
                    SingleIdentifiers = currentPermutation
                        .Select(x => new ScalarId {Name = x.Name, Value = x.Value}).ToList()
                });
                for (var indexToIncrease = currentPermutation.Count - 1; indexToIncrease >= -1; indexToIncrease--)
                {
                    if (indexToIncrease == -1)
                    {
                        return result;
                    }

                    if (currentPermutation[indexToIncrease].Value < ranges[indexToIncrease].LastValue - 1)
                    {
                        currentPermutation[indexToIncrease].Value++;
                        for (var trailingIndex = indexToIncrease + 1;
                            trailingIndex < currentPermutation.Count;
                            trailingIndex++)
                        {
                            currentPermutation[trailingIndex].Value = ranges[trailingIndex].FirstValue;
                        }

                        break;
                    }
                }
            }
        }
    }
}