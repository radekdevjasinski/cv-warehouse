using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseSizer
    {
        private const int Percent = 100;
        private const int GrowthStepPercent = 50;

        private readonly GeneratorSettings settings;

        public WarehouseSizer(GeneratorSettings settings)
        {
            this.settings = settings;
        }

        public int CountBoxCells(IReadOnlyList<CvBlock> blocks)
        {
            int boxCells = 0;
            foreach (CvBlock block in blocks)
            {
                GridSize footprint = BoxFootprints.For(settings.WeightOf(block));
                boxCells += footprint.Width * footprint.Height;
            }

            return boxCells;
        }

        public GridSize StorageSizeFor(int boxCells, int growthSteps)
        {
            int storageCells = boxCells * Percent / Math.Max(1, settings.FillPercent);
            storageCells += storageCells * growthSteps * GrowthStepPercent / Percent;

            int width = Math.Max(settings.MinWidth, RampWidth());
            while (width * width * Percent < storageCells * settings.WidthToHeightPercent)
                width++;

            int height = Math.Max(settings.MinStorageHeight, (storageCells + width - 1) / width);
            return new GridSize(width, height);
        }

        private int RampWidth()
        {
            return Math.Max(settings.TruckWidth, settings.DropCells) + settings.ParkingPerSide * 2;
        }
    }
}
