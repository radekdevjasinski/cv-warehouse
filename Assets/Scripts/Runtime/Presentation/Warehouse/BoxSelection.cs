using CvWarehouse.Core.Warehouse;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BoxSelection
    {
        private Box box;

        public bool HasBox => box != null && !box.IsEmpty;

        public Box SelectedBox => HasBox ? box : null;

        public void Select(Box selectedBox)
        {
            box = selectedBox;
        }

        public void Clear()
        {
            box = null;
        }
    }
}
