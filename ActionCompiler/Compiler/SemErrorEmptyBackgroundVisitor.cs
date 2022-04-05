using Action.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorEmptyBackgroundVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
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
            return CheckColour(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            return CheckColour(sectionNode);
        }

        public IEnumerable<DiagnosticResult> CheckColour(ComplexNode node){

            foreach(var property in node.properties){
                if(property.identifier is BackgroundKeywordNode){
                    ComplexNode val = (ComplexNode)property.value;
                    if(val.type is ColourKeywordNode){
                        if(!val.properties.Any()){
                            yield return new DiagnosticResult(Severity.Error, "missing colour value");
                        }
                    }
                    if(val.type is ImageKeywordNode){
                        if(!val.properties.Any()){
                            yield return new DiagnosticResult(Severity.Error, "missing image path");
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