using System.Collections;
using CvWarehouse.Core.Menu;
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

namespace CvWarehouse.Tests.PlayMode.Menu
{
    public sealed class MainMenuSceneTests
    {
        private const string ScenePath = "Assets/Scenes/MainMenu.unity";
        private const string TitleTextName = "TitleText";
        private const string StartShiftButtonName = "StartShiftButton";
        private const string SubtitleName = "Subtitle";
        private const string GitHubLinkName = "GitHubLink";
        private const string LinkedInLinkName = "LinkedInLink";
        private const string VersionTextName = "VersionText";
        private static readonly string[] MenuButtonNames = { StartShiftButtonName, "ViewCvButton", "HowToPlayButton", "SettingsButton", "SoundButton" };
        private const string BotBodyName = "Body";
        private const string CargoName = "Cargo";
        private const int MaxFramesToWait = 300;
        private const int MaxSteps = 1000;
        private const float StepSeconds = 0.02f;
        private const float MinimumWalkCells = 4f;

        [UnitySetUp]
        public IEnumerator LoadMenuScene()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return null;
        }

        [UnityTest]
        public IEnumerator Scene_Loaded_ShowsTheTitleAndEveryMenuButton()
        {
            yield return null;

            Assert.AreEqual("cv-warehouse", GameObject.Find(TitleTextName).GetComponent<TMP_Text>().text);
            foreach (string buttonName in MenuButtonNames)
                Assert.IsTrue(GameObject.Find(buttonName).GetComponent<Button>().interactable, buttonName);
            Assert.IsNotNull(GameObject.Find(StartShiftButtonName).GetComponentInParent<VerticalLayoutGroup>());
        }

        [UnityTest]
        public IEnumerator Subtitle_CvLoaded_NamesThePersonFromTheCvFile()
        {
            TMP_Text subtitle = GameObject.Find(SubtitleName).GetComponent<TMP_Text>();
            for (int frame = 0; frame < MaxFramesToWait && subtitle.text == MenuSubtitle.WithoutName; frame++)
                yield return null;

            StringAssert.StartsWith(MenuSubtitle.WithoutName + " by ", subtitle.text);
        }

        [UnityTest]
        public IEnumerator Footer_Loaded_ShowsTheLinksAndTheBuildVersion()
        {
            yield return null;

            Assert.IsNotNull(GameObject.Find(GitHubLinkName));
            Assert.IsNotNull(GameObject.Find(LinkedInLinkName));
            StringAssert.Contains(Application.version, GameObject.Find(VersionTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator Backdrop_AtTheTargetBox_BotHasWalkedOverWithPaperOnItsHead()
        {
            MenuBotView botView = Object.FindAnyObjectByType<MenuBotView>();
            Transform body = GameObject.Find(BotBodyName).transform;
            yield return StepUntil(botView.Loop, MenuBotPhase.TakingOut);
            float sourceX = body.position.x;

            yield return StepUntil(botView.Loop, MenuBotPhase.PuttingIn);

            Assert.Greater(body.position.x - sourceX, MinimumWalkCells);
            Assert.IsTrue(GameObject.Find(CargoName).GetComponent<SpriteRenderer>().enabled);
            Assert.Greater(body.GetComponent<SpriteRenderer>().sortingOrder, DepthSorting.OnTheFloorOrder);
        }

        [UnityTest]
        public IEnumerator Backdrop_WalkingBack_BotHasEmptyHands()
        {
            MenuBotView botView = Object.FindAnyObjectByType<MenuBotView>();

            yield return StepUntil(botView.Loop, MenuBotPhase.Returning);

            Assert.IsFalse(GameObject.Find(CargoName).GetComponent<SpriteRenderer>().enabled);
            Assert.Greater(botView.Loop.DeliveredTrips, 0);
        }

        [UnityTest]
        public IEnumerator StartShiftButton_Clicked_LoadsTheWarehouse()
        {
            GameObject.Find(StartShiftButtonName).GetComponent<Button>().onClick.Invoke();
            for (int frame = 0; frame < MaxFramesToWait && Object.FindAnyObjectByType<WarehouseDemo>() == null; frame++)
                yield return null;

            Assert.IsNotNull(Object.FindAnyObjectByType<WarehouseDemo>());
            Assert.IsNull(Object.FindAnyObjectByType<MainMenu>());
        }

        private static IEnumerator StepUntil(MenuBotLoop loop, MenuBotPhase phase)
        {
            for (int step = 0; step < MaxSteps && loop.Phase != phase; step++)
                loop.Advance(StepSeconds);
            yield return null;

            Assert.AreEqual(phase, loop.Phase, "The menu bot never reached " + phase + ".");
        }
    }
}
