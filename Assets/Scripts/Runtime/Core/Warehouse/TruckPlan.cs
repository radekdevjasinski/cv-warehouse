using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class TruckPlan
    {
        public const int MinWidth = 3;

        private const float PixelsPerColumn = 16f;
        private const float FrontWheelCentre = 12f / PixelsPerColumn;
        private const float DriveWheelCentre = 36f / PixelsPerColumn;
        private const int MinWidthWithDriveWheel = 5;
        private const float RearWheelFromBack = 22f / PixelsPerColumn;
        private const float RearWheelSpacing = 12f / PixelsPerColumn;
        private const int ColumnsPerExtraRearWheel = 3;
        private const int WidthWithOneRearWheel = 4;
        private const float BoxStartPixels = 24f;
        private const float BoxEndFromBackPixels = 2f;
        private const float LogoWidthPixels = 26f;

        private readonly TruckPart[] parts;
        private readonly List<float> wheelCentres = new List<float>();

        public TruckPlan(int width)
        {
            parts = new TruckPart[Math.Max(0, width)];
            if (width < MinWidth)
            {
                Array.Fill(parts, TruckPart.TrailerMiddle);
                return;
            }

            for (int column = 0; column < width; column++)
                parts[column] = PartAt(column);

            float boxEndPixels = width * PixelsPerColumn - BoxEndFromBackPixels;
            HasLogo = boxEndPixels - BoxStartPixels > LogoWidthPixels;
            LogoCentre = (BoxStartPixels + boxEndPixels) / 2f / PixelsPerColumn;
            AddWheels(width);
        }

        public IReadOnlyList<TruckPart> Parts => parts;

        public IReadOnlyList<float> WheelCentres => wheelCentres;

        public bool HasLogo { get; }

        public float LogoCentre { get; }

        private void AddWheels(int width)
        {
            wheelCentres.Add(FrontWheelCentre);
            if (width >= MinWidthWithDriveWheel)
                wheelCentres.Add(DriveWheelCentre);
            int rearWheels = 1 + Math.Max(0, (width - WidthWithOneRearWheel) / ColumnsPerExtraRearWheel);
            float rearWheelCentre = width - RearWheelFromBack;
            for (int wheel = rearWheels - 1; wheel >= 0; wheel--)
                wheelCentres.Add(rearWheelCentre - wheel * RearWheelSpacing);
        }

        private TruckPart PartAt(int column)
        {
            if (column == 0)
                return TruckPart.CabFront;
            if (column == 1)
                return TruckPart.CabBack;
            return column == parts.Length - 1 ? TruckPart.TrailerRear : TruckPart.TrailerMiddle;
        }
    }
}
