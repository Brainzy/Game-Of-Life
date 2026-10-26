using System;

namespace GameOfLife
{
    public readonly struct Cell : IEquatable<Cell>, IComparable<Cell>
    {
        public long X { get; }
        public long Y { get; }

        public Cell(long x, long y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Cell other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Cell other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public int CompareTo(Cell other)
        {
            int xOrder = X.CompareTo(other.X);
            return xOrder != 0 ? xOrder : Y.CompareTo(other.Y);
        }
    }
}
