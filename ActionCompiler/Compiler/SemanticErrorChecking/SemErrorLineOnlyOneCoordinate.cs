using Action.AST;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorLineOnlyOneCoordinate : NodeVisitor<IEnumerable<DiagnosticResult>>
    {

        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            foreach (var node in nodes.nodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode){
            return CheckLine(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            return CheckLine(sectionNode);
        }

        public IEnumerable<DiagnosticResult> CheckLine(ComplexNode node){
            foreach (var property in node.properties){
                if (property.identifier is ShapeKeywordNode){
                    ComplexNode val = (ComplexNode)property.value;
                    if (val.type is LineKeywordNode){
                        var numOfCoords = val.values.Count();
                        if (numOfCoords == 1){
                            yield return new DiagnosticResult(Severity.Error, "cannot have a line node with only a single point");
                        }  
                    }
                }
            }
            if(node.values.Any()){
                foreach(var sec in node.values.OfType<SectionNode>()){
                    foreach(var val in Visit(sec)){
                        yield return val;
                    }
                }
            }   
        }
    }
}