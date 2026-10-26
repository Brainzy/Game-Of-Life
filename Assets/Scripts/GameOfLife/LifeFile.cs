using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace GameOfLife
{
    public static class LifeFile
    {
        private const string Header = "#Life 1.06";
        private const int RequiredGenerations = 10;

        public static HashSet<Cell> Read(TextReader reader)
        {
            if (reader == null) 
                throw new ArgumentNullException(nameof(reader));
            
            if (reader.ReadLine()?.Trim() != Header)
                throw new FormatException("Line 1: expected #Life 1.06 header.");

            var cells = new HashSet<Cell>();
            int lineNumber = 1;
            while (reader.ReadLine() is { } line)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] coordinates = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (coordinates.Length != 2 ||
                    !long.TryParse(coordinates[0], NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture, out long x) ||
                    !long.TryParse(coordinates[1], NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture, out long y))
                    throw new FormatException($"Line {lineNumber}: expected two signed 64-bit integers.");

                cells.Add(new Cell(x, y));
            }

            return cells;
        }

        public static void Write(TextWriter writer, IEnumerable<Cell> cells)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (cells == null) throw new ArgumentNullException(nameof(cells));

            var sorted = new List<Cell>(new HashSet<Cell>(cells));
            sorted.Sort();
            writer.WriteLine(Header);
            foreach (Cell cell in sorted)
            {
                writer.Write(cell.X.ToString(CultureInfo.InvariantCulture));
                writer.Write(' ');
                writer.WriteLine(cell.Y.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static int RunFile(string inputPath, string outputPath)
        {
            HashSet<Cell> cells;
            using (var reader = new StreamReader(inputPath))
                cells = Read(reader);

            cells = LifeSimulation.Run(cells, RequiredGenerations);
            using (var writer = new StreamWriter(outputPath, false, new UTF8Encoding(false)))
                Write(writer, cells);

            return cells.Count;
        }
    }
}
