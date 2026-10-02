namespace CvWarehouse.Core.Warehouse
{
    public readonly struct GridSize
    {
        public GridSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }
    }
}
