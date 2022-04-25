namespace Action.Compiler
{
    public record DiagnosticResult(Severity severity, string message, Error error = Error.Unspecified);

    public enum Error
    {
        Unspecified,
        MultipleProperties,
        IntegerOverflow,
        IntegerUnderflow,
        FloatOverflow,
        FloatUnderflow,
        InvalidAssignment,
        MultipleDeclaration,
        IdentifierAlreadyDefined,
        InvalidLoneStatement,
        InvalidDeclarationInEmbeddedStatement,
        StandaloneSectionWithCoordinates,
        MissingBackgroundColorValue,
        MissingBackgroundImagePathValue,
        MissingBoxWidthHeight,
        CoordinatesOffMap
    }
}