using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphAlgorithmRenderer.GraphElementIdentifier
{
    public class IdentifierPartRange
    {
        public string Name { get; set; }
        public int Begin { get; set; }
        public int End { get; set; }

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
        public string Name { get; }

        public Identifier(string name, params IdentifierPart[] parts)
        {
            Name = name;
            var identifiersList = new List<IdentifierPart>();
            identifiersList.AddRange(parts);
            IdentifierParts = identifiersList.AsReadOnly();
        }

        public Identifier(string name, List<IdentifierPart> parts)
        {
            Name = name;
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
            return Name + "@" + String.Join("#", IdentifierParts.Select(i => i.Name + " " + i.Value.ToString()));
        }


        public static List<Identifier> GetAllIdentifiersInRange(string name, List<IdentifierPartRange> ranges)
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
                result.Add(new Identifier(name, currentPermutation
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