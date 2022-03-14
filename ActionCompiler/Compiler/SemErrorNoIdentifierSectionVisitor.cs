using Action.AST;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorNoIdentifierSectionVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            IEnumerable<SectionNode> query1 = nodes.nodes.OfType<SectionNode>();
            foreach (var node in query1)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }
        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            if(sectionNode.identifier == null){
                yield return new DiagnosticResult(Severity.Error, "missing section identifier");
            } 
        }
    }
}