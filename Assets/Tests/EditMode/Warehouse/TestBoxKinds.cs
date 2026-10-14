using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public static class TestBoxKinds
    {
        public static readonly BoxKind Small = Of(BoxSizeClass.Small);
        public static readonly BoxKind Medium = Of(BoxSizeClass.Medium);
        public static readonly BoxKind Large = Of(BoxSizeClass.Large);

        public static BoxKind Of(BoxSizeClass sizeClass)
        {
            return new BoxKind(sizeClass, Importance.Normal);
        }

        public static BoxKind Of(Importance importance)
        {
            return new BoxKind(BoxSizeClass.Small, importance);
        }
    }
}
