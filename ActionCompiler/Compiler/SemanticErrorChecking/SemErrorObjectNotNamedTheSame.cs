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
            IEnumerable<MapNode> mapNodes = file.nodes.OfType<MapNode>();
            IEnumerable<SectionNode> sectionNodes = file.nodes.OfType<SectionNode>();
            IEnumerable<EntityNode> entityNodes = file.nodes.OfType<EntityNode>();

            // TODO: Game Node is missing

            HashSet<string> identifiers = new HashSet<string>();

            foreach (var node in file.nodes)
            {
                switch (node.GetType().Name)
                {
                    case nameof(MapNode):
                        var map = (MapNode)node;
                        if (!identifiers.Add(map.identifier.identifier))
                        {
                            yield return new DiagnosticResult(Severity.Error, $"The identifier {map.identifier.identifier} has already been defined!");
                        }
                        break;
                    case nameof(SectionNode):
                        var section = (SectionNode)node;
                        if (section.identifier is null)
                        {
                            continue;
                        }
                        if (!identifiers.Add(section.identifier.identifier))
                        {
                            yield return new DiagnosticResult(Severity.Error, $"The identifier {section.identifier.identifier} has already been defined!");
                        }
                        break;
                    case nameof(EntityNode):
                        var entity = (EntityNode)node;
                        if (!identifiers.Add(entity.identifier.identifier))
                        {
                            yield return new DiagnosticResult(Severity.Error, $"The identifier {entity.identifier.identifier} has already been defined!");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
