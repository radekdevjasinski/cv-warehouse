namespace CvWarehouse.Core.Warehouse
{
    public static class BoxFootprints
    {
        public static GridSize For(BoxSizeClass sizeClass)
        {
            switch (sizeClass)
            {
                case BoxSizeClass.Large:
                    return new GridSize(2, 2);
                case BoxSizeClass.Medium:
                    return new GridSize(2, 1);
                default:
                    return new GridSize(1, 1);
            }
        }
    }
}
