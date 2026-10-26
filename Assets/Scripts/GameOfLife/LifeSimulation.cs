using System;
using System.Collections.Generic;

namespace GameOfLife
{
    public static class LifeSimulation
    {
        public static HashSet<Cell> Run(IEnumerable<Cell> initialCells, int generations)
        {
            if (initialCells == null) throw new ArgumentNullException(nameof(initialCells));
            if (generations < 0) throw new ArgumentOutOfRangeException(nameof(generations));

            var liveCells = new HashSet<Cell>(initialCells);
            for (int generation = 0; generation < generations; generation++)
                liveCells = ComputeNextGeneration(liveCells);

            return liveCells;
        }

        public static HashSet<Cell> ComputeNextGeneration(HashSet<Cell> liveCells)
        {
            if (liveCells == null) 
                throw new ArgumentNullException(nameof(liveCells));

            var neighbourCounts = CountNeighbours(liveCells);
            var nextGeneration = new HashSet<Cell>();

            foreach (KeyValuePair<Cell, byte> entry in neighbourCounts)
            {
                if (entry.Value == 3 || (entry.Value == 2 && liveCells.Contains(entry.Key)))
                {
                    nextGeneration.Add(entry.Key);
                }
            }

            return nextGeneration;
        }

        private static Dictionary<Cell, byte> CountNeighbours(HashSet<Cell> liveCells)
        {
            var counts = new Dictionary<Cell, byte>();

            foreach (Cell cell in liveCells)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (!CanAddOffset(cell.X, dx)) 
                        continue;

                    AddNeighbourCountsForCell(dx, cell, counts);
                }
            }

            return counts;
        }

        private static void AddNeighbourCountsForCell(int dx, Cell cell, Dictionary<Cell, byte> counts)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) 
                    continue;
                        
                if (!CanAddOffset(cell.Y, dy)) 
                    continue;

                var neighbour = new Cell(cell.X + dx, cell.Y + dy);
                counts.TryGetValue(neighbour, out byte count);
                counts[neighbour] = (byte)(count + 1);
            }
        }

        private static bool CanAddOffset(long value, int offset) =>
            !(offset < 0 && value == long.MinValue) &&
            !(offset > 0 && value == long.MaxValue);
    }
}
