using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class WarehouseSizerTests
    {
        [Test]
        public void CountBoxCells_Cv_AddsUpTheFootprintOfEveryBox()
        {
            List<CvBlock> blocks = CvTestBlocks.BuildCv(2, 3, 4);
            var sizer = new WarehouseSizer(new GeneratorSettings());

            int nameJobTitleAndTwoContacts = 1 + 1 + 1 + 1;
            Assert.AreEqual(nameJobTitleAndTwoContacts + 2 * 4 + 3 * 2 + 4, sizer.CountBoxCells(blocks));
        }

        [Test]
        public void StorageSizeFor_BoxCells_LeavesTheConfiguredShareOfTheFloorFree()
        {
            var sizer = new WarehouseSizer(new GeneratorSettings { FillPercent = 10, MinWidth = 1, MinStorageHeight = 1 });

            GridSize storage = sizer.StorageSizeFor(200, 0);

            Assert.GreaterOrEqual(storage.Width * storage.Height, 2000);
            Assert.Less(storage.Width * storage.Height, 2200);
        }

        [Test]
        public void StorageSizeFor_WidthToHeightPercent_ShapesTheMap()
        {
            var sizer = new WarehouseSizer(new GeneratorSettings { FillPercent = 10, WidthToHeightPercent = 200, MinWidth = 1, MinStorageHeight = 1 });

            GridSize storage = sizer.StorageSizeFor(200, 0);

            Assert.AreEqual(2f, (float)storage.Width / storage.Height, 0.2f);
        }

        [Test]
        public void StorageSizeFor_FewBoxCells_IsNeverNarrowerThanTheRamp()
        {
            var settings = new GeneratorSettings { MinWidth = 1, MinStorageHeight = 5 };
            var sizer = new WarehouseSizer(settings);

            GridSize storage = sizer.StorageSizeFor(1, 0);

            Assert.AreEqual(settings.TruckWidth + settings.ParkingPerSide * 2, storage.Width);
            Assert.AreEqual(5, storage.Height);
        }

        [Test]
        public void StorageSizeFor_GrowthSteps_MakeTheMapBigger()
        {
            var sizer = new WarehouseSizer(new GeneratorSettings { MinWidth = 1, MinStorageHeight = 1 });

            GridSize first = sizer.StorageSizeFor(300, 0);
            GridSize grown = sizer.StorageSizeFor(300, 2);

            Assert.Greater(grown.Width * grown.Height, first.Width * first.Height);
        }
    }
}
