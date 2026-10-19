using System.Collections;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Effects;
using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace CvWarehouse.Tests.PlayMode.Effects
{
    public sealed class DeliveryPointsSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const int MaxFramesToWait = 300;
        private const int MaxTicks = 600000;
        private const float PopupLifetimeSeconds = 3f;
        private const float PositionTolerance = 0.01f;

        private WarehouseDemo demo;
        private DeliveryPointsView view;

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
            view = Object.FindAnyObjectByType<DeliveryPointsView>();
            Assert.IsNotNull(view, "The warehouse scene has no delivery points view.");
        }

        [UnityTest]
        public IEnumerator Delivery_BotUnloadsAtTheTruck_ShowsTheEarnedPointsAboveTheBot()
        {
            Bot unloadedBot = null;
            int earnedPoints = 0;
            demo.Simulation.Unloaded += (bot, points) =>
            {
                unloadedBot = bot;
                earnedPoints = points;
            };

            for (int tick = 0; tick < MaxTicks && unloadedBot == null; tick++)
                demo.Simulation.Tick();
            yield return null;

            Assert.IsNotNull(unloadedBot, "No bot reached the truck.");
            Assert.AreEqual(1, view.VisiblePopupCount);
            TextMeshPro label = view.Label(0);
            Assert.IsTrue(label.gameObject.activeInHierarchy);
            Assert.AreEqual("+" + earnedPoints, label.text);
            Assert.AreEqual(unloadedBot.CentreX, label.transform.position.x, PositionTolerance);
            Assert.Greater(label.transform.position.y, unloadedBot.CentreY);
            Assert.IsTrue(label.fontSharedMaterial.IsKeywordEnabled("OUTLINE_ON"), "The points need a black outline.");
        }

        [UnityTest]
        public IEnumerator Delivery_PopupLifetimeOver_HidesThePoints()
        {
            bool hasUnloaded = false;
            demo.Simulation.Unloaded += (bot, points) => hasUnloaded = true;
            for (int tick = 0; tick < MaxTicks && !hasUnloaded; tick++)
                demo.Simulation.Tick();

            yield return new WaitForSeconds(PopupLifetimeSeconds);

            Assert.AreEqual(0, view.VisiblePopupCount);
            Assert.IsFalse(view.Label(0).gameObject.activeSelf);
        }
    }
}
