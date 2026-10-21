using System.Collections;
using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace CvWarehouse.Tests.PlayMode.Warehouse
{
    public sealed class CrewAndBotsSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const string SelectAllButtonName = "SelectAllButton";
        private const string CrewChipsName = "CrewChips";
        private const string BotObjectPrefix = "Bot ";
        private const string FaceName = "Face";
        private const string CargoName = "Cargo";
        private const string ProgressBarName = "Progress Bar";
        private const int MaxFramesToWait = 300;

        private WarehouseDemo demo;

        [UnitySetUp]
        public IEnumerator LoadDemoScene()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return null;
            demo = Object.FindAnyObjectByType<WarehouseDemo>();
            demo.enabled = false;
            for (int frame = 0; frame < MaxFramesToWait && demo.Simulation == null; frame++)
                yield return null;

            Assert.IsNotNull(demo.Simulation, "The warehouse did not boot from the CV in time.");
        }

        [UnityTest]
        public IEnumerator SelectAllButton_Clicked_SelectsEveryBot()
        {
            while (demo.Simulation.TryAddBot())
            {
            }

            GameObject.Find(SelectAllButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(demo.Simulation.Bots.Count, demo.Selection.Count);
            foreach (var bot in demo.Simulation.Bots)
                Assert.IsTrue(demo.Selection.Contains(bot), bot.Name);
        }

        [Test]
        public void CrewChips_Booted_AreCentredInThePanel()
        {
            Assert.AreEqual(TextAnchor.UpperCenter, GameObject.Find(CrewChipsName).GetComponent<LayoutGroup>().childAlignment);
        }

        [UnityTest]
        public IEnumerator BotFigures_Shown_DrawEachBotAsOneSortingGroup()
        {
            yield return null;

            for (int index = 0; index < demo.Simulation.Bots.Count; index++)
            {
                GameObject botObject = GameObject.Find(BotObjectPrefix + index);
                SortingGroup group = botObject.GetComponent<SortingGroup>();
                Assert.IsNotNull(group, botObject.name);
                SpriteRenderer body = botObject.GetComponent<SpriteRenderer>();
                Assert.Greater(botObject.transform.Find(FaceName).GetComponent<SpriteRenderer>().sortingOrder, body.sortingOrder);
                Assert.Greater(botObject.transform.Find(CargoName).GetComponent<SpriteRenderer>().sortingOrder, body.sortingOrder);
                Assert.IsNull(botObject.transform.Find(ProgressBarName), "The progress bar must stay outside the group to draw over boxes.");
            }
        }

        [UnityTest]
        public IEnumerator BotFigures_OnTheSameRow_NeverShareADepth()
        {
            yield return null;

            var depths = new System.Collections.Generic.HashSet<float>();
            for (int index = 0; index < demo.Simulation.Bots.Count; index++)
                Assert.IsTrue(depths.Add(GameObject.Find(BotObjectPrefix + index).transform.position.z), "bot " + index);
        }
    }
}
