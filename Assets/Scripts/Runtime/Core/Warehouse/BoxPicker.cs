using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public static class BoxPicker
    {
        public static bool TryPick(IReadOnlyList<Box> boxes, float pointX, float pointY, out Box picked)
        {
            for (int index = 0; index < boxes.Count; index++)
            {
                Box box = boxes[index];
                if (box.IsEmpty || !Covers(box, pointX, pointY))
                    continue;

                picked = box;
                return true;
            }

            picked = null;
            return false;
        }

        private static bool Covers(Box box, float pointX, float pointY)
        {
            return pointX >= box.Origin.X && pointX < box.Origin.X + box.Size.Width
                && pointY >= box.Origin.Y && pointY < box.Origin.Y + box.Size.Height;
        }
    }
}
