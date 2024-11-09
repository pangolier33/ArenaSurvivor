using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bones.Gameplay.HashGrids
{
	public static class ArrayExtensions
    {
        /*
        public static void ShiftHorizontally<T>(this T[][] grid, int distance)
        {
            var height = grid.Length;
            if (height == 0)
                return;
            
            var width = grid[0].Length;
            if (width == 0)
                return;
            
            switch (distance)
            {
                case 0:
                    return;
                case > 0:
                    if (distance >= width)
                    {
                        DisposeAll();
                        return;
                    }

                    for (var r = 0; r < height; r++)
                    {
                        for (var c = width - distance; c < width; c++)
                            DisposeBucket(grid[r][c]);

                        Array.Copy(grid[r], 0, grid[r], distance, height - distance);
                        for (var c = 0; c < distance; c++)
                            grid[r][c] = default;
                    }
                    return;
                case < 0:
                    if (distance <= -width)
                    {
                        DisposeAll();
                        return;
                    }

                    for (var r = 0; r < height; r++)
                    {
                        for (var c = 0; c < -distance; c++)
                            DisposeBucket(grid[r][c]);

                        Array.Copy(grid[r], -distance, grid[r], 0, height + distance);
                        for (var c = width + distance; c < width; c++)
                            grid[r][c] = default;
                    }
                    return;
            }
        }

        public static void ShiftVertically<T>(this T[][] grid, int distance)
        {
            var height = grid.Length;
            if (height == 0)
                return;
            
            var width = grid[0].Length;
            if (width == 0)
                return;
            
            switch (distance)
            {
                case 0:
                    return;
                case > 0:
                    if (distance >= height)
                    {
                        DisposeAll();
                        return;
                    }

                    for (var row = height - distance; row < height; row++)
                        DisposeRow(row);
                    Array.Copy(grid, 0, grid, distance, height - distance);
                    for (var row = 0; row < distance; row++)
                        grid[row] = null;
                    return;

                case < 0:
                    if (distance <= -height)
                    {
                        DisposeAll();
                        return;
                    }

                    for (var r = 0; r < -distance; r++)
                        DisposeRow(r);
                    Array.Copy(grid, -distance, grid, 0, height + distance);
                    for (var row = height + distance; row < height; row++)
                        grid[row] = null;
                    return;
            }
        }
        */
        
        public static string Print<T>(this T[][] grid)
        {
            var height = grid.Length;
            if (height == 0)
                return "";
            var width = grid[0].Length;
            if (width == 0)
                return "";
            var sb = new StringBuilder();
            for (var y = height - 1; y > -1; y--)
            {
                for (var x = 0; x < width; x++)
                {
                    sb.Append(grid[y]?[x]?.ToString() ?? "*");
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }
        
        public static IEnumerable<T> SpiralTraverse<T>(T[][] grid, int x, int width, int y, int height, int maxDepth = int.MaxValue)
            => grid.TraverseRings(x, width, y, height, maxDepth).SelectMany(item => item);

        private static IEnumerable<IEnumerable<T>> TraverseRings<T>(this T[][] grid, int x, int width, int y, int height, int maxDepth)
        {
            if (grid[y] != null && grid[y][x] != null)
                yield return new[] { grid[y][x] };

            maxDepth = Math.Min(maxDepth, Math.Max(Math.Max(width - x, x), Math.Max(height - y, y)));
            for (var depth = 1; depth < maxDepth; depth++)
                yield return TraverseRing(grid, x, width, y, height, depth);
        }

        private static IEnumerable<T> TraverseRing<T>(this T[][] grid, int x, int width, int y, int height, int depth)
        {
            var l = x - depth;
            var r = x + depth;
            var b = y - depth;
            var t = y + depth;

            if (t < height && grid[t] != null)
                for (x = Math.Max(l, 0); x <= Math.Min(r, width - 1); x++)
                    if (grid[t][x] != null)
                        yield return grid[t][x];
            if (b > -1 && grid[b] != null)
                for (x = Math.Max(l, 0); x <= Math.Min(r, width - 1); x++)
                    if (grid[b][x] != null)
                        yield return grid[b][x];
	
            if (r < width)
                for (y = Math.Max(b + 1, 0); y < Math.Min(t, height); y++)
                    if (grid[y] != null && grid[y][r] != null)
                        yield return grid[y][r];
            if (l > -1)
                for (y = Math.Max(b + 1, 0); y < Math.Min(t, height); y++)
                    if (grid[y] != null && grid[y][l] != null)
                        yield return grid[y][l];
        }
    }
}