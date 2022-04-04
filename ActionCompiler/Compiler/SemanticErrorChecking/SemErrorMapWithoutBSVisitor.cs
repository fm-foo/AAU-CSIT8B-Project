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
            var combinedNodes = nodes.nodes.Where(n => n.GetType() == typeof(MapNode) || n.GetType() == typeof(SectionNode));

            foreach (var node in combinedNodes)
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
        }
    }
}