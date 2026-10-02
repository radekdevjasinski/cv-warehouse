using System.Collections;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace CvWarehouse.Tests.PlayMode.Warehouse
{
    public sealed class WarehouseDemoSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const string AddBotButtonName = "AddBotButton";
        private const string RemoveBotButtonName = "RemoveBotButton";
        private const int StartingBots = 3;
        private const int MaxTicks = 600000;

        private WarehouseDemo demo;

        [UnitySetUp]
        public IEnumerator LoadDemoScene()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return null;
            yield return null;
            demo =Object.FindAnyObjectByType<WarehouseDemo>();
            demo.enabled = false;
        }

        [UnityTest]
        public IEnumerator Scene_Booted_ShowsEveryBoxAndTheStartingBots()
        {
            yield return null;

            Assert.AreEqual(StartingBots, demo.Simulation.Bots.Count);
            Assert.AreEqual(StartingBots, Object.FindAnyObjectByType<WarehouseBotsView>().VisibleBotCount);
            Assert.AreEqual(demo.Simulation.Layout.Boxes.Count, Object.FindAnyObjectByType<WarehouseBoxesView>().VisibleBoxCount);
        }

        [UnityTest]
        public IEnumerator Buttons_Clicked_AddAndRemoveABot()
        {
            GameObject.Find(AddBotButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.AreEqual(StartingBots + 1, Object.FindAnyObjectByType<WarehouseBotsView>().VisibleBotCount);

            GameObject.Find(RemoveBotButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.AreEqual(StartingBots, Object.FindAnyObjectByType<WarehouseBotsView>().VisibleBotCount);
        }

        [UnityTest]
        public IEnumerator Simulation_RunToTheEnd_HidesEveryBox()
        {
            WarehouseSimulation simulation = demo.Simulation;
            while (simulation.TryAddBot())
            {
            }

            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
                simulation.Tick();
            yield return null;

            Assert.IsTrue(simulation.IsComplete);
            Assert.AreEqual(0, Object.FindAnyObjectByType<WarehouseBoxesView>().VisibleBoxCount);
        }
    }
}
