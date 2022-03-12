using Action.AST;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorLineOnlyOneCoordinate : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitComplex(ComplexNode complexNode)
        {
            if (complexNode.type is not LineKeywordNode)
                yield break;
            var numOfCoords = complexNode.values.Count();
            if (numOfCoords == 1)
                yield return new DiagnosticResult(Severity.Error, "cannot have a line node with only a single point");
        }
    }

    public class SELOOCTests
    {
        private readonly SemErrorLineOnlyOneCoordinate selooc;

        public SELOOCTests()
        {
            this.selooc = new SemErrorLineOnlyOneCoordinate();
        }

        public void SuccessTest()
        {
            var coord1 = new CoordinateNode(new IntNode(0), new IntNode(0));
            var coord2 = new CoordinateNode(new IntNode(5), new IntNode(3));
            var coords = new[] { coord1, coord2 };
            var line = new ComplexNode(new LineKeywordNode(), Enumerable.Empty<PropertyNode>(), coords);

            var result = selooc.Visit(line);
            Debug.Assert(!result.Any());
        }
        
        public void Success2Test()
        {
            var coord1 = new CoordinateNode(new IntNode(0), new IntNode(0));
            var coords = new[] { coord1 };
            var line = new ComplexNode(new CoordinatesKeywordNode(), Enumerable.Empty<PropertyNode>(), coords);

            var result = selooc.Visit(line);
            Debug.Assert(!result.Any());
        }

        public void FailureTest()
        {
            var coord1 = new CoordinateNode(new IntNode(0), new IntNode(0));
            var coords = new[] { coord1 };
            var line = new ComplexNode(new LineKeywordNode(), Enumerable.Empty<PropertyNode>(), coords);

            var result = selooc.Visit(line);
            Debug.Assert(result.Any());

            var error = result.Single();
            Debug.Assert(error.severity == Severity.Error);
            Debug.Assert(error.message == "cannot have a line node with only a single point");
        }
    }
}