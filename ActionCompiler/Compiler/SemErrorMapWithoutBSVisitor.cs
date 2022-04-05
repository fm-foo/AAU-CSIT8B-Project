using Action.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorMapWithoutBSVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
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
            return CheckProperties(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            return CheckProperties(sectionNode);
        }

        public IEnumerable<DiagnosticResult> CheckProperties(ComplexNode node){

            var testBack = false;
            var testShape = false;

            foreach(var property in node.properties){
                if(property.identifier is BackgroundKeywordNode){
                    testBack = true;
                }
                if(property.identifier is ShapeKeywordNode){
                    testShape = true;
                }
            }
            if(testBack == false){
                yield return new DiagnosticResult(Severity.Error, "missing background property");
            }
            if(testShape == false){
                yield return new DiagnosticResult(Severity.Error, "missing shape property");
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