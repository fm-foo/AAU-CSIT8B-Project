using Action.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorSectionOffMapVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public IEnumerable<DiagnosticResult> Visit(List<ComplexNode> nodes)
        {
            foreach (var node in nodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode){
            GetCoord(mapNode);
            return CheckLimit(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            return CheckLimit(sectionNode);
        }

        public IEnumerable<PropertyNode> GetCoord(ComplexNode node){
            foreach(var property in node.properties){
                if(property.identifier is ShapeKeywordNode){
                    ComplexNode val = (ComplexNode)property.value;
                    if(val.type is BoxKeywordNode){
                        yield break;
                    }
                }
            }
        }

        public IEnumerable<DiagnosticResult> CheckLimit(ComplexNode node){

            foreach(var property in node.properties){
                if(property.identifier is ShapeKeywordNode){
                    ComplexNode val = (ComplexNode)property.value;
                    if(val.type is BoxKeywordNode){
                        if(!val.properties.Any()){
                            yield return new DiagnosticResult(Severity.Error, "missing height and/or width of the box");
                        }
                    }
                }
            }
        }
    }
}