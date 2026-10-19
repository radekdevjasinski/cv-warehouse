namespace CvWarehouse.Core.Effects
{
    public readonly struct PointsPopup
    {
        public PointsPopup(float x, float y, int points, float ageSeconds)
        {
            X = x;
            Y = y;
            Points = points;
            AgeSeconds = ageSeconds;
        }

        public float X { get; }

        public float Y { get; }

        public int Points { get; }

        public float AgeSeconds { get; }

        public PointsPopup Aged(float seconds)
        {
            return new PointsPopup(X, Y, Points, AgeSeconds + seconds);
        }
    }
}
