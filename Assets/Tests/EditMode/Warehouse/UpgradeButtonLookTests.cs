using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class UpgradeButtonLookTests
    {
        [TestCase(true, true, UpgradeButtonState.Affordable)]
        [TestCase(true, false, UpgradeButtonState.TooExpensive)]
        [TestCase(false, false, UpgradeButtonState.Maxed)]
        public void StateOf_UpgradeAndPoints_PicksTheButtonState(bool hasUpgrade, bool canUpgrade, UpgradeButtonState expectedState)
        {
            Assert.AreEqual(expectedState, UpgradeButtonLook.StateOf(hasUpgrade, canUpgrade));
        }
    }
}
