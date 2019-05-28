namespace SpaceBattle.Data
{
    public struct Location
    {
        public readonly int Y;
        public readonly int X;

        public Location(int y, int x)
        {
            Y = y;
            X = x;
        }

        public static Location operator +(Location l1, Location l2) => new Location(l1.Y + l2.Y, l1.X + l2.X);

        public static Location operator *(Location l, int i) => new Location(l.Y * i, l.X * i);

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Location l)
                return Y == l.Y && X == l.X;
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Y * 397) ^ X;
            }
        }
    }
}