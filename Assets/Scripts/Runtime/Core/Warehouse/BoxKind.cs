using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public readonly struct BoxKind
    {
        public BoxKind(BoxSizeClass sizeClass, Importance importance)
        {
            SizeClass = sizeClass;
            Importance = importance;
        }

        public BoxSizeClass SizeClass { get; }

        public Importance Importance { get; }
    }
}
