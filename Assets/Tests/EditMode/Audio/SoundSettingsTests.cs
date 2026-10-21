using CvWarehouse.Core.Audio;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Audio
{
    public sealed class SoundSettingsTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void New_NothingSaved_StartsAtFullVolumeAndUnmuted()
        {
            var settings = new SoundSettings(new FakeSoundSettingsStore(SoundLevels.Default));

            Assert.AreEqual(1f, settings.MusicVolume, Tolerance);
            Assert.AreEqual(1f, settings.SfxVolume, Tolerance);
            Assert.IsFalse(settings.IsMuted);
        }

        [Test]
        public void New_SavedLevels_StartsFromThem()
        {
            var settings = new SoundSettings(new FakeSoundSettingsStore(new SoundLevels(0.25f, 0.5f, true)));

            Assert.AreEqual(0.25f, settings.MusicVolume, Tolerance);
            Assert.AreEqual(0.5f, settings.SfxVolume, Tolerance);
            Assert.IsTrue(settings.IsMuted);
        }

        [Test]
        public void New_SavedLevelsOutOfRange_ClampsThem()
        {
            var settings = new SoundSettings(new FakeSoundSettingsStore(new SoundLevels(3f, -1f, false)));

            Assert.AreEqual(1f, settings.MusicVolume, Tolerance);
            Assert.AreEqual(0f, settings.SfxVolume, Tolerance);
        }

        [TestCase(0.4f, 0.4f)]
        [TestCase(-0.5f, 0f)]
        [TestCase(1.5f, 1f)]
        public void SetMusicVolume_Value_StoresItClamped(float volume, float expected)
        {
            var settings = new SoundSettings(new FakeSoundSettingsStore(SoundLevels.Default));

            settings.SetMusicVolume(volume);

            Assert.AreEqual(expected, settings.MusicVolume, Tolerance);
        }

        [Test]
        public void SetSfxVolume_Value_ChangesOnlyTheEffects()
        {
            var settings = new SoundSettings(new FakeSoundSettingsStore(SoundLevels.Default));

            settings.SetSfxVolume(0.3f);

            Assert.AreEqual(0.3f, settings.SfxVolume, Tolerance);
            Assert.AreEqual(1f, settings.MusicVolume, Tolerance);
        }

        [Test]
        public void ToggleMute_Twice_EndsUnmutedAndKeepsTheVolumes()
        {
            var settings = new SoundSettings(new FakeSoundSettingsStore(SoundLevels.Default));
            settings.SetMusicVolume(0.6f);

            settings.ToggleMute();
            Assert.IsTrue(settings.IsMuted);
            settings.ToggleMute();

            Assert.IsFalse(settings.IsMuted);
            Assert.AreEqual(0.6f, settings.MusicVolume, Tolerance);
        }

        [Test]
        public void Changes_AnyChange_RaisesChanged()
        {
            var settings = new SoundSettings(new FakeSoundSettingsStore(SoundLevels.Default));
            int changes = 0;
            settings.Changed += () => changes++;

            settings.SetMusicVolume(0.5f);
            settings.SetSfxVolume(0.5f);
            settings.ToggleMute();

            Assert.AreEqual(3, changes);
        }

        [Test]
        public void Changes_BeforeSave_AreNotWritten()
        {
            var store = new FakeSoundSettingsStore(SoundLevels.Default);
            var settings = new SoundSettings(store);

            settings.SetMusicVolume(0.5f);

            Assert.AreEqual(0, store.SaveCount);
        }

        [Test]
        public void Save_AfterChanges_WritesTheCurrentLevels()
        {
            var store = new FakeSoundSettingsStore(SoundLevels.Default);
            var settings = new SoundSettings(store);
            settings.SetMusicVolume(0.2f);
            settings.SetSfxVolume(0.7f);
            settings.ToggleMute();

            settings.Save();

            Assert.AreEqual(1, store.SaveCount);
            Assert.AreEqual(0.2f, store.Saved.MusicVolume, Tolerance);
            Assert.AreEqual(0.7f, store.Saved.SfxVolume, Tolerance);
            Assert.IsTrue(store.Saved.IsMuted);
        }
    }
}
