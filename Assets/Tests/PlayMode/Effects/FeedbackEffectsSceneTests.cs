using System.Collections;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace CvWarehouse.Tests.PlayMode.Effects
{
    public sealed class FeedbackEffectsSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const int MaxFramesToWait = 300;
        private const int MaxTicks = 600000;
        private const float EffectOverSeconds = 1f;
        private const float Tolerance = 0.0001f;

        private WarehouseDemo demo;
        private WarehouseBotsView botsView;

        [UnitySetUp]
        public IEnumerator LoadWarehouseScene()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return null;
            yield return null;
            demo = Object.FindAnyObjectByType<WarehouseDemo>();
            demo.enabled = false;
            for (int frame = 0; frame < MaxFramesToWait && demo.Simulation == null; frame++)
                yield return null;

            Assert.IsNotNull(demo.Simulation, "The warehouse did not boot from the CV in time.");
            botsView = Object.FindAnyObjectByType<WarehouseBotsView>();
        }

        [UnityTest]
        public IEnumerator NewBot_Added_PopsInFromSmallToFullSize()
        {
            Assert.IsTrue(demo.Simulation.TryAddBot(), "The warehouse has no free parking cell.");
            int newBot = demo.Simulation.Bots.Count - 1;
            yield return null;

            Assert.Less(botsView.Figure(newBot).Scale, 1f);

            yield return new WaitForSeconds(EffectOverSeconds);

            Assert.AreEqual(1f, botsView.Figure(newBot).Scale, Tolerance);
        }

        [UnityTest]
        public IEnumerator StartingBots_Booted_StandAtFullSize()
        {
            yield return null;

            Assert.AreEqual(1f, botsView.Figure(0).Scale, Tolerance);
        }

        [UnityTest]
        public IEnumerator Upgrade_BotUpgraded_FlashesBrighterThenReturnsToItsColour()
        {
            yield return null;
            Color crewColor = botsView.Figure(0).BodyColor;

            demo.Simulation.Upgrade(demo.Simulation.Bots[0], BotUpgrade.Speed);
            yield return null;

            Assert.Greater(Brightness(botsView.Figure(0).BodyColor), Brightness(crewColor));

            yield return new WaitForSeconds(EffectOverSeconds);

            Assert.AreEqual((Color32)crewColor, (Color32)botsView.Figure(0).BodyColor);
        }

        [UnityTest]
        public IEnumerator PointsCounter_PointsEarned_PulsesThenSettles()
        {
            PointsView pointsView = Object.FindAnyObjectByType<PointsView>();
            yield return null;
            Assert.AreEqual(1f, pointsView.PulseScale, Tolerance);

            for (int tick = 0; tick < MaxTicks && demo.Simulation.Score.Points == 0; tick++)
                demo.Simulation.Tick();
            yield return null;
            yield return null;

            Assert.Greater(pointsView.PulseScale, 1f);

            yield return new WaitForSeconds(EffectOverSeconds);

            Assert.AreEqual(1f, pointsView.PulseScale, Tolerance);
        }

        private static float Brightness(Color color)
        {
            return color.r + color.g + color.b;
        }
    }
}
