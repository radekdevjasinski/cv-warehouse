using CvWarehouse.Core.Audio;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Audio
{
    public sealed class MixerVolumeTests
    {
        private const float BaseDecibels = -6f;
        private const float Tolerance = 0.01f;

        [Test]
        public void DecibelsFor_FullVolume_KeepsTheBaseLevel()
        {
            Assert.AreEqual(BaseDecibels, MixerVolume.DecibelsFor(BaseDecibels, 1f, false), Tolerance);
        }

        [Test]
        public void DecibelsFor_HalfVolume_IsAboutSixDecibelsQuieter()
        {
            Assert.AreEqual(BaseDecibels - 6.02f, MixerVolume.DecibelsFor(BaseDecibels, 0.5f, false), Tolerance);
        }

        [TestCase(0f, false)]
        [TestCase(1f, true)]
        [TestCase(0.5f, true)]
        public void DecibelsFor_ZeroOrMuted_IsSilent(float volume, bool isMuted)
        {
            Assert.AreEqual(MixerVolume.SilentDecibels, MixerVolume.DecibelsFor(BaseDecibels, volume, isMuted), Tolerance);
        }

        [Test]
        public void DecibelsFor_TinyVolume_NeverGoesBelowSilence()
        {
            Assert.AreEqual(MixerVolume.SilentDecibels, MixerVolume.DecibelsFor(BaseDecibels, 0.00001f, false), Tolerance);
        }
    }
}
