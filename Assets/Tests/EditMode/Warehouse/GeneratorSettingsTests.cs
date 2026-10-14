using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class GeneratorSettingsTests
    {
        [TestCase(1, BoxSizeClass.Small)]
        [TestCase(8, BoxSizeClass.Small)]
        [TestCase(9, BoxSizeClass.Medium)]
        [TestCase(20, BoxSizeClass.Medium)]
        [TestCase(21, BoxSizeClass.Large)]
        public void SizeClassOf_BlockWords_PicksTheSizeByTheDefaultLimits(int words, BoxSizeClass expectedSizeClass)
        {
            CvBlock block = CvTestBlocks.FromText(CvTestBlocks.Words(words));

            Assert.AreEqual(expectedSizeClass, new GeneratorSettings().SizeClassOf(block));
        }

        [Test]
        public void SizeClassOf_LimitsChanged_FollowsTheNewLimits()
        {
            var settings = new GeneratorSettings { SmallBoxMaxWords = 2, MediumBoxMaxWords = 3 };

            Assert.AreEqual(BoxSizeClass.Medium, settings.SizeClassOf(CvTestBlocks.FromText(CvTestBlocks.Words(3))));
            Assert.AreEqual(BoxSizeClass.Large, settings.SizeClassOf(CvTestBlocks.FromText(CvTestBlocks.Words(4))));
        }

        [Test]
        public void ImportanceOf_Entry_KeepsTheImportanceFromTheFile()
        {
            CvBlock block = CvTestBlocks.Build("{\"sections\":[{\"entries\":[{\"title\":\"Game\",\"importance\":\"important\"}]}]}")[0];

            Assert.AreEqual(Importance.Important, new GeneratorSettings().ImportanceOf(block));
        }
    }
}
