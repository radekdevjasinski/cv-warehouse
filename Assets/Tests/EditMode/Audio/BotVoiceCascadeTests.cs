using System;
using System.Collections.Generic;
using CvWarehouse.Core.Audio;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Audio
{
    public sealed class BotVoiceCascadeTests
    {
        private const int Seed = 7;
        private const int EightVoices = 8;
        private const int MaxVoices = 4;
        private const float Stagger = 0.05f;
        private const float GroupVolume = 0.8f;
        private const float Pitch = 1f;
        private const float PitchVariation = 0.03f;
        private const float LongPause = 10f;
        private const int ManyCascades = 200;

        private BotVoiceSettings settings;
        private BotVoiceCascade cascade;
        private List<BotVoiceCue> dueCues;

        [SetUp]
        public void SetUp()
        {
            settings = new BotVoiceSettings
            {
                VoiceCount = EightVoices,
                MaxVoicesInCascade = MaxVoices,
                SecondsBetweenVoices = Stagger,
                GroupVolume = GroupVolume,
                Pitch = Pitch,
                PitchVariation = PitchVariation
            };
            cascade = new BotVoiceCascade(settings, new Random(Seed));
            dueCues = new List<BotVoiceCue>();
        }

        [Test]
        public void Start_SingleBot_PlaysItsOwnVoiceAtOnceAtFullVolume()
        {
            cascade.Start(new[] { 3 });
            cascade.Advance(0f, dueCues);

            Assert.AreEqual(1, dueCues.Count);
            Assert.AreEqual(3, dueCues[0].Voice);
            Assert.AreEqual(1f, dueCues[0].Volume);
            Assert.IsFalse(cascade.IsPlaying);
        }

        [Test]
        public void Start_CrewSlotPastTheLastVoice_WrapsAroundLikeTheCrewColours()
        {
            cascade.Start(new[] { EightVoices + 2 });
            cascade.Advance(0f, dueCues);

            Assert.AreEqual(2, dueCues[0].Voice);
        }

        [Test]
        public void Start_SeveralBots_PlaysOneVoiceAtATimeSpacedByTheStagger()
        {
            cascade.Start(new[] { 0, 1, 2 });

            cascade.Advance(0f, dueCues);
            Assert.AreEqual(1, dueCues.Count);

            cascade.Advance(Stagger / 2f, dueCues);
            Assert.AreEqual(0, dueCues.Count);

            cascade.Advance(Stagger / 2f, dueCues);
            Assert.AreEqual(1, dueCues.Count);

            cascade.Advance(Stagger, dueCues);
            Assert.AreEqual(1, dueCues.Count);
            Assert.IsFalse(cascade.IsPlaying);
        }

        [Test]
        public void Start_SeveralBots_PlaysThemInCrewSlotOrder()
        {
            cascade.Start(new[] { 5, 1, 3 });

            List<int> voices = PlayWholeCascade();

            CollectionAssert.AreEqual(new[] { 1, 3, 5 }, voices);
        }

        [Test]
        public void Start_MoreBotsThanTheCap_PlaysOnlyTheFirstFourByCrewSlot()
        {
            cascade.Start(new[] { 7, 6, 5, 4, 3, 2, 1, 0 });

            List<int> voices = PlayWholeCascade();

            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3 }, voices);
        }

        [Test]
        public void Start_SeveralBots_PlaysEveryVoiceAtTheGroupVolume()
        {
            cascade.Start(new[] { 0, 1 });
            cascade.Advance(LongPause, dueCues);

            Assert.AreEqual(2, dueCues.Count);
            foreach (BotVoiceCue cue in dueCues)
                Assert.AreEqual(GroupVolume, cue.Volume);
        }

        [Test]
        public void Start_WhileACascadeIsPlaying_ReplacesItWithTheNewSelection()
        {
            cascade.Start(new[] { 0, 1, 2, 3 });
            cascade.Advance(0f, dueCues);

            cascade.Start(new[] { 6 });
            List<int> voices = PlayWholeCascade();

            CollectionAssert.AreEqual(new[] { 6 }, voices);
        }

        [Test]
        public void Advance_ManyCascades_PitchStaysCloseToTheVoice()
        {
            for (int play = 0; play < ManyCascades; play++)
            {
                cascade.Start(new[] { 0 });
                cascade.Advance(0f, dueCues);
                Assert.That(dueCues[0].Pitch, Is.InRange(Pitch - PitchVariation, Pitch + PitchVariation));
            }
        }

        [Test]
        public void Start_NoVoices_StaysSilent()
        {
            settings.VoiceCount = 0;

            cascade.Start(new[] { 0, 1 });
            cascade.Advance(LongPause, dueCues);

            Assert.AreEqual(0, dueCues.Count);
            Assert.IsFalse(cascade.IsPlaying);
        }

        [Test]
        public void Start_NoBots_StaysSilent()
        {
            cascade.Start(Array.Empty<int>());
            cascade.Advance(LongPause, dueCues);

            Assert.AreEqual(0, dueCues.Count);
        }

        private List<int> PlayWholeCascade()
        {
            var voices = new List<int>();
            cascade.Advance(0f, dueCues);
            while (true)
            {
                foreach (BotVoiceCue cue in dueCues)
                    voices.Add(cue.Voice);
                if (!cascade.IsPlaying)
                    return voices;
                cascade.Advance(Stagger, dueCues);
            }
        }
    }
}
