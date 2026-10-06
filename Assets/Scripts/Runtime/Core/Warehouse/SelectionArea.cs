using System;

namespace CvWarehouse.Core.Warehouse
{
    public readonly struct SelectionArea
    {
        public SelectionArea(float cornerX, float cornerY, float oppositeX, float oppositeY)
        {
            MinX = Math.Min(cornerX, oppositeX);
            MinY = Math.Min(cornerY, oppositeY);
            MaxX = Math.Max(cornerX, oppositeX);
            MaxY = Math.Max(cornerY, oppositeY);
        }

        public float MinX { get; }

        public float MinY { get; }

        public float MaxX { get; }

        public float MaxY { get; }

        public float Width => MaxX - MinX;

        public float Height => MaxY - MinY;

        public bool Contains(float pointX, float pointY)
        {
            return pointX >= MinX && pointX <= MaxX && pointY >= MinY && pointY <= MaxY;
        }
    }
}
