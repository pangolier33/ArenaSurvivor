using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Base
{
    public sealed class ClustersGrid<TCluster, TEntity>
        where TCluster : class, ICollection<TEntity>, new()
    {
        private readonly TCluster[][] _grid;
        private readonly int _width;
        private readonly int _height;

        public ClustersGrid(Vector2Int size)
            : this(size.x, size.y) { }
        public ClustersGrid(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(_width), width, "Width must be positive");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(_height), height, "Height must be positive");
            
            _width = width;
            _height = height;
            
            _grid = new TCluster[height][];
        }

        public IEnumerable<TCluster> Traverse(int x, int y, int maxDepth = int.MaxValue)
        {
            return TraverseRings(_grid, x, _width, y, _height, maxDepth).SelectMany(item => item);
        }

        private static IEnumerable<IEnumerable<T>> TraverseRings<T>(T[][] grid, int x, int width, int y, int height, int maxDepth)
            where T : class
        {
            if (grid[y] != null && grid[y][x] != null)
                yield return new[] { grid[y][x] };

            maxDepth = Math.Min(maxDepth, Math.Max(Math.Max(width - x, x), Math.Max(height - y, y)));
            for (var depth = 1; depth < maxDepth; depth++)
                yield return TraverseRing(grid, x, width, y, height, depth);
        }

        private static IEnumerable<T> TraverseRing<T>(T[][] grid, int x, int width, int y, int height, int depth)
            where T : class
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
        
        public bool TryGet(int x, int y, out TCluster cluster)
        {
            if (IsInBounds(x, y))
            {
                var row = _grid[y] ??= new TCluster[_width];
                cluster = row[x] ??= new TCluster();
                return true;
            }

            cluster = default;
            return false;
        }

        public bool TryGet(Vector2Int indexes, out TCluster cluster)
        {
            return TryGet(indexes.x, indexes.y, out cluster);
        }

        public IEnumerable<TCluster> GetAll()
        {
            return _grid.Where(x => x != null)
                        .SelectMany(x => x)
                        .Where(x => x != null);
        }

        public void Shift(Vector2Int delta)
        {
            // dispose all clusters when delta is full-width or full-height
            if (delta.x >= _width || delta.y >= _height)
            {
                for (var y = 0; y < _height; y++)
                {
                    DisposeRow(y);
                    _grid[y] = null;
                }

                return;
            }

            // shifting y-axis
            switch (delta.y)
            {
                case > 0:
                    for (var y = _height - delta.y; y < _height; y++)
                    {
                        DisposeRow(y);
                    }

                    Array.Copy(_grid, 0, _grid, delta.y, _height - delta.y);
                    
                    for (var y = 0; y < delta.y; y++)
                    {
                        _grid[y] = null;
                    }
                    break;
                case < 0:
                    for (var y = 0; y < -delta.y; y++)
                    {
                        DisposeRow(y);
                    }

                    Array.Copy(_grid, -delta.y, _grid, 0, _height + delta.y);
                    
                    for (var y = _height + delta.y; y < _height; y++)
                    {
                        _grid[y] = null;
                    }
                    break;
            }

            switch (delta.x)
            {
                case > 0:
                    foreach (var row in _grid)
                    {
                        if (row == null)
                            continue;

                        for (var x = _width - delta.x; x < _width; x++)
                        {
                            var cluster = row[x];
                            if (cluster == null)
                                continue;
                            cluster.Clear();
                        }

                        Array.Copy(row, 0, row, delta.x, _height - delta.x);
                        
                        for (var x = 0; x < delta.x; x++)
                        {
                            row[x] = default;
                        }
                    }
                    break;
                case < 0:
                    foreach (var row in _grid)
                    {
                        if (row == null)
                            continue;

                        for (var x = 0; x < -delta.x; x++)
                        {
                            var cluster = row[x];
                            if (cluster == null)
                                continue;
                            cluster.Clear();
                        }

                        Array.Copy(row, -delta.x, row, 0, _height + delta.x);
                        
                        for (var x = 0; x < delta.x; x++)
                        {
                            row[x] = default;
                        }
                    }
                    break;
            }
        }

        private void DisposeRow(int y)
        {
            var row = _grid[y];
            if (row == null)
                return;

            foreach (var cluster in row)
            {
                if (cluster == null)
                    continue;
                cluster.Clear();
            }

            _grid[y] = null;
        }
        
        private bool IsInBounds(int x, int y)
        {
            return x > -1 &&
                   x < _width &&
                   y > -1 &&
                   y < _height;
        }
    }
}