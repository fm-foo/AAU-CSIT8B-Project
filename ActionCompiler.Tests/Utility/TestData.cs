using Action.Compiler;

public record TestData(string File, Diagnostic? Diagnostics = null);
public record Diagnostic(Severity Severity, Error Error);
