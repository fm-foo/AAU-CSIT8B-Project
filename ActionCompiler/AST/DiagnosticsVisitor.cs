using System;
using System.Collections.Generic;
using System.Linq;
using ActionCompiler.Compiler;

namespace ActionCompiler.AST
{
    public abstract class DiagnosticsVisitor : AutomaticNodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> Default => Array.Empty<DiagnosticResult>();
        public override IEnumerable<DiagnosticResult> MergeValues(IEnumerable<DiagnosticResult> oldValue, IEnumerable<DiagnosticResult> newValue)
        {
            return (oldValue, newValue) switch
            {
                // both are empty arrays (Default)
                (Array { Length: 0 }, Array { Length: 0 }) => Default,
                // oldValue is empty array 
                (Array { Length: 0 }, _) => newValue,
                // newValue is empty array
                (_, Array { Length: 0 }) => oldValue,
                // neither is empty array
                _ => oldValue.Concat(newValue)
            };
        }
    }
}