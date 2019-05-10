using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using GraphAlgorithmRenderer.Config;
using GraphAlgorithmRenderer.GraphRenderer;
using Debugger = EnvDTE.Debugger;

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

        public Identifier AddIdentifierPart(IdentifierPart part)
        {
            var list = new List<IdentifierPart>(IdentifierParts) {part};
            return new Identifier(Name, list);
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

        public static List<Identifier> GetIdentifiers(string name, List<IdentifierPartTemplate> templates,
            Debugger debugger)
        {
            Identifier start = new Identifier(name:name, parts : new List<IdentifierPart>());
            return GetIdentifiers(start, templates, debugger);
        }

        private static List<Identifier> GetIdentifiers(Identifier cur, List<IdentifierPartTemplate> templates,
            Debugger debugger)
        {
            if (templates.Count == 0)
            {
                return new List<Identifier> {cur};
            }

            var head = templates.FirstOrDefault();
            Debug.Assert(head != null, nameof(head) + " != null");
            int begin = GetNumber(head.BeginTemplate, cur, debugger, $"Begin of {head.Name}");
            int end = GetNumber(head.EndTemplate, cur, debugger, $"End of {head.Name}");
            var res = new List<Identifier>();
            for (int i = begin; i < end; i++)
            {
                var part = new IdentifierPart(head.Name, i);
                var partRes = GetIdentifiers(cur.AddIdentifierPart(part), templates.Skip(1).ToList(), debugger);
                res.AddRange(partRes);
            }

            return res;
        }

        private static int GetNumber(string template, Identifier id, Debugger debugger,string message)
        {
            var expressionResult = DebuggerOperations.GetExpressionForIdentifier(template, id, debugger);
            if (!expressionResult.IsValid || !Int32.TryParse(expressionResult.Value, out var res))
            {
                throw new GraphRenderException($"{message} {template} is not valid\n{expressionResult.Value}");
            }

            return res;
        }
    }

   
}