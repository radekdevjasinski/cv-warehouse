namespace CvWarehouse.Core.Effects
{
    public sealed class PointsPopupSettings
    {
        public float DurationSeconds { get; set; } = 1.2f;

        public float RiseDistance { get; set; } = 1f;

        public float FadeStartFraction { get; set; } = 0.5f;

        public float OverlapWidth { get; set; } = 1.5f;

        public float LineHeight { get; set; } = 0.6f;
    }
}
