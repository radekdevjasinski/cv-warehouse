namespace CvWarehouse.Presentation.Warehouse
{
    public readonly struct CameraZoomLimits
    {
        public CameraZoomLimits(int spritePixelsPerUnit, float closestViewCells, float farthestViewCells)
        {
            SpritePixelsPerUnit = spritePixelsPerUnit;
            ClosestViewCells = closestViewCells;
            FarthestViewCells = farthestViewCells;
        }

        public int SpritePixelsPerUnit { get; }

        public float ClosestViewCells { get; }

        public float FarthestViewCells { get; }
    }
}
