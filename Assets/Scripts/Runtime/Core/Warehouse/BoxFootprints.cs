using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public static class BoxFootprints
    {
        public static GridSize For(WeightClass weightClass)
        {
            switch (weightClass)
            {
                case WeightClass.Heavy:
                    return new GridSize(2, 2);
                case WeightClass.Medium:
                    return new GridSize(2, 1);
                default:
                    return new GridSize(1, 1);
            }
        }
    }
}
