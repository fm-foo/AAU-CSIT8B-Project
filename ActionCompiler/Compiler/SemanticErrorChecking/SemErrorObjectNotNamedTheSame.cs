using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    internal class SemErrorObjectNotNamedTheSame : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            HashSet<string> identifiers = new HashSet<string>();

            foreach (var node in file.nodes)
            {
                switch (node)
                {
                    case MapNode mapNode:
                        if (!identifiers.Add(mapNode.identifier.identifier))
                        {
                            yield return new DiagnosticResult(Severity.Error, $"The identifier {mapNode.identifier.identifier} has already been defined!");
                        }
                        break;
                    case SectionNode sectionNode:
                        if (sectionNode.identifier is null)
                        {
                            continue;
                        }
                        if (!identifiers.Add(sectionNode.identifier.identifier))
                        {
                            yield return new DiagnosticResult(Severity.Error, $"The identifier {sectionNode.identifier.identifier} has already been defined!", Error.IdentifierAlreadyDefined);
                        }
                        break;
                    case EntityNode entityNode:
                        if (!identifiers.Add(entityNode.identifier.identifier))
                        {
                            yield return new DiagnosticResult(Severity.Error, $"The identifier {entityNode.identifier.identifier} has already been defined!", Error.IdentifierAlreadyDefined);
                        }
                        break;
                    case GameNode gameNode:
                        if (!identifiers.Add(gameNode.identifier.identifier))
                        {
                            yield return new DiagnosticResult(Severity.Error, $"The identifier {gameNode.identifier.identifier} has already been defined!", Error.IdentifierAlreadyDefined);
                        }
                        break;
                    default:
                        throw new Exception($"Unknown type: {node.GetType()}");
                }
            }
        }
    }
}
