using System.Collections;
using CvWarehouse.Presentation.Cv;
using CvWarehouse.Presentation.Menu;
using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace CvWarehouse.Tests.PlayMode.Warehouse
{
    public sealed class GameMenuSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const string MenuButtonName = "MenuButton";
        private const string PanelName = "GameMenuPanel";
        private const string ResumeButtonName = "ResumeButton";
        private const string RestartButtonName = "RestartButton";
        private const string ShowCvButtonName = "ShowCvButton";
        private const string SkipGameButtonName = "SkipGameButton";
        private const string ConfirmSkipButtonName = "ConfirmSkipButton";
        private const string CancelSkipButtonName = "CancelSkipButton";
        private const string MenuWindowName = "GameMenuWindow";
        private const string ShowCvLabel = "Show CV";
        private const string HideCvLabel = "Hide CV";
        private const string MainMenuButtonName = "MainMenuButton";
        private const string PauseButtonName = "PauseButton";
        private const string ShiftStatusTextName = "ShiftStatusText";
        private const string PausedLabel = "PAUSED";
        private const int MaxFramesToWait = 300;

        private WarehouseDemo demo;
        private GameMenuView gameMenu;

        [UnitySetUp]
        public IEnumerator LoadDemoScene()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return null;
            demo = Object.FindAnyObjectByType<WarehouseDemo>();
            gameMenu = Object.FindAnyObjectByType<GameMenuView>();
            for (int frame = 0; frame < MaxFramesToWait && demo.Simulation == null; frame++)
                yield return null;

            Assert.IsNotNull(demo.Simulation, "The warehouse did not boot from the CV in time.");
        }

        [UnityTest]
        public IEnumerator Scene_Booted_StartsWithTheMenuClosed()
        {
            yield return null;

            Assert.IsFalse(gameMenu.IsOpen);
            Assert.IsNull(GameObject.Find(PanelName));
            Assert.IsFalse(demo.Simulation.IsHeld);
        }

        [UnityTest]
        public IEnumerator MenuButton_Clicked_OpensThePanelAndStopsTheSimulation()
        {
            Click(MenuButtonName);
            int ticksWhenOpened = demo.Simulation.TickCount;
            yield return null;
            yield return null;

            Assert.IsTrue(gameMenu.IsOpen);
            Assert.IsNotNull(GameObject.Find(PanelName));
            Assert.IsTrue(demo.Simulation.IsHeld);
            Assert.AreEqual(ticksWhenOpened, demo.Simulation.TickCount);
            Assert.AreEqual(PausedLabel, GameObject.Find(ShiftStatusTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator ResumeButton_Clicked_ClosesThePanelAndRunsTheSimulationAgain()
        {
            Click(MenuButtonName);
            yield return null;

            Click(ResumeButtonName);
            yield return null;

            Assert.IsFalse(gameMenu.IsOpen);
            Assert.IsFalse(demo.Simulation.IsHeld);
            Assert.IsFalse(demo.Simulation.IsPaused);
        }

        [UnityTest]
        public IEnumerator ResumeButton_GameWasPausedBeforeTheMenu_KeepsThePause()
        {
            Click(PauseButtonName);
            Click(MenuButtonName);
            yield return null;

            Click(ResumeButtonName);
            yield return null;

            Assert.IsFalse(demo.Simulation.IsHeld);
            Assert.IsTrue(demo.Simulation.IsPaused);
        }

        [UnityTest]
        public IEnumerator ShowCvButton_Clicked_ExpandsTheWaybillWithoutRevealingIt()
        {
            Click(MenuButtonName);
            yield return null;
            Assert.AreEqual(ShowCvLabel, LabelOf(ShowCvButtonName));

            Click(ShowCvButtonName);
            yield return null;

            Assert.IsTrue(Object.FindAnyObjectByType<WarehouseScreenSplit>().IsWaybillExpanded);
            Assert.IsFalse(Object.FindAnyObjectByType<CvScreen>().RevealState.IsComplete);
            Assert.IsFalse(gameMenu.IsOpen);
            Assert.IsFalse(demo.Simulation.IsHeld);
        }

        [UnityTest]
        public IEnumerator ShowCvButton_WaybillAlreadyExpanded_ReadsHideCvAndCollapsesIt()
        {
            Object.FindAnyObjectByType<WarehouseScreenSplit>().ExpandWaybill();
            Click(MenuButtonName);
            yield return null;
            Assert.AreEqual(HideCvLabel, LabelOf(ShowCvButtonName));

            Click(ShowCvButtonName);
            yield return null;

            Assert.IsFalse(Object.FindAnyObjectByType<WarehouseScreenSplit>().IsWaybillExpanded);
            Assert.IsFalse(gameMenu.IsOpen);
        }

        [UnityTest]
        public IEnumerator SkipGameButton_Clicked_AsksBeforeRevealingAnything()
        {
            Click(MenuButtonName);
            yield return null;

            Click(SkipGameButtonName);
            yield return null;

            Assert.IsTrue(gameMenu.IsAskingToSkip);
            Assert.IsNull(GameObject.Find(MenuWindowName));
            Assert.IsFalse(Object.FindAnyObjectByType<CvScreen>().RevealState.IsComplete);
            Assert.IsTrue(demo.Simulation.IsHeld);
        }

        [UnityTest]
        public IEnumerator CancelSkipButton_Clicked_GoesBackToTheMenuWithoutRevealing()
        {
            Click(MenuButtonName);
            Click(SkipGameButtonName);
            yield return null;

            Click(CancelSkipButtonName);
            yield return null;

            Assert.IsFalse(gameMenu.IsAskingToSkip);
            Assert.IsNotNull(GameObject.Find(MenuWindowName));
            Assert.IsTrue(gameMenu.IsOpen);
            Assert.IsFalse(Object.FindAnyObjectByType<CvScreen>().RevealState.IsComplete);
        }

        [UnityTest]
        public IEnumerator MenuReopened_AfterAskingToSkip_StartsOnTheMenuAgain()
        {
            Click(MenuButtonName);
            Click(SkipGameButtonName);
            gameMenu.Close();
            yield return null;

            Click(MenuButtonName);
            yield return null;

            Assert.IsFalse(gameMenu.IsAskingToSkip);
            Assert.IsNotNull(GameObject.Find(MenuWindowName));
        }

        [UnityTest]
        public IEnumerator ConfirmSkipButton_Clicked_RevealsTheWholeCvOnTheExpandedWaybill()
        {
            Click(MenuButtonName);
            Click(SkipGameButtonName);
            yield return null;

            Click(ConfirmSkipButtonName);
            yield return null;

            Assert.IsTrue(Object.FindAnyObjectByType<CvScreen>().RevealState.IsComplete);
            Assert.IsTrue(Object.FindAnyObjectByType<WarehouseScreenSplit>().IsWaybillExpanded);
            Assert.IsFalse(gameMenu.IsOpen);
            Assert.IsFalse(demo.Simulation.IsHeld);
        }

        [UnityTest]
        public IEnumerator RestartButton_Clicked_BootsAFreshWarehouseWithTheSameSeed()
        {
            int seed = demo.Seed;
            Click(MenuButtonName);
            yield return null;

            Click(RestartButtonName);
            yield return WaitForNewDemo(demo);

            Assert.AreEqual(seed, demo.Seed);
            Assert.AreEqual(0, demo.Simulation.Score.DeliveredPieces);
            Assert.IsFalse(demo.Simulation.IsHeld);
        }

        [UnityTest]
        public IEnumerator MainMenuButton_Clicked_LoadsTheMainMenu()
        {
            Click(MenuButtonName);
            yield return null;

            Click(MainMenuButtonName);
            for (int frame = 0; frame < MaxFramesToWait && Object.FindAnyObjectByType<MainMenu>() == null; frame++)
                yield return null;

            Assert.IsNotNull(Object.FindAnyObjectByType<MainMenu>());
            Assert.IsNull(Object.FindAnyObjectByType<WarehouseDemo>());
        }

        private static void Click(string buttonName)
        {
            GameObject.Find(buttonName).GetComponent<Button>().onClick.Invoke();
        }

        private static string LabelOf(string buttonName)
        {
            return GameObject.Find(buttonName).GetComponentInChildren<TMP_Text>().text;
        }

        private IEnumerator WaitForNewDemo(WarehouseDemo oldDemo)
        {
            for (int frame = 0; frame < MaxFramesToWait; frame++)
            {
                yield return null;
                demo = Object.FindAnyObjectByType<WarehouseDemo>();
                if (demo != null && demo != oldDemo && demo.Simulation != null)
                    yield break;
            }

            Assert.Fail("The warehouse was not restarted in time.");
        }
    }
}
