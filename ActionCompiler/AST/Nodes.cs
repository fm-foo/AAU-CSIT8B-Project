using System.Collections.Generic;

namespace Action.AST
{
    public record SectionNode(
        CoordinateNode? coords,
        IdentifierNode? identifier,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> sections) : ComplexNode(new SectionKeywordNode(), properties, sections);

    public record MapNode(
        IdentifierNode identifier,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> sections) : ComplexNode(new MapKeywordNode(), properties, sections);

    // complex types
    public record ComplexNode(
        IdentifierNode type,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> values) : ValueNode;
    public record PropertyNode(IdentifierNode identifier, ValueNode value);

    public record ReferenceNode(IdentifierNode referenceType, IdentifierNode reference, CoordinateNode coords) : ValueNode;


    // primitive types
    public record ValueNode;
    public record StringNode(string s) : ValueNode;
    public record CoordinateNode(IntNode x, IntNode y) : ValueNode;
    public record IdentifierNode(string identifier) : ValueNode;
    public record IntNode(int i) : ValueNode;
    public record NatNumNode(uint i) : ValueNode;
    public record ColourNode(byte r, byte g, byte b) : ValueNode;

    // keywords

    public record MapKeywordNode() : IdentifierNode("map");
    public record SectionKeywordNode() : IdentifierNode("section");
    public record ReferenceKeywordNode() : IdentifierNode("reference");
    public record BackgroundKeywordNode() : IdentifierNode("background");
    public record ColourKeywordNode() : IdentifierNode("colour");
    public record BoxKeywordNode() : IdentifierNode("box");
    public record ImageKeywordNode() : IdentifierNode("image");
    public record CoordinatesKeywordNode() : IdentifierNode("coordinates");
    public record LineKeywordNode() : IdentifierNode("line");
    public record HexKeywordNode() : IdentifierNode("hex");
    public record HeightKeywordNode() : IdentifierNode("height");
    public record WidthKeywordNode() : IdentifierNode("width");
    public record PathKeywordNode() : IdentifierNode("path");
    public record PointKeywordNode() : IdentifierNode("point");
    public record ShapeKeywordNode() : IdentifierNode("shape");
}