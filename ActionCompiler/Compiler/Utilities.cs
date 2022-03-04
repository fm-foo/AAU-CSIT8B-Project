using System;
using System.Collections.Generic;
using System.Linq;
using Action.AST;

namespace Action.Compiler
{
    public static class Utilities
    {
        public static U GetProperty<T, U>(this ComplexNode node)
            where T : KeywordNode
            where U : ValueNode
        {
            return node.properties
                .Where(n => n.identifier is T)
                .Select(n => n.value)
                .Cast<U>()
                .Single();
        }

        public static void Fill<T>(this T[,] array, T obj)
        {
            array.ForEach((x, y, _) => array[x, y] = obj);
        }

        public static void ForEach<T>(this T[,] array, Action<int, int, T> action)
        {
            for (int x = 0; x < array.GetLength(0); x++)
                for (int y = 0; y < array.GetLength(1); y++)
                    action(x, y, array[x, y]);
        }

        public static void CopyTo(this Tile[,] source, Tile[,] dest, int x, int y)
        {
            for (int source_x = 0; source_x < source.GetLength(0); source_x++)
            {
                for (int source_y = 0; source_y < source.GetLength(1); source_y++)
                {
                    int dest_x = source_x + x;
                    int dest_y = source_y + y;
                    Tile srctile = source[source_x, source_y];
                    if (dest_x < dest.GetLength(0) && dest_y < dest.GetLength(1) && srctile is not EmptyTile)
                    {
                        dest[dest_x, dest_y] = srctile; 
                    }
                }
            }
        }
    } 
}