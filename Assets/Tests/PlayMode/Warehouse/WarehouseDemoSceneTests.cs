using System.Collections;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Cv;
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
    public sealed class WarehouseDemoSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const string BuyBotButtonName = "BuyBotButton";
        private const string PointsTextName = "PointsText";
        private const string BotWindowName = "BotWindow";
        private const string BotNameTextName = "BotNameText";
        private const string BotCarryTextName = "BotCarryText";
        private const string BotUpgradeCarryButtonName = "BotUpgradeCarryButton";
        private const string DeleteBotButtonName = "DeleteBotButton";
        private const string CloseBotWindowButtonName = "CloseBotWindowButton";
        private const string PreviousFocusButtonName = "PreviousFocusButton";
        private const string NextFocusButtonName = "NextFocusButton";
        private const string FocusNameTextName = "FocusNameText";
        private const string BotGroupWindowName = "BotGroupWindow";
        private const string BotGroupTextName = "BotGroupText";
        private const string DeleteBotsButtonName = "DeleteBotsButton";
        private const string PauseButtonName = "PauseButton";
        private const string PausedBadgeName = "PausedBadge";
        private const float PositionTolerance = 0.001f;
        private const int StartingBots = 3;
        private const string WaybillPanelPath = "CvCanvas/Panel";
        private const string ExpandButtonName = "ExpandButton";
        private const int MaxTicks = 600000;
        private const int MaxFramesToWait = 300;
        private const float AnchorTolerance = 0.0001f;

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
        public IEnumerator BuyBotButton_NotEnoughPoints_IsDisabled()
        {
            yield return null;

            Assert.IsFalse(GameObject.Find(BuyBotButtonName).GetComponent<Button>().interactable);
            Assert.AreEqual("0 points", GameObject.Find(PointsTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator BuyBotButton_ClickedWithEnoughPoints_AddsABotAndSpendsItsPrice()
        {
            Button buyBotButton = GameObject.Find(BuyBotButtonName).GetComponent<Button>();
            TickUntil(() => demo.Shop.CanBuyBot);
            int pointsBefore = demo.Shop.Balance;
            int botPrice = demo.Shop.BotPrice;
            yield return null;
            Assert.IsTrue(buyBotButton.interactable);

            buyBotButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(StartingBots + 1, Object.FindAnyObjectByType<WarehouseBotsView>().VisibleBotCount);
            Assert.AreEqual(pointsBefore - botPrice, demo.Shop.Balance);
            Assert.AreEqual(demo.Shop.Balance + " points", GameObject.Find(PointsTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator BotWindow_NoBotSelected_IsHiddenAlongWithTheHighlightAndTheRoute()
        {
            yield return null;

            Assert.IsNull(GameObject.Find(BotWindowName));
            Assert.AreEqual(0, Object.FindAnyObjectByType<WarehouseBotsView>().VisibleHighlightCount);
            Assert.AreEqual(0, Object.FindAnyObjectByType<BotRouteView>().VisiblePointCount);
        }

        [UnityTest]
        public IEnumerator BotSelector_TapOnABot_OpensItsWindowAndHighlightsIt()
        {
            Bot secondBot = demo.Simulation.Bots[1];
            WarehouseBotsView botsView = Object.FindAnyObjectByType<WarehouseBotsView>();

            TapOn(secondBot);
            yield return null;

            Assert.AreSame(secondBot, demo.Selection.SingleBot);
            Assert.AreEqual("Bot 2", GameObject.Find(BotNameTextName).GetComponent<TMP_Text>().text);
            Assert.AreEqual("Carries letters", GameObject.Find(BotCarryTextName).GetComponent<TMP_Text>().text);
            Assert.AreEqual(1, botsView.VisibleHighlightCount);
            Assert.AreEqual(secondBot.CentreX, botsView.HighlightPosition(0).x, PositionTolerance);
            Assert.AreEqual(secondBot.CentreY, botsView.HighlightPosition(0).y, PositionTolerance);
        }

        [UnityTest]
        public IEnumerator BotSelector_TapOnEmptyFloor_ClosesTheBotWindow()
        {
            TapOn(demo.Simulation.Bots[0]);
            yield return null;

            Object.FindAnyObjectByType<WarehouseBotSelector>().SelectAt(new Vector2(-10f, -10f));
            yield return null;

            Assert.AreEqual(0, demo.Selection.Count);
            Assert.IsNull(GameObject.Find(BotWindowName));
        }

        [UnityTest]
        public IEnumerator CloseBotWindowButton_Clicked_ClearsTheSelection()
        {
            TapOn(demo.Simulation.Bots[0]);
            yield return null;

            GameObject.Find(CloseBotWindowButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(0, demo.Selection.Count);
            Assert.IsNull(GameObject.Find(BotWindowName));
        }

        [UnityTest]
        public IEnumerator RouteView_SelectedBotOnItsWay_DrawsTheRouteToItsDestination()
        {
            Bot bot = demo.Simulation.Bots[0];
            LineRenderer route = Object.FindAnyObjectByType<BotRouteView>().GetComponent<LineRenderer>();
            demo.Simulation.Tick();
            Assert.IsTrue(bot.IsMoving);

            TapOn(bot);
            yield return null;

            Assert.GreaterOrEqual(route.positionCount, 2);
            Vector3 routeStart = route.GetPosition(0);
            Vector3 routeEnd = route.GetPosition(route.positionCount - 1);
            Assert.AreEqual(bot.CentreX, routeStart.x, PositionTolerance);
            Assert.AreEqual(bot.CentreY, routeStart.y, PositionTolerance);
            Assert.AreEqual(bot.Destination.X + 0.5f, routeEnd.x, PositionTolerance);
            Assert.AreEqual(bot.Destination.Y + 0.5f, routeEnd.y, PositionTolerance);
            Assert.Less(route.startColor.a, 1f);
            Assert.Greater(route.numCornerVertices, 0);
        }

        [UnityTest]
        public IEnumerator DeleteBotButton_Clicked_RemovesTheSelectedBotAndClosesItsWindow()
        {
            Bot secondBot = demo.Simulation.Bots[1];
            TapOn(secondBot);
            yield return null;

            GameObject.Find(DeleteBotButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            CollectionAssert.DoesNotContain(demo.Simulation.Bots, secondBot);
            Assert.AreEqual(StartingBots - 1, Object.FindAnyObjectByType<WarehouseBotsView>().VisibleBotCount);
            Assert.IsNull(GameObject.Find(BotWindowName));
        }

        [UnityTest]
        public IEnumerator DeleteBotButton_OnlyOneBotLeft_IsDisabled()
        {
            WarehouseSimulation simulation = demo.Simulation;
            while (simulation.CanRemoveBot)
                simulation.TryRemoveBot(simulation.Bots[simulation.Bots.Count - 1]);

            TapOn(simulation.Bots[0]);
            yield return null;

            Assert.IsFalse(GameObject.Find(DeleteBotButtonName).GetComponent<Button>().interactable);
        }

        [UnityTest]
        public IEnumerator BotUpgradeCarryButton_ClickedWithEnoughPoints_MakesOnlyThatBotCarryWords()
        {
            Bot firstBot = demo.Simulation.Bots[0];
            TapOn(firstBot);
            yield return null;
            Button upgradeCarryButton = GameObject.Find(BotUpgradeCarryButtonName).GetComponent<Button>();
            Assert.IsFalse(upgradeCarryButton.interactable);

            TickUntil(() => demo.Shop.CanUpgradeCarry(firstBot));
            yield return null;
            Assert.IsTrue(upgradeCarryButton.interactable);

            upgradeCarryButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(CarrySize.Word, firstBot.CarrySize);
            Assert.AreEqual(CarrySize.Letter, demo.Simulation.Bots[1].CarrySize);
            Assert.AreEqual("Carries words", GameObject.Find(BotCarryTextName).GetComponent<TMP_Text>().text);
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

        [UnityTest]
        public IEnumerator Scene_Booted_SplitsTheScreenBetweenTheWarehouseAndTheWaybill()
        {
            yield return null;
            var waybillPanel = (RectTransform)Object.FindAnyObjectByType<CvScreen>().transform.Find(WaybillPanelPath);
            Rect warehouseView = Object.FindAnyObjectByType<WarehouseCamera>().GetComponent<Camera>().rect;

            Assert.Less(warehouseView.xMax, 1f);
            Assert.AreEqual(warehouseView.xMax, waybillPanel.anchorMin.x, AnchorTolerance);
            Assert.AreEqual(1f, waybillPanel.anchorMax.x, AnchorTolerance);
        }

        [UnityTest]
        public IEnumerator Scene_Booted_ShowsTheWaybillWithEveryBlockHidden()
        {
            CvScreen waybill = Object.FindAnyObjectByType<CvScreen>();

            yield return WaitForTheCv(waybill);
            yield return null;

            Assert.Greater(waybill.RevealState.Blocks.Count, 0);
            Assert.AreEqual(0, waybill.RevealState.RevealedLetterCount);
        }

        [UnityTest]
        public IEnumerator Simulation_RunToTheEnd_RevealsTheWholeCv()
        {
            CvScreen waybill = Object.FindAnyObjectByType<CvScreen>();
            WarehouseSimulation simulation = demo.Simulation;
            while (simulation.TryAddBot())
            {
            }

            yield return WaitForTheCv(waybill);
            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
                simulation.Tick();
            yield return null;

            Assert.IsTrue(waybill.RevealState.IsComplete);
        }

        [UnityTest]
        public IEnumerator ExpandButton_Clicked_ShowsTheWaybillFullScreenAndHidesTheWarehouse()
        {
            var waybillPanel = (RectTransform)Object.FindAnyObjectByType<CvScreen>().transform.Find(WaybillPanelPath);
            WarehouseCamera warehouseCamera = Object.FindAnyObjectByType<WarehouseCamera>();
            WarehouseHudView hud = Object.FindAnyObjectByType<WarehouseHudView>();

            GameObject.Find(ExpandButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(0f, waybillPanel.anchorMin.x, AnchorTolerance);
            Assert.IsFalse(warehouseCamera.gameObject.activeSelf);
            Assert.IsFalse(hud.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator ExpandButton_ClickedTwice_BringsTheWarehouseBack()
        {
            var waybillPanel = (RectTransform)Object.FindAnyObjectByType<CvScreen>().transform.Find(WaybillPanelPath);
            WarehouseCamera warehouseCamera = Object.FindAnyObjectByType<WarehouseCamera>();
            Button expandButton = GameObject.Find(ExpandButtonName).GetComponent<Button>();

            expandButton.onClick.Invoke();
            yield return null;
            expandButton.onClick.Invoke();
            yield return null;

            Assert.Greater(waybillPanel.anchorMin.x, 0f);
            Assert.IsTrue(warehouseCamera.gameObject.activeSelf);
            Assert.AreEqual(waybillPanel.anchorMin.x, warehouseCamera.GetComponent<Camera>().rect.xMax, AnchorTolerance);
        }

        [UnityTest]
        public IEnumerator PauseButton_Clicked_StopsTheSimulationAndShowsThePausedBadge()
        {
            Button pauseButton = GameObject.Find(PauseButtonName).GetComponent<Button>();
            Assert.IsNull(GameObject.Find(PausedBadgeName));
            demo.enabled = true;

            pauseButton.onClick.Invoke();
            int ticksWhenPaused = demo.Simulation.TickCount;
            yield return null;
            yield return null;

            Assert.IsTrue(demo.Simulation.IsPaused);
            Assert.AreEqual(ticksWhenPaused, demo.Simulation.TickCount);
            Assert.IsNotNull(GameObject.Find(PausedBadgeName));
            Assert.AreEqual("Resume (Space)", pauseButton.GetComponentInChildren<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator PauseButton_ClickedTwice_ResumesTheSimulation()
        {
            Button pauseButton = GameObject.Find(PauseButtonName).GetComponent<Button>();

            pauseButton.onClick.Invoke();
            pauseButton.onClick.Invoke();
            yield return null;

            Assert.IsFalse(demo.Simulation.IsPaused);
            Assert.IsNull(GameObject.Find(PausedBadgeName));
            Assert.AreEqual("Pause (Space)", pauseButton.GetComponentInChildren<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator BotWindow_WhilePaused_StillSelectsBotsAndChangesTheirFocus()
        {
            Bot firstBot = demo.Simulation.Bots[0];
            GameObject.Find(PauseButtonName).GetComponent<Button>().onClick.Invoke();

            TapOn(firstBot);
            yield return null;
            GameObject.Find(NextFocusButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(BotFocus.BiggestBox, firstBot.Focus);
        }

        [UnityTest]
        public IEnumerator NextFocusButton_Clicked_ShowsTheNextFocusOfTheSelectedBotOnly()
        {
            Bot firstBot = demo.Simulation.Bots[0];
            TapOn(firstBot);
            yield return null;
            TMP_Text focusName = GameObject.Find(FocusNameTextName).GetComponent<TMP_Text>();
            Button nextButton = GameObject.Find(NextFocusButtonName).GetComponent<Button>();
            Assert.AreEqual("Closest box", focusName.text);

            nextButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(BotFocus.BiggestBox, firstBot.Focus);
            Assert.AreEqual(BotFocus.ClosestBox, demo.Simulation.Bots[1].Focus);
            Assert.AreEqual("Biggest box", focusName.text);

            nextButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(BotFocus.ClosestBox, firstBot.Focus);
            Assert.AreEqual("Closest box", focusName.text);
        }

        [UnityTest]
        public IEnumerator PreviousFocusButton_Clicked_ShowsThePreviousFocus()
        {
            Bot firstBot = demo.Simulation.Bots[0];
            TapOn(firstBot);
            yield return null;

            GameObject.Find(PreviousFocusButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(BotFocus.BiggestBox, firstBot.Focus);
            Assert.AreEqual("Biggest box", GameObject.Find(FocusNameTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator NextFocusButton_ClickedWhileTheBotHeadsForABox_RedrawsItsRouteToTheNewBox()
        {
            Bot bot = demo.Simulation.Bots[0];
            LineRenderer route = Object.FindAnyObjectByType<BotRouteView>().GetComponent<LineRenderer>();
            demo.Simulation.Tick();
            TapOn(bot);
            yield return null;

            GameObject.Find(NextFocusButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(BotState.GoingToBox, bot.State);
            Assert.AreEqual(WeightClass.Heavy, bot.JobBox.WeightClass);
            Assert.AreEqual(bot.AccessCell, bot.Destination);
            Vector3 routeEnd = route.GetPosition(route.positionCount - 1);
            Assert.AreEqual(bot.AccessCell.X + 0.5f, routeEnd.x, PositionTolerance);
            Assert.AreEqual(bot.AccessCell.Y + 0.5f, routeEnd.y, PositionTolerance);
        }

        [UnityTest]
        public IEnumerator BotSelector_BoxAroundSeveralBots_HighlightsEachAndShowsTheGroupWindow()
        {
            WarehouseBotsView botsView = Object.FindAnyObjectByType<WarehouseBotsView>();
            demo.Simulation.Tick();

            Object.FindAnyObjectByType<WarehouseBotSelector>().SelectInside(AreaAround(demo.Simulation.Bots));
            yield return null;

            Assert.AreEqual(StartingBots, demo.Selection.Count);
            Assert.AreEqual(StartingBots, botsView.VisibleHighlightCount);
            for (int index = 0; index < StartingBots; index++)
            {
                Assert.AreEqual(demo.Simulation.Bots[index].CentreX, botsView.HighlightPosition(index).x, PositionTolerance);
                Assert.AreEqual(demo.Simulation.Bots[index].CentreY, botsView.HighlightPosition(index).y, PositionTolerance);
            }

            Assert.AreEqual("Multiple bots selected", GameObject.Find(BotGroupTextName).GetComponent<TMP_Text>().text);
            Assert.IsNotNull(GameObject.Find(BotGroupWindowName));
            Assert.IsNull(GameObject.Find(BotWindowName));
            Assert.AreEqual(0, Object.FindAnyObjectByType<BotRouteView>().VisiblePointCount);
        }

        [UnityTest]
        public IEnumerator BotSelector_BoxAroundOneBot_OpensTheSingleBotWindow()
        {
            Bot secondBot = demo.Simulation.Bots[1];
            var area = new SelectionArea(secondBot.CentreX - 0.4f, secondBot.CentreY - 0.4f, secondBot.CentreX + 0.4f, secondBot.CentreY + 0.4f);

            Object.FindAnyObjectByType<WarehouseBotSelector>().SelectInside(area);
            yield return null;

            Assert.AreSame(secondBot, demo.Selection.SingleBot);
            Assert.IsNotNull(GameObject.Find(BotWindowName));
            Assert.IsNull(GameObject.Find(BotGroupWindowName));
        }

        [UnityTest]
        public IEnumerator BotSelector_BoxAroundNoBots_ClearsTheSelection()
        {
            WarehouseBotSelector selector = Object.FindAnyObjectByType<WarehouseBotSelector>();
            selector.SelectInside(AreaAround(demo.Simulation.Bots));
            yield return null;

            selector.SelectInside(new SelectionArea(-20f, -20f, -10f, -10f));
            yield return null;

            Assert.AreEqual(0, demo.Selection.Count);
            Assert.IsNull(GameObject.Find(BotGroupWindowName));
            Assert.AreEqual(0, Object.FindAnyObjectByType<WarehouseBotsView>().VisibleHighlightCount);
        }

        [UnityTest]
        public IEnumerator DeleteBotsButton_SomeBotsSelected_RemovesThemAndClosesTheGroupWindow()
        {
            Bot keptBot = demo.Simulation.Bots[0];
            var doomedBots = new System.Collections.Generic.List<Bot> { demo.Simulation.Bots[1], demo.Simulation.Bots[2] };
            demo.Selection.SelectAll(doomedBots);
            yield return null;

            GameObject.Find(DeleteBotsButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(1, demo.Simulation.Bots.Count);
            Assert.AreSame(keptBot, demo.Simulation.Bots[0]);
            Assert.AreEqual(0, demo.Selection.Count);
            Assert.IsNull(GameObject.Find(BotGroupWindowName));
        }

        [UnityTest]
        public IEnumerator DeleteBotsButton_EveryBotSelected_KeepsOneBotSoTheGameCanBeFinished()
        {
            Object.FindAnyObjectByType<WarehouseBotSelector>().SelectInside(AreaAround(demo.Simulation.Bots));
            yield return null;

            GameObject.Find(DeleteBotsButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(1, demo.Simulation.Bots.Count);
            Assert.AreEqual(0, demo.Selection.Count);
        }

        [UnityTest]
        public IEnumerator SelectionBox_NoDragInProgress_IsHidden()
        {
            yield return null;

            Assert.IsFalse(Object.FindAnyObjectByType<SelectionBoxView>().IsVisible);
        }

        [UnityTest]
        public IEnumerator SelectionBox_Drawn_CoversTheDraggedArea()
        {
            SelectionBoxView boxView = Object.FindAnyObjectByType<SelectionBoxView>();
            yield return null;

            boxView.Draw(new SelectionArea(4f, 9f, 1f, 3f));

            Assert.IsTrue(boxView.IsVisible);
            Assert.AreEqual(3f, boxView.Size.x, PositionTolerance);
            Assert.AreEqual(6f, boxView.Size.y, PositionTolerance);

            boxView.Hide();

            Assert.IsFalse(boxView.IsVisible);
        }

        private static SelectionArea AreaAround(System.Collections.Generic.IReadOnlyList<Bot> bots)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            foreach (Bot bot in bots)
            {
                minX = Mathf.Min(minX, bot.CentreX);
                minY = Mathf.Min(minY, bot.CentreY);
                maxX = Mathf.Max(maxX, bot.CentreX);
                maxY = Mathf.Max(maxY, bot.CentreY);
            }

            return new SelectionArea(minX - 0.5f, minY - 0.5f, maxX + 0.5f, maxY + 0.5f);
        }

        private static void TapOn(Bot bot)
        {
            Object.FindAnyObjectByType<WarehouseBotSelector>().SelectAt(new Vector2(bot.CentreX, bot.CentreY));
        }

        private void TickUntil(System.Func<bool> isReached)
        {
            for (int tick = 0; tick < MaxTicks && !isReached(); tick++)
                demo.Simulation.Tick();

            Assert.IsTrue(isReached(), "The simulation never got that far.");
        }

        private static IEnumerator WaitForTheCv(CvScreen waybill)
        {
            for (int frame = 0; frame < MaxFramesToWait && waybill.RevealState == null; frame++)
                yield return null;

            Assert.IsNotNull(waybill.RevealState, "The CV was not loaded in time.");
        }
    }
}
