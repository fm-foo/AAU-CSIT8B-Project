using Action.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorEntitiesFunctionVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            var combinedNodes = nodes.nodes.Where(n => n.GetType() == typeof(EntityNode));

            foreach (var node in combinedNodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitEntity(EntityNode entity){
            List<string> func = new List<string>();

            foreach(var fun in entity.funcDecs){
                func.Add(fun.identifier.identifier);
            }
            if(!func.Contains("act")){
                yield return new DiagnosticResult(Severity.Error, "The act function is missing");
            }if(!func.Contains("create")){
                yield return new DiagnosticResult(Severity.Error, "The create function is missing");
            }if(!func.Contains("destroy")){
                yield return new DiagnosticResult(Severity.Error, "The destroy function is missing");
            }
        }
    }
}