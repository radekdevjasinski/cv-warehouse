using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class SimulationSettingsTests
    {
        private const int TopSpeedLevel = 5;

        [Test]
        public void StepTicks_DefaultSettingsAtFirstLevel_TakeQuarterOfASecondPerCell()
        {
            var settings = new SimulationSettings();

            Assert.AreEqual(16, settings.StepTicks(Bot.FirstLevel, GridDirections.StraightCost));
        }

        [Test]
        public void StepTicks_DefaultSettingsAtTopLevel_TakeHalfAsLongAsAtFirstLevel()
        {
            var settings = new SimulationSettings();

            Assert.AreEqual(8, settings.StepTicks(TopSpeedLevel, GridDirections.StraightCost));
        }

        [TestCase(Importance.Normal, 0.5f)]
        [TestCase(Importance.Important, 1f)]
        [TestCase(Importance.Key, 2f)]
        public void ExtractTicks_DefaultSettings_LastTheDesignedSeconds(Importance importance, float expectedSeconds)
        {
            var settings = new SimulationSettings();

            float seconds = settings.ExtractTicks(importance) * settings.TickSeconds;

            Assert.AreEqual(expectedSeconds, seconds, 0.001f);
        }
    }
}
