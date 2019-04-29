using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphConfig.GraphElementIdentifier
{
    public class IdentifierPartRange
    {
        public string Name { get; }
        public int Begin { get; }
        public int End { get; }

        public IdentifierPartRange(string name, int begin, int end)
        {
            Name = name;
            Begin = begin;
            End = end;
        }
    }

    public class IdentifierPart
    {
        public IdentifierPart(string name, int value)
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
        public Identifier(params IdentifierPart[] parts)
        {
            var identifiersList = new List<IdentifierPart>();
            identifiersList.AddRange(parts);
            IdentifierParts = identifiersList.AsReadOnly();
        }

        public Identifier(List<IdentifierPart> parts)
        {
            IdentifierParts = parts.AsReadOnly();
        }

        public ReadOnlyCollection<IdentifierPart> IdentifierParts { get; }

        public string Substitute(string expression)
        {
            var stringToSubstitute = String.Copy(expression);
            foreach (var identifier in IdentifierParts)
            {
                stringToSubstitute = identifier.SubstituteValue(stringToSubstitute);
            }

            return stringToSubstitute;
        }

        public string Id()
        {
            return String.Join("#", IdentifierParts.Select(i => i.Name + " " + i.Value.ToString()));
        }

        public static List<Identifier> GetAllIdentifiersInRange(List<IdentifierPartRange> ranges)
        {
            var result = new List<Identifier>();
            var currentPermutation = new List<IdentifierPart>();
            foreach (var identifier in ranges)
            {
                currentPermutation.Add(new IdentifierPart(identifier.Name, identifier.Begin));
            }

            //TODO use delegate function instead
            while (true)
            {
                //TODO simplify this deep copy (or change type of current permutation to List<int>)
                result.Add(new Identifier(currentPermutation
                    .Select(x => new IdentifierPart(x.Name, x.Value)).ToList()
                ));
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