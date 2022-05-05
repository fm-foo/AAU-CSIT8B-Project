using ActionCompiler.Compiler;
using System.Collections.Generic;

public record IntegrationTestData(string File, IEnumerable<DiagnosticResult> Diagnostics);
