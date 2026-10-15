using System;
using CvWarehouse.Core.Audio;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Audio
{
    public sealed class SoundVariationTests
    {
        private const int Seed = 42;
        private const int FourClips = 4;
        private const int ManyPlays = 500;
        private const float Pitch = 1f;
        private const float PitchVariation = 0.1f;
        private const float MinPitch = Pitch - PitchVariation;
        private const float MaxPitch = Pitch + PitchVariation;
        private const float Cooldown = 0.05f;

        [Test]
        public void TryPick_ManyPlays_PitchStaysInsideTheRange()
        {
            SoundVariation variation = CreateVariation(FourClips, 0f);

            for (int play = 0; play < ManyPlays; play++)
            {
                Assert.IsTrue(variation.TryPick(out _, out float pitch));
                Assert.That(pitch, Is.InRange(MinPitch, MaxPitch));
            }
        }

        [Test]
        public void TryPick_ManyPlays_PitchActuallyVaries()
        {
            SoundVariation variation = CreateVariation(FourClips, 0f);
            variation.TryPick(out _, out float firstPitch);

            bool hasDifferentPitch = false;
            for (int play = 0; play < ManyPlays && !hasDifferentPitch; play++)
            {
                variation.TryPick(out _, out float pitch);
                hasDifferentPitch = pitch != firstPitch;
            }

            Assert.IsTrue(hasDifferentPitch);
        }

        [Test]
        public void TryPick_SeveralClips_NeverRepeatsTheSameClipTwiceInARow()
        {
            SoundVariation variation = CreateVariation(FourClips, 0f);
            variation.TryPick(out int previousClip, out _);

            for (int play = 0; play < ManyPlays; play++)
            {
                variation.TryPick(out int clip, out _);
                Assert.That(clip, Is.InRange(0, FourClips - 1));
                Assert.AreNotEqual(previousClip, clip);
                previousClip = clip;
            }
        }

        [Test]
        public void TryPick_ManyPlays_UsesEveryClip()
        {
            SoundVariation variation = CreateVariation(FourClips, 0f);
            var timesPlayed = new int[FourClips];

            for (int play = 0; play < ManyPlays; play++)
            {
                variation.TryPick(out int clip, out _);
                timesPlayed[clip]++;
            }

            foreach (int times in timesPlayed)
                Assert.Greater(times, 0);
        }

        [Test]
        public void TryPick_SingleClip_AlwaysPicksIt()
        {
            SoundVariation variation = CreateVariation(1, 0f);

            for (int play = 0; play < ManyPlays; play++)
            {
                Assert.IsTrue(variation.TryPick(out int clip, out _));
                Assert.AreEqual(0, clip);
            }
        }

        [Test]
        public void TryPick_NoClips_StaysSilent()
        {
            SoundVariation variation = CreateVariation(0, 0f);

            Assert.IsFalse(variation.TryPick(out _, out _));
        }

        [Test]
        public void TryPick_FirstPlay_IsNotHeldBackByTheCooldown()
        {
            SoundVariation variation = CreateVariation(FourClips, Cooldown);

            Assert.IsTrue(variation.TryPick(out _, out _));
        }

        [Test]
        public void TryPick_InsideTheCooldown_SkipsThePlay()
        {
            SoundVariation variation = CreateVariation(FourClips, Cooldown);
            variation.TryPick(out _, out _);
            variation.Advance(Cooldown / 2f);

            Assert.IsFalse(variation.TryPick(out _, out _));
        }

        [Test]
        public void TryPick_AfterTheCooldown_PlaysAgain()
        {
            SoundVariation variation = CreateVariation(FourClips, Cooldown);
            variation.TryPick(out _, out _);
            variation.Advance(Cooldown);

            Assert.IsTrue(variation.TryPick(out _, out _));
        }

        [Test]
        public void TryPick_SameSeed_PicksTheSameSequence()
        {
            SoundVariation first = CreateVariation(FourClips, 0f);
            SoundVariation second = CreateVariation(FourClips, 0f);

            for (int play = 0; play < ManyPlays; play++)
            {
                first.TryPick(out int firstClip, out float firstPitch);
                second.TryPick(out int secondClip, out float secondPitch);
                Assert.AreEqual(firstClip, secondClip);
                Assert.AreEqual(firstPitch, secondPitch);
            }
        }

        [Test]
        public void TryPick_VariationReachesBelowZero_KeepsThePitchPositive()
        {
            var settings = new SoundVariationSettings { ClipCount = FourClips, Pitch = 0.1f, PitchVariation = 1f, MinSecondsBetweenPlays = 0f };
            var variation = new SoundVariation(settings, new Random(Seed));

            for (int play = 0; play < ManyPlays; play++)
            {
                variation.TryPick(out _, out float pitch);
                Assert.Greater(pitch, 0f);
            }
        }

        [Test]
        public void TryPick_SettingsChangedAfterCreation_UsesTheNewValues()
        {
            var settings = new SoundVariationSettings { ClipCount = FourClips, Pitch = Pitch, PitchVariation = 0f, MinSecondsBetweenPlays = 0f };
            var variation = new SoundVariation(settings, new Random(Seed));
            variation.TryPick(out _, out _);

            settings.ClipCount = 1;
            settings.Pitch = MaxPitch;

            Assert.IsTrue(variation.TryPick(out int clip, out float pitch));
            Assert.AreEqual(0, clip);
            Assert.AreEqual(MaxPitch, pitch);
        }

        [Test]
        public void TryPick_NegativeClipCount_StaysSilent()
        {
            SoundVariation variation = CreateVariation(-1, 0f);

            Assert.IsFalse(variation.TryPick(out _, out _));
        }

        private static SoundVariation CreateVariation(int clipCount, float cooldown)
        {
            var settings = new SoundVariationSettings
            {
                ClipCount = clipCount,
                Pitch = Pitch,
                PitchVariation = PitchVariation,
                MinSecondsBetweenPlays = cooldown
            };
            return new SoundVariation(settings, new Random(Seed));
        }
    }
}
