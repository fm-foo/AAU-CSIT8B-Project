namespace Action.AST
{
    public record KeywordNode(string keyword) : IdentifierNode(keyword);
    public record MapKeywordNode() : KeywordNode("map");
    public record SectionKeywordNode() : KeywordNode("section");
    public record ReferenceKeywordNode() : KeywordNode("reference");
    public record BackgroundKeywordNode() : KeywordNode("background");
    public record ColourKeywordNode() : KeywordNode("colour");
    public record BoxKeywordNode() : KeywordNode("box");
    public record ImageKeywordNode() : KeywordNode("image");
    public record CoordinatesKeywordNode() : KeywordNode("coordinates");
    public record LineKeywordNode() : KeywordNode("line");
    public record HexKeywordNode() : KeywordNode("hex");
    public record HeightKeywordNode() : KeywordNode("height");
    public record WidthKeywordNode() : KeywordNode("width");
    public record PathKeywordNode() : KeywordNode("path");
    public record PointKeywordNode() : KeywordNode("point");
    public record ShapeKeywordNode() : KeywordNode("shape");
}