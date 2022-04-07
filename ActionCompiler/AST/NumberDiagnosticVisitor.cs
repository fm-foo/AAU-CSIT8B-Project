using Antlr4.Runtime.Tree;
using Action.Parser;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Action.Compiler;
using System.Numerics;

namespace Action.AST
{
    public class NumberDiagnosticVisitor : ActionBaseVisitor<IEnumerable<DiagnosticResult>>
    {

        public override IEnumerable<DiagnosticResult> VisitTerminal(ITerminalNode node)
        {
            ActionToken token = (ActionToken)node.Symbol.Type;
            return token switch
            {
                ActionToken.POINT_LIT => VisitPoint(node),
                ActionToken.INTEGER => VisitInteger(node),
                ActionToken.FLOAT_LIT => VisitFloat(node),
                _ => base.VisitTerminal(node),
            };
        }

        private IEnumerable<DiagnosticResult> VisitFloat(ITerminalNode node)
        {
            float result = float.Parse(node.GetText());
            if (result == float.PositiveInfinity)
            {
                return new DiagnosticResult[] { new DiagnosticResult(Severity.Error, $"Float value ({node.GetText()}) is larger than maximum!", Error.FloatOverflow) };
            }
            else if (result == float.NegativeInfinity)
            {
                return new DiagnosticResult[] { new DiagnosticResult(Severity.Error, $"Float value ({node.GetText()}) is smaller than minimum!", Error.FloatUnderflow) };
            }

            return Enumerable.Empty<DiagnosticResult>();
        }

        private IEnumerable<DiagnosticResult> VisitInteger(ITerminalNode node)
        {
            BigInteger result = BigInteger.Parse(node.GetText());

            if (result > int.MaxValue)
            {
                return new DiagnosticResult[] { new DiagnosticResult(Severity.Error, $"Intger value ({node.GetText()}) is larger than maximum!", Error.IntegerOverflow) };
            }
            else if (result < int.MinValue)
            {
                return new DiagnosticResult[] { new DiagnosticResult(Severity.Error, $"Intger value ({node.GetText()}) is smaller than minimum!", Error.IntegerUnderflow) };
            }

            return Enumerable.Empty<DiagnosticResult>();
        }

        private static readonly Regex pointRegex = new(
           @"(-?[0-9]+)\s*,\s*(-?[0-9]+)",
           RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private IEnumerable<DiagnosticResult> VisitPoint(ITerminalNode node)
        {
            Match match = pointRegex.Match(node.GetText());

            BigInteger left = BigInteger.Parse(match.Groups[1].Value);
            BigInteger right = BigInteger.Parse(match.Groups[2].Value);

            List<DiagnosticResult> result = new();

            if (left > int.MaxValue)
            {
                result.Add(new DiagnosticResult(Severity.Error, $"X coordinate is larger ({left}) than maximum integer value", Error.IntegerOverflow));
            }
            else if (left < int.MinValue)
            {
                result.Add(new DiagnosticResult(Severity.Error, $"X coordinate is smaller ({left}) than minimum integer value", Error.IntegerUnderflow));
            }

            if (right > int.MaxValue)
            {
                result.Add(new DiagnosticResult(Severity.Error, $"Y coordinate is larger ({right}) than maximum integer value", Error.IntegerOverflow));
            }
            else if (right < int.MinValue)
            {
                result.Add(new DiagnosticResult(Severity.Error, $"Y coordinate is smaller ({right}) than minimum integer value", Error.IntegerUnderflow));
            }

            return result;
        }

        protected override IEnumerable<DiagnosticResult> AggregateResult(IEnumerable<DiagnosticResult> aggregate, IEnumerable<DiagnosticResult> nextResult)
        {
            return (aggregate, nextResult) switch
            {
                (null, null) => Enumerable.Empty<DiagnosticResult>(),
                (_, null) => aggregate,
                (null, _) => nextResult,
                (_, _) => aggregate.Concat(nextResult)
            };
        }
    }
}