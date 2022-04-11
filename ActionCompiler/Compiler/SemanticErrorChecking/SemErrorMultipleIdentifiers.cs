using Action.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorMultipleIdentifiers : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            foreach (var entity in file.nodes.Where(n => n is EntityNode || n is GameNode))
            {
                foreach (var result in Visit(entity))
                    yield return result;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitGame(GameNode gameNode)
        {
            return VisitEntityOrGame(gameNode);
        }

        public override IEnumerable<DiagnosticResult> VisitEntity(EntityNode entityNode)
        {
            return VisitEntityOrGame(entityNode);
        }

        public IEnumerable<DiagnosticResult> VisitEntityOrGame(ComplexNode node)
        {

        }
    }
}