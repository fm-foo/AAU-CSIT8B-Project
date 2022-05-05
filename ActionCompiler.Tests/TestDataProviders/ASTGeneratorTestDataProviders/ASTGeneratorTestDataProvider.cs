using Action.AST;
using ActionCompiler.AST;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.UnitTests.TestDataProviders.ASTGeneratorTestDataProviders
{
    internal static class ASTGeneratorTestDataProvider
    {
        public static IEnumerable<object[]> GetMapTestData()
        {
            yield return new object[]
            {
                new ASTGeneratorTestData
                (
                 @"
                    map BasicMap {};
                 ",
                  new FileNode
                  (
                    new List<ValueNode>
                    {
                        new MapNode
                        (
                            new IdentifierNode("BasicMap"), 
                            Enumerable.Empty<PropertyNode>(), 
                            Enumerable.Empty<ValueNode>()
                        )
                    }    
                  ) 
                )
            };
            yield return new object[]
            {
                new ASTGeneratorTestData(
                    @"
                    map BasicMap {
                            background: colour { hex: #123456; };
                    };
                    ",
                    new FileNode
                    (
                        new List<ValueNode>
                        {
                            new MapNode
                            (
                                new IdentifierNode("BasicMap"),
                                new List<PropertyNode>
                                {
                                    new PropertyNode
                                    (
                                        new BackgroundKeywordNode(),
                                        new ComplexNode
                                        (
                                            new ColourKeywordNode(),
                                            new List<PropertyNode>
                                            {
                                                new PropertyNode(new HexKeywordNode(), new ColourNode(18, 52, 86))
                                            },
                                            Enumerable.Empty<ValueNode>()
                                        )
                                    )
                                },
                               Enumerable.Empty<ValueNode>()
                            )
                        }
                    )
                )
            };
            yield return new object[]
            {
                new ASTGeneratorTestData
                (
                    @"
                    map BasicMap {
                        background: colour { hex: #123456; };
                            shape: line {
                                (0, 0);
                                (4, -4);
                        };
                    };
                    ",
                    new FileNode
                    (
                        new List<ValueNode>
                        {
                            new MapNode
                            (
                                new IdentifierNode("BasicMap"),
                                new List<PropertyNode>
                                {
                                    new PropertyNode
                                    (
                                        new BackgroundKeywordNode(),
                                        new ComplexNode
                                        (
                                            new ColourKeywordNode(),
                                            new List<PropertyNode>
                                            {
                                                new PropertyNode(new HexKeywordNode(), new ColourNode(18, 52, 86))
                                            },
                                            Enumerable.Empty<ValueNode>()
                                    )
                                    ),
                                    new PropertyNode
                                    (
                                        new ShapeKeywordNode(),
                                        new ComplexNode
                                        (
                                            new LineKeywordNode(),
                                            Enumerable.Empty<PropertyNode>(),
                                            new List<ValueNode>
                                            {
                                                new CoordinateNode(new IntNode(0), new IntNode(0)),
                                                new CoordinateNode(new IntNode(4), new IntNode(-4))
                                            }
                                        )
                                    )
                                },
                                Enumerable.Empty<ValueNode>()
                            )
                        }
                    )
                )
            };
        }

    }
}
