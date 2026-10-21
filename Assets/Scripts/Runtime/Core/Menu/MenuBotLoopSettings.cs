namespace CvWarehouse.Core.Menu
{
    public sealed class MenuBotLoopSettings
    {
        public float TakeOutSeconds { get; set; } = 0.5f;

        public float WalkSeconds { get; set; } = 1.6f;

        public float PutInSeconds { get; set; } = 0.35f;

        public int[] CargoWordsPerTrip { get; set; } = { 1, 2, 3, 2 };
    }
}
