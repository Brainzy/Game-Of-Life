using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GameOfLife;

namespace Tests.Editor
{
    public sealed class LifeRegressionChecks
    {
        private const int Generations = 10;
        private const string LifeHeader = "#Life 1.06";
        private const long DistantBlockMax = -2_000_000_000_000;
        private const long DistantBlockMin = DistantBlockMax - 1;

        private static HashSet<Cell> Cells(params (long X, long Y)[] coordinates)
        {
            var result = new HashSet<Cell>();

            foreach (var coordinate in coordinates)
                result.Add(new Cell(coordinate.X, coordinate.Y));

            return result;
        }

        private static HashSet<Cell> BlockAt(long x, long y) => Cells(
            (x, y), (x, y + 1), (x + 1, y), (x + 1, y + 1));

        private static string CoordinateLine(long x, long y) =>
            string.Format(CultureInfo.InvariantCulture, "{0} {1}", x, y);

        private static string LifeText(params string[] coordinateLines) =>
            LifeHeader + "\n" + string.Join("\n", coordinateLines) + "\n";

        private static void Check(bool condition, string name)
        {
            if (!condition) throw new Exception(name);
        }

        private static void Equal(HashSet<Cell> expected, HashSet<Cell> actual, string name)
            => Check(expected.SetEquals(actual), name);

        public void PatternsEvolve()
        {
            Equal(Cells(), LifeSimulation.Run(Cells(), Generations), "Empty universe");
            Equal(Cells(), LifeSimulation.ComputeNextGeneration(Cells((0, 0))),
                "Isolated cell dies");

            var block = BlockAt(0, 0);
            Equal(block, LifeSimulation.Run(block, Generations), "Block stays alive");

            var blinker = Cells((-1, 0), (0, 0), (1, 0));
            Equal(Cells((0, -1), (0, 0), (0, 1)),
                LifeSimulation.ComputeNextGeneration(blinker), "Blinker rotates");
            Equal(blinker, LifeSimulation.Run(blinker, Generations), "Blinker returns after 10 steps");

            var glider = Cells((0, 1), (1, 2), (2, 0), (2, 1), (2, 2));
            Equal(Cells((1, 2), (2, 3), (3, 1), (3, 2), (3, 3)),
                LifeSimulation.Run(glider, 4), "Glider translates");
        }

        public void LongLimitBlocksSurvive()
        {
            foreach (long x in new[] { long.MinValue, long.MaxValue - 1 })
            {
                foreach (long y in new[] { long.MinValue, long.MaxValue - 1 })
                {
                    var block = BlockAt(x, y);
                    Equal(block, LifeSimulation.Run(block, Generations), "Block at 64-bit limits stays alive");
                }
            }
        }

        public void EdgesDoNotWrap()
        {
            Equal(
                Cells(),
                LifeSimulation.ComputeNextGeneration(
                    Cells((long.MinValue, 0), (long.MaxValue, 0), (long.MaxValue, 1))),
                "X edges do not wrap");

            Equal(
                Cells(),
                LifeSimulation.ComputeNextGeneration(
                    Cells((0, long.MinValue), (0, long.MaxValue), (1, long.MaxValue))),
                "Y edges do not wrap");
        }

        public void FileParsingAndSorting()
        {
            var cells = LifeFile.Read(new StringReader(LifeText(
                "", "1 0\t", "-1 0", "0 1", "1 0", "1 -1",
                CoordinateLine(long.MaxValue, long.MinValue),
                CoordinateLine(long.MinValue, long.MaxValue))));
            Equal(Cells((-1, 0), (0, 1), (1, 0), (1, -1),
                    (long.MaxValue, long.MinValue), (long.MinValue, long.MaxValue)),
                cells, "64-bit coordinates, whitespace and duplicates");

            var writer = new StringWriter { NewLine = "\n" };
            LifeFile.Write(writer, cells);
            Check(writer.ToString() == LifeText(
                CoordinateLine(long.MinValue, long.MaxValue), "-1 0", "0 1", "1 -1", "1 0",
                CoordinateLine(long.MaxValue, long.MinValue)),
                "Output sorted by x then y across full 64-bit range");

            foreach (string invalid in new[] { "", "#Life 1.05\n0 0", "#Life 1.06\n0",
                         "#Life 1.06\n0 0 0", "#Life 1.06\nx 0",
                         "#Life 1.06\n9223372036854775808 0", "#Life 1.06\n0 -9223372036854775809" })
            {
                bool rejected = false;
                try { LifeFile.Read(new StringReader(invalid)); }
                catch (FormatException) { rejected = true; }
                Check(rejected, "Invalid input rejected");
            }
        }

        public void SampleRunsTenGenerations()
        {
            string directory = Path.Combine(Path.GetTempPath(), "GameOfLifeTests-" + Guid.NewGuid());
            Directory.CreateDirectory(directory);
            string inputPath = Path.Combine(directory, "input.txt");
            string outputPath = Path.Combine(directory, "output.txt");
            try
            {
                File.WriteAllText(inputPath, LifeText(
                    "0 1", "1 2", "2 0", "2 1", "2 2",
                    CoordinateLine(DistantBlockMax, DistantBlockMax),
                    CoordinateLine(DistantBlockMin, DistantBlockMin),
                    CoordinateLine(DistantBlockMax, DistantBlockMin)));
                LifeFile.RunFile(inputPath, outputPath);

                using (var reader = File.OpenText(outputPath))
                    Equal(Cells(
                            (DistantBlockMin, DistantBlockMin),
                            (DistantBlockMin, DistantBlockMax),
                            (DistantBlockMax, DistantBlockMin),
                            (DistantBlockMax, DistantBlockMax),
                            (3, 4), (4, 2), (4, 4), (5, 3), (5, 4)),
                        LifeFile.Read(reader), "Sample after 10 steps");
            }
            finally
            {
                File.Delete(inputPath);
                File.Delete(outputPath);
                Directory.Delete(directory);
            }
        }
    }
}
