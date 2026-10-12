using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class VignetteShapeTests
    {
        private const float ClearRadius = 0.5f;
        private const float CornerOpacity = 0.4f;
        private const float Tolerance = 0.0001f;

        [Test]
        public void OpacityAt_InsideTheClearRadius_IsFullyClear()
        {
            Assert.AreEqual(0f, VignetteShape.OpacityAt(0f, ClearRadius, CornerOpacity), Tolerance);
            Assert.AreEqual(0f, VignetteShape.OpacityAt(ClearRadius, ClearRadius, CornerOpacity), Tolerance);
        }

        [Test]
        public void OpacityAt_Corner_IsTheCornerOpacity()
        {
            Assert.AreEqual(CornerOpacity, VignetteShape.OpacityAt(1f, ClearRadius, CornerOpacity), Tolerance);
        }

        [Test]
        public void OpacityAt_FurtherFromTheCentre_NeverGetsLighter()
        {
            float previous = 0f;
            for (float distance = 0f; distance <= 1f; distance += 0.05f)
            {
                float opacity = VignetteShape.OpacityAt(distance, ClearRadius, CornerOpacity);
                Assert.GreaterOrEqual(opacity, previous);
                previous = opacity;
            }
        }
    }
}
