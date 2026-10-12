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
        private const string BotStatusTextName = "BotStatusText";
        private const string ShiftStatusTextName = "ShiftStatusText";
        private const string CvProgressTextName = "CvProgressText";
        private const string TopBarName = "TopBar";
        private const string BotCarryTextName = "BotCarryText";
        private const string BotUpgradeCarryButtonName = "BotUpgradeCarryButton";
        private const string BotSpeedTextName = "BotSpeedText";
        private const string BotUpgradeSpeedButtonName = "BotUpgradeSpeedButton";
        private const string DeleteBotButtonName = "DeleteBotButton";
        private const string CloseBotWindowButtonName = "CloseBotWindowButton";
        private const string PreviousFocusButtonName = "PreviousFocusButton";
        private const string NextFocusButtonName = "NextFocusButton";
        private const string FocusNameTextName = "FocusNameText";
        private const string BotGroupWindowName = "BotGroupWindow";
        private const string BotGroupTextName = "BotGroupText";
        private const string DeleteBotsButtonName = "DeleteBotsButton";
        private const string BoxWindowName = "BoxWindow";
        private const string BoxWeightTextName = "BoxWeightText";
        private const string BoxCategoryTextName = "BoxCategoryText";
        private const string CloseBoxWindowButtonName = "CloseBoxWindowButton";
        private const string PauseButtonName = "PauseButton";
        private const string PausedBadgeName = "PausedBadge";
        private const float PositionTolerance = 0.001f;
        private const int StartingBots = 3;
        private const string WaybillPanelPath = "CvCanvas/Panel";
        private const string ExpandButtonName = "ExpandButton";
        private const string AutoButtonName = "AutoButton";
        private const int MaxTicks = 600000;
        private const int MaxFramesToWait = 300;
        private const float AnchorTolerance = 0.0001f;
        private const float SpriteMargin = 0.2f;
        private const float HighlightSeconds = 2f;
        private const float LongAfterAnyHighlight = 60f;
        private const float FarDownThePage = 100f;
        private const int FramesToSettle = 30;

        private WarehouseDemo demo;

        [UnitySetUp]
        public IEnumerator LoadDemoScene()
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
        }

        [UnityTest]
        public IEnumerator Scene_Booted_HasOneBoxForEveryBlockOfTheCv()
        {
            yield return null;
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;

            Assert.AreEqual(waybill.Blocks.Count, demo.Simulation.Layout.Boxes.Count);
            Assert.AreEqual(waybill.TotalLetterCount, demo.Simulation.Layout.TotalPieces);
            foreach (Box box in demo.Simulation.Layout.Boxes)
            {
                Assert.IsTrue(waybill.TryGetBlock(box.Block.Id, out CvBlockReveal block));
                Assert.AreEqual(block.Block.LetterCount, box.TotalPieces);
            }
        }

        [UnityTest]
        public IEnumerator Simulation_FirstDelivery_ShowsThoseLettersInTheBlockOfTheirBox()
        {
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;
            WarehouseSimulation simulation = demo.Simulation;
            var deliveredPerBlock = new System.Collections.Generic.Dictionary<string, int>();
            simulation.Score.Delivered += (block, letters) =>
            {
                deliveredPerBlock.TryGetValue(block.Id, out int deliveredSoFar);
                deliveredPerBlock[block.Id] = deliveredSoFar + letters.Count;
            };

            for (int tick = 0; tick < MaxTicks && simulation.Score.DeliveredPieces == 0; tick++)
                simulation.Tick();
            yield return null;

            Assert.Greater(simulation.Score.DeliveredPieces, 0);
            Assert.AreEqual(simulation.Score.DeliveredPieces, waybill.RevealedLetterCount);
            foreach (System.Collections.Generic.KeyValuePair<string, int> delivered in deliveredPerBlock)
            {
                Assert.IsTrue(waybill.TryGetBlock(delivered.Key, out CvBlockReveal block));
                Assert.AreEqual(delivered.Value, block.RevealedCount);
            }
        }

        [UnityTest]
        public IEnumerator BuyBotButton_NotEnoughPoints_IsDisabled()
        {
            yield return null;

            Assert.IsFalse(GameObject.Find(BuyBotButtonName).GetComponent<Button>().interactable);
            Assert.AreEqual("0", GameObject.Find(PointsTextName).GetComponent<TMP_Text>().text);
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
            Assert.AreEqual(demo.Shop.Balance.ToString(), GameObject.Find(PointsTextName).GetComponent<TMP_Text>().text);
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
            Assert.AreEqual("BLIP", GameObject.Find(BotNameTextName).GetComponent<TMP_Text>().text);
            Assert.AreEqual("Words per haul: 1", GameObject.Find(BotCarryTextName).GetComponent<TMP_Text>().text);
            Assert.AreEqual("Speed: 1", GameObject.Find(BotSpeedTextName).GetComponent<TMP_Text>().text);
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
        public IEnumerator BotUpgradeCarryButton_ClickedWithEnoughPoints_MakesOnlyThatBotCarryTwoWords()
        {
            Bot firstBot = demo.Simulation.Bots[0];
            TapOn(firstBot);
            yield return null;
            Button upgradeCarryButton = GameObject.Find(BotUpgradeCarryButtonName).GetComponent<Button>();
            Assert.IsFalse(upgradeCarryButton.interactable);

            TickUntil(() => demo.Shop.CanUpgrade(firstBot, BotUpgrade.Carry));
            int price = demo.Shop.UpgradePrice(firstBot, BotUpgrade.Carry);
            int pointsBefore = demo.Shop.Balance;
            yield return null;
            Assert.IsTrue(upgradeCarryButton.interactable);
            Assert.AreEqual("More words (" + price + ")", upgradeCarryButton.GetComponentInChildren<TMP_Text>().text);

            upgradeCarryButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, firstBot.CarryLevel);
            Assert.AreEqual(1, firstBot.SpeedLevel);
            Assert.AreEqual(1, demo.Simulation.Bots[1].CarryLevel);
            Assert.AreEqual(pointsBefore - price, demo.Shop.Balance);
            Assert.AreEqual("Words per haul: 2", GameObject.Find(BotCarryTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator BotUpgradeSpeedButton_ClickedWithEnoughPoints_MakesOnlyThatBotFaster()
        {
            Bot firstBot = demo.Simulation.Bots[0];
            TapOn(firstBot);
            yield return null;
            Button upgradeSpeedButton = GameObject.Find(BotUpgradeSpeedButtonName).GetComponent<Button>();
            Assert.IsFalse(upgradeSpeedButton.interactable);

            TickUntil(() => demo.Shop.CanUpgrade(firstBot, BotUpgrade.Speed));
            int price = demo.Shop.UpgradePrice(firstBot, BotUpgrade.Speed);
            yield return null;
            Assert.IsTrue(upgradeSpeedButton.interactable);
            Assert.AreEqual("More speed (" + price + ")", upgradeSpeedButton.GetComponentInChildren<TMP_Text>().text);

            upgradeSpeedButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, firstBot.SpeedLevel);
            Assert.AreEqual(1, firstBot.CarryLevel);
            Assert.AreEqual(1, demo.Simulation.Bots[1].SpeedLevel);
            Assert.AreEqual("Speed: 2", GameObject.Find(BotSpeedTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator BotUpgradeButtons_BotAtTheLastLevel_AreHidden()
        {
            Bot firstBot = demo.Simulation.Bots[0];
            while (demo.Shop.HasUpgrade(firstBot, BotUpgrade.Carry))
                demo.Simulation.Upgrade(firstBot, BotUpgrade.Carry);
            while (demo.Shop.HasUpgrade(firstBot, BotUpgrade.Speed))
                demo.Simulation.Upgrade(firstBot, BotUpgrade.Speed);

            TapOn(firstBot);
            yield return null;

            Assert.IsNull(GameObject.Find(BotUpgradeCarryButtonName));
            Assert.IsNull(GameObject.Find(BotUpgradeSpeedButtonName));
            Assert.AreEqual("Words per haul: 5", GameObject.Find(BotCarryTextName).GetComponent<TMP_Text>().text);
            Assert.AreEqual("Speed: 5", GameObject.Find(BotSpeedTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator Simulation_RunToTheEnd_LeavesAnEmptiedBoxOnTheFloorWhereEveryBoxStood()
        {
            WarehouseSimulation simulation = demo.Simulation;
            WarehouseBoxesView boxesView = Object.FindAnyObjectByType<WarehouseBoxesView>();
            WarehouseBotsView botsView = Object.FindAnyObjectByType<WarehouseBotsView>();
            while (simulation.TryAddBot())
            {
            }

            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
                simulation.Tick();
            yield return null;

            Assert.IsTrue(simulation.IsComplete);
            for (int index = 0; index < simulation.Layout.Boxes.Count; index++)
            {
                StringAssert.EndsWith("Empty", boxesView.ShownSprite(index).name);
                Assert.Less(boxesView.SortingOrder(index), botsView.Figure(0).SortingOrder);
            }
        }

        [UnityTest]
        public IEnumerator BoxesView_HeavyBoxPartlyEmptied_ShowsItOpen()
        {
            WarehouseBoxesView boxesView = Object.FindAnyObjectByType<WarehouseBoxesView>();
            int heavyBox = IndexOfFirstBox(candidate => candidate.WeightClass == WeightClass.Heavy);
            Box box = demo.Simulation.Layout.Boxes[heavyBox];
            yield return null;
            Assert.AreEqual("BoxHeavy", boxesView.ShownSprite(heavyBox).name);

            TickUntil(() => box.RemainingPieces < box.TotalPieces);
            yield return null;

            Assert.IsFalse(box.IsEmpty);
            Assert.AreEqual("BoxHeavyOpen", boxesView.ShownSprite(heavyBox).name);
        }

        [UnityTest]
        public IEnumerator BoxesView_LightBoxPartlyEmptied_StaysClosed()
        {
            WarehouseBoxesView boxesView = Object.FindAnyObjectByType<WarehouseBoxesView>();
            int startedBox = -1;

            TickUntil(() => (startedBox = IndexOfFirstBox(IsPartlyEmptiedLightBox)) >= 0);
            yield return null;

            Assert.AreEqual("BoxLight", boxesView.ShownSprite(startedBox).name);
        }

        [UnityTest]
        public IEnumerator BoxHighlight_HeavyBoxSelected_FramesItsWholeFootprint()
        {
            Box heavyBox = FirstBox(candidate => candidate.WeightClass == WeightClass.Heavy);
            BoxHighlightView highlight = Object.FindAnyObjectByType<BoxHighlightView>();

            TapOn(heavyBox);
            yield return null;

            Vector3 frameSize = highlight.FrameSprite.bounds.size;
            Assert.GreaterOrEqual(frameSize.x, heavyBox.Size.Width);
            Assert.GreaterOrEqual(frameSize.y, heavyBox.Size.Height);
            Assert.AreEqual(heavyBox.Size.Width, frameSize.x, SpriteMargin);
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

            Assert.AreEqual(BotFocus.SmallestBox, firstBot.Focus);
            Assert.AreEqual("Smallest box", focusName.text);

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

            Assert.AreEqual(BotFocus.SmallestBox, firstBot.Focus);
            Assert.AreEqual("Smallest box", GameObject.Find(FocusNameTextName).GetComponent<TMP_Text>().text);
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

        [UnityTest]
        public IEnumerator BoxWindow_NoBoxSelected_IsHiddenAlongWithTheHighlight()
        {
            yield return null;

            Assert.IsNull(GameObject.Find(BoxWindowName));
            Assert.IsFalse(Object.FindAnyObjectByType<BoxHighlightView>().IsVisible);
        }

        [UnityTest]
        public IEnumerator BotSelector_TapOnABox_OpensItsWindowWithWeightAndCategoryAndHighlightsIt()
        {
            Box box = FirstBox(candidate => candidate.Block.CategoryName.Length > 0);
            BoxHighlightView highlight = Object.FindAnyObjectByType<BoxHighlightView>();

            TapOn(box);
            yield return null;

            Assert.AreSame(box, demo.BoxSelection.SelectedBox);
            Assert.AreEqual(
                "Weight: " + box.WeightClass.ToString().ToLowerInvariant(),
                GameObject.Find(BoxWeightTextName).GetComponent<TMP_Text>().text);
            Assert.AreEqual("Category: " + box.Block.CategoryName, GameObject.Find(BoxCategoryTextName).GetComponent<TMP_Text>().text);
            Assert.IsTrue(highlight.IsVisible);
            Assert.AreEqual(box.Origin.X + box.Size.Width * 0.5f, highlight.Position.x, PositionTolerance);
            Assert.AreEqual(box.Origin.Y + box.Size.Height * 0.5f, highlight.Position.y, PositionTolerance);
        }

        [UnityTest]
        public IEnumerator BotSelector_TapOnABoxWithoutCategory_SaysItHasNone()
        {
            Box nameBox = FirstBox(candidate => candidate.Block.Role == CvBlockRole.Name);

            TapOn(nameBox);
            yield return null;

            Assert.AreEqual("Weight: heavy", GameObject.Find(BoxWeightTextName).GetComponent<TMP_Text>().text);
            Assert.AreEqual("Category: none", GameObject.Find(BoxCategoryTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator BotSelector_TapOnAnotherBox_ShowsThatBoxInstead()
        {
            Box lightBox = FirstBox(candidate => candidate.WeightClass == WeightClass.Light);
            Box heavyBox = FirstBox(candidate => candidate.WeightClass == WeightClass.Heavy);
            TapOn(lightBox);
            yield return null;

            TapOn(heavyBox);
            yield return null;

            Assert.AreSame(heavyBox, demo.BoxSelection.SelectedBox);
            Assert.AreEqual("Weight: heavy", GameObject.Find(BoxWeightTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator BotSelector_TapOnABoxWhileABotIsSelected_SwapsTheBotWindowForTheBoxWindow()
        {
            TapOn(demo.Simulation.Bots[0]);
            yield return null;

            TapOn(demo.Simulation.Layout.Boxes[0]);
            yield return null;

            Assert.AreEqual(0, demo.Selection.Count);
            Assert.IsNull(GameObject.Find(BotWindowName));
            Assert.IsNotNull(GameObject.Find(BoxWindowName));
        }

        [UnityTest]
        public IEnumerator BotSelector_TapOnABotWhileABoxIsInspected_SwapsTheBoxWindowForTheBotWindow()
        {
            TapOn(demo.Simulation.Layout.Boxes[0]);
            yield return null;

            TapOn(demo.Simulation.Bots[0]);
            yield return null;

            Assert.IsFalse(demo.BoxSelection.HasBox);
            Assert.IsNull(GameObject.Find(BoxWindowName));
            Assert.IsNotNull(GameObject.Find(BotWindowName));
        }

        [UnityTest]
        public IEnumerator BotSelector_BoxAroundBotsWhileABoxIsInspected_ClosesTheBoxWindow()
        {
            TapOn(demo.Simulation.Layout.Boxes[0]);
            yield return null;

            Object.FindAnyObjectByType<WarehouseBotSelector>().SelectInside(AreaAround(demo.Simulation.Bots));
            yield return null;

            Assert.IsFalse(demo.BoxSelection.HasBox);
            Assert.IsNull(GameObject.Find(BoxWindowName));
        }

        [UnityTest]
        public IEnumerator BotSelector_TapOnEmptyFloor_ClosesTheBoxWindow()
        {
            TapOn(demo.Simulation.Layout.Boxes[0]);
            yield return null;

            Object.FindAnyObjectByType<WarehouseBotSelector>().SelectAt(new Vector2(-10f, -10f));
            yield return null;

            Assert.IsFalse(demo.BoxSelection.HasBox);
            Assert.IsNull(GameObject.Find(BoxWindowName));
            Assert.IsFalse(Object.FindAnyObjectByType<BoxHighlightView>().IsVisible);
        }

        [UnityTest]
        public IEnumerator CloseBoxWindowButton_Clicked_ClearsTheBoxSelection()
        {
            TapOn(demo.Simulation.Layout.Boxes[0]);
            yield return null;

            GameObject.Find(CloseBoxWindowButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsFalse(demo.BoxSelection.HasBox);
            Assert.IsNull(GameObject.Find(BoxWindowName));
        }

        [UnityTest]
        public IEnumerator BoxWindow_InspectedBoxEmptied_ClosesAlongWithTheHighlight()
        {
            Box nameBox = FirstBox(candidate => candidate.Block.Role == CvBlockRole.Name);
            TapOn(nameBox);
            yield return null;
            Assert.IsNotNull(GameObject.Find(BoxWindowName));

            TickUntil(() => nameBox.IsEmpty);
            yield return null;

            Assert.IsNull(GameObject.Find(BoxWindowName));
            Assert.IsFalse(Object.FindAnyObjectByType<BoxHighlightView>().IsVisible);
        }

        [UnityTest]
        public IEnumerator BoxWindow_WhilePaused_StillOpens()
        {
            GameObject.Find(PauseButtonName).GetComponent<Button>().onClick.Invoke();

            TapOn(demo.Simulation.Layout.Boxes[0]);
            yield return null;

            Assert.IsNotNull(GameObject.Find(BoxWindowName));
        }

        [UnityTest]
        public IEnumerator CrewList_Booted_ShowsAChipWithTheNameOfEveryBot()
        {
            CrewListView crewList = Object.FindAnyObjectByType<CrewListView>();
            yield return null;

            Assert.AreEqual(StartingBots, crewList.VisibleChipCount);
            Assert.AreEqual("PIP", crewList.Chip(0).ShownName);
            Assert.AreEqual("BLIP", crewList.Chip(1).ShownName);
            Assert.AreEqual("NIB", crewList.Chip(2).ShownName);
            Assert.IsFalse(crewList.Chip(StartingBots).gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator CrewList_ChipClicked_SelectsThatBotAndOpensItsWindow()
        {
            CrewListView crewList = Object.FindAnyObjectByType<CrewListView>();
            TapOn(demo.Simulation.Layout.Boxes[0]);
            yield return null;

            crewList.Chip(1).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreSame(demo.Simulation.Bots[1], demo.Selection.SingleBot);
            Assert.IsTrue(crewList.Chip(1).IsSelected);
            Assert.IsFalse(crewList.Chip(0).IsSelected);
            Assert.IsFalse(demo.BoxSelection.HasBox);
            Assert.AreEqual("BLIP", GameObject.Find(BotNameTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator CrewList_SelectedChipClickedAgain_ClearsTheSelection()
        {
            CrewListView crewList = Object.FindAnyObjectByType<CrewListView>();
            crewList.Chip(0).GetComponent<Button>().onClick.Invoke();
            yield return null;

            crewList.Chip(0).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(0, demo.Selection.Count);
            Assert.IsFalse(crewList.Chip(0).IsSelected);
            Assert.IsNull(GameObject.Find(BotWindowName));
        }

        [UnityTest]
        public IEnumerator CrewList_BotBoughtAndBotDeleted_FollowsTheCrew()
        {
            CrewListView crewList = Object.FindAnyObjectByType<CrewListView>();
            demo.Simulation.TryAddBot();
            yield return null;

            Assert.AreEqual(StartingBots + 1, crewList.VisibleChipCount);
            Assert.AreEqual("MOX", crewList.Chip(StartingBots).ShownName);

            demo.Simulation.TryRemoveBot(demo.Simulation.Bots[0]);
            yield return null;

            Assert.AreEqual(StartingBots, crewList.VisibleChipCount);
            Assert.AreEqual("BLIP", crewList.Chip(0).ShownName);
        }

        [UnityTest]
        public IEnumerator BotWindow_SelectedBotAtWork_DescribesWhatItIsDoing()
        {
            Bot bot = demo.Simulation.Bots[0];
            TickUntil(() => bot.State == BotState.GoingToBox);
            TapOn(bot);
            yield return null;
            TMP_Text status = GameObject.Find(BotStatusTextName).GetComponent<TMP_Text>();
            Assert.AreEqual("Walking to a box", status.text);

            TickUntil(() => bot.State == BotState.Extracting);
            yield return null;
            Assert.AreEqual("Picking up", status.text);

            TickUntil(() => bot.State == BotState.GoingToDrop);
            yield return null;
            Assert.AreEqual("Hauling 1 word", status.text);

            TickUntil(() => bot.State == BotState.Unloading);
            yield return null;
            Assert.AreEqual("Loading truck", status.text);
        }

        [UnityTest]
        public IEnumerator BotWindow_SelectedBotHaulingSeveralWords_SaysHowMany()
        {
            Bot bot = demo.Simulation.Bots[0];
            demo.Simulation.Upgrade(bot, BotUpgrade.Carry);
            TapOn(bot);

            TickUntil(() => bot.State == BotState.GoingToDrop && bot.CargoWords == 2);
            yield return null;

            Assert.AreEqual("Hauling 2 words", GameObject.Find(BotStatusTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator CvProgress_Booted_ShowsZeroPercent()
        {
            CvProgressView progress = Object.FindAnyObjectByType<CvProgressView>();
            yield return null;

            Assert.AreEqual(0, progress.ShownPercent);
            Assert.AreEqual("0%", GameObject.Find(CvProgressTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator CvProgress_AfterDeliveries_ShowsTheShareOfTheCvOnTheWaybill()
        {
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;
            CvProgressView progress = Object.FindAnyObjectByType<CvProgressView>();

            TickUntil(() => waybill.RevealedPercent >= 10);
            yield return null;

            Assert.AreEqual(waybill.RevealedPercent, progress.ShownPercent);
            Assert.AreEqual(waybill.RevealedPercent + "%", GameObject.Find(CvProgressTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator CvProgress_SimulationRunToTheEnd_ShowsOneHundredPercent()
        {
            while (demo.Simulation.TryAddBot())
            {
            }

            TickUntil(() => demo.Simulation.IsComplete);
            yield return null;

            Assert.AreEqual("100%", GameObject.Find(CvProgressTextName).GetComponent<TMP_Text>().text);
            Assert.AreEqual("SHIPPED", GameObject.Find(ShiftStatusTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator ShiftStatus_PausedAndResumed_FollowsTheSimulation()
        {
            TMP_Text status = GameObject.Find(ShiftStatusTextName).GetComponent<TMP_Text>();
            Button pauseButton = GameObject.Find(PauseButtonName).GetComponent<Button>();
            yield return null;
            Assert.AreEqual("ON SHIFT", status.text);

            pauseButton.onClick.Invoke();
            yield return null;
            Assert.AreEqual("PAUSED", status.text);

            pauseButton.onClick.Invoke();
            yield return null;
            Assert.AreEqual("ON SHIFT", status.text);
        }

        [UnityTest]
        public IEnumerator ExpandButton_Clicked_KeepsTheTopBarAndItsPointsUpToDate()
        {
            var waybillPanel = (RectTransform)Object.FindAnyObjectByType<CvScreen>().transform.Find(WaybillPanelPath);
            var topBar = (RectTransform)GameObject.Find(TopBarName).transform;

            GameObject.Find(ExpandButtonName).GetComponent<Button>().onClick.Invoke();
            TickUntil(() => demo.Shop.Balance > 0);
            yield return null;

            Assert.IsTrue(topBar.gameObject.activeInHierarchy);
            Assert.AreEqual(topBar.anchorMin.y, waybillPanel.anchorMax.y, AnchorTolerance);
            Assert.AreEqual(demo.Shop.Balance.ToString(), GameObject.Find(PointsTextName).GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator Scene_Booted_KeepsTheWarehouseViewBetweenTheTopBarAndTheDock()
        {
            yield return null;
            Rect warehouseView = Object.FindAnyObjectByType<WarehouseCamera>().GetComponent<Camera>().rect;
            var topBar = (RectTransform)GameObject.Find(TopBarName).transform;

            Assert.Greater(warehouseView.yMin, 0f);
            Assert.AreEqual(topBar.anchorMin.y, warehouseView.yMax, AnchorTolerance);
        }

        [UnityTest]
        public IEnumerator BotsView_Booted_TintsEveryBotWithItsCrewColourAndKeepsTheFaceUntinted()
        {
            WarehouseBotsView botsView = Object.FindAnyObjectByType<WarehouseBotsView>();
            yield return null;

            var pip = new Color32(0x7b, 0xd3, 0x89, 0xff);
            var blip = new Color32(0xa5, 0x93, 0xe0, 0xff);
            Assert.AreEqual(pip, (Color32)botsView.Figure(0).BodyColor);
            Assert.AreEqual(blip, (Color32)botsView.Figure(1).BodyColor);
            Assert.AreEqual("BotBodyStand", botsView.Figure(0).BodySprite.name);
            StringAssert.StartsWith("BotFaceStand", botsView.Figure(0).FaceSprite.name);
            Assert.IsFalse(botsView.Figure(StartingBots).IsVisible);
        }

        [UnityTest]
        public IEnumerator BotsView_BotHauling_ShowsAsManySheetsAsWordsItCarries()
        {
            WarehouseBotsView botsView = Object.FindAnyObjectByType<WarehouseBotsView>();
            Bot bot = demo.Simulation.Bots[0];
            yield return null;
            Assert.IsFalse(botsView.Figure(0).IsCarrying);

            TickUntil(() => bot.State == BotState.GoingToDrop);
            yield return null;
            Assert.IsTrue(botsView.Figure(0).IsCarrying);
            Assert.AreEqual("Cargo1", botsView.Figure(0).CargoSprite.name);

            demo.Simulation.Upgrade(bot, BotUpgrade.Carry);
            TickUntil(() => bot.State == BotState.GoingToDrop && bot.CargoWords == 2);
            yield return null;
            Assert.AreEqual("Cargo2", botsView.Figure(0).CargoSprite.name);
        }

        [UnityTest]
        public IEnumerator BotsView_BotAmongBoxes_IsDrawnBehindTheBoxesBelowItAndInFrontOfTheOnesAbove()
        {
            WarehouseBotsView botsView = Object.FindAnyObjectByType<WarehouseBotsView>();
            WarehouseBoxesView boxesView = Object.FindAnyObjectByType<WarehouseBoxesView>();
            Bot bot = demo.Simulation.Bots[0];
            TickUntil(() => bot.State == BotState.Extracting);
            yield return null;

            for (int index = 0; index < demo.Simulation.Layout.Boxes.Count; index++)
            {
                Box box = demo.Simulation.Layout.Boxes[index];
                bool isBotBehind = botsView.Figure(0).SortingOrder < boxesView.SortingOrder(index);
                if (bot.Cell.Y >= box.Origin.Y + box.Size.Height)
                    Assert.IsTrue(isBotBehind, "Box " + index + " is below the bot");
                else if (bot.Cell.Y < box.Origin.Y)
                    Assert.IsFalse(isBotBehind, "Box " + index + " is above the bot");
            }
        }

        [UnityTest]
        public IEnumerator BoxesView_Booted_DrawsEveryBoxAsBigAsItsFootprint()
        {
            WarehouseBoxesView boxesView = Object.FindAnyObjectByType<WarehouseBoxesView>();
            yield return null;

            for (int index = 0; index < demo.Simulation.Layout.Boxes.Count; index++)
            {
                Box box = demo.Simulation.Layout.Boxes[index];
                Vector3 size = boxesView.ShownSprite(index).bounds.size;
                Assert.AreEqual(box.Size.Width, size.x, SpriteMargin, box.WeightClass + " box width");
                Assert.AreEqual(box.Size.Height, size.y, SpriteMargin, box.WeightClass + " box height");
            }
        }

        [UnityTest]
        public IEnumerator GridView_Booted_PaintsATileOnEveryCellAndFloorUnderEveryBox()
        {
            WarehouseGridView gridView = Object.FindAnyObjectByType<WarehouseGridView>();
            WarehouseGrid grid = demo.Simulation.Layout.Grid;
            yield return null;

            for (int cellIndex = 0; cellIndex < grid.CellCount; cellIndex++)
                Assert.IsNotNull(gridView.SpriteAt(grid.PositionOf(cellIndex)));

            foreach (Box box in demo.Simulation.Layout.Boxes)
                StringAssert.StartsWith("TileFloor", gridView.SpriteAt(box.Origin).name);
            foreach (GridPosition parkingCell in demo.Simulation.Layout.ParkingCells)
                StringAssert.StartsWith("TileFloor", gridView.SpriteAt(parkingCell).name);
        }

        [UnityTest]
        public IEnumerator GridView_BoxEmptied_KeepsFloorWhereItStood()
        {
            WarehouseGridView gridView = Object.FindAnyObjectByType<WarehouseGridView>();
            Box nameBox = FirstBox(candidate => candidate.Block.Role == CvBlockRole.Name);

            TickUntil(() => nameBox.IsEmpty);
            yield return null;

            StringAssert.StartsWith("TileFloor", gridView.SpriteAt(nameBox.Origin).name);
        }

        [UnityTest]
        public IEnumerator Scanlines_Booted_CoverTheWarehouseViewWithOneLinePerPeriodAndLetClicksThrough()
        {
            ScanlinesView scanlines = Object.FindAnyObjectByType<ScanlinesView>();
            yield return null;
            var area = (RectTransform)scanlines.transform;
            var warehouseArea = (RectTransform)area.parent;

            Assert.AreEqual(warehouseArea.rect.width, area.rect.width, PositionTolerance);
            Assert.AreEqual(warehouseArea.rect.height, area.rect.height, PositionTolerance);
            Assert.AreEqual(scanlines.ScreenPixelHeight / scanlines.LinePeriodPixels, scanlines.ShownRepeats, PositionTolerance);
            Assert.Greater(scanlines.ShownRepeats, 1f);
            Assert.IsFalse(scanlines.GetComponent<RawImage>().raycastTarget);
        }

        [UnityTest]
        public IEnumerator Vignette_Booted_CoversTheWarehouseViewUnderItsFrameAndLetsClicksThrough()
        {
            VignetteView vignette = Object.FindAnyObjectByType<VignetteView>();
            yield return null;
            var area = (RectTransform)vignette.transform;
            var warehouseArea = (RectTransform)area.parent;
            RawImage image = vignette.GetComponent<RawImage>();

            Assert.AreEqual(warehouseArea.rect.width, area.rect.width, PositionTolerance);
            Assert.AreEqual(warehouseArea.rect.height, area.rect.height, PositionTolerance);
            Assert.IsTrue(image.enabled);
            Assert.IsNotNull(image.texture);
            Assert.IsFalse(image.raycastTarget);
            Assert.Less(area.GetSiblingIndex(), GameObject.Find("ViewFrame").transform.GetSiblingIndex());
        }

        [UnityTest]
        public IEnumerator Scanlines_WaybillExpanded_AreHiddenWithTheWarehouse()
        {
            ScanlinesView scanlines = Object.FindAnyObjectByType<ScanlinesView>();

            GameObject.Find(ExpandButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsFalse(scanlines.gameObject.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator Waybill_Delivery_HighlightsTheNewLettersUntilTheirTimeIsUp()
        {
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;
            CvDocumentView page = Object.FindAnyObjectByType<CvDocumentView>();
            yield return null;
            Assert.AreEqual(0, page.FreshMarkCount);

            TickUntil(() => demo.Simulation.Score.DeliveredPieces > 0);

            Assert.Greater(page.FreshMarkCount, 0);

            waybill.Advance(LongAfterAnyHighlight);

            Assert.AreEqual(0, page.FreshMarkCount);
        }

        [UnityTest]
        public IEnumerator AutoScroll_DeliveryFarDownThePage_ScrollsThereWhenNobodyTouchesTheCv()
        {
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;
            CvAutoScroll autoScroll = Object.FindAnyObjectByType<CvAutoScroll>();
            yield return null;
            Assert.IsTrue(autoScroll.IsFollowing);
            Assert.AreEqual(0f, autoScroll.Offset, PositionTolerance);

            DeliverToTheLastBlock(waybill);
            for (int frame = 0; frame < MaxFramesToWait && autoScroll.Offset < FarDownThePage; frame++)
                yield return null;

            Assert.GreaterOrEqual(autoScroll.Offset, FarDownThePage);
        }

        [UnityTest]
        public IEnumerator AutoScroll_PointerOverTheCv_LeavesThePageWhereItIs()
        {
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;
            CvAutoScroll autoScroll = Object.FindAnyObjectByType<CvAutoScroll>();
            yield return null;

            autoScroll.OnPointerEnter(null);
            DeliverToTheLastBlock(waybill);
            for (int frame = 0; frame < FramesToSettle; frame++)
                yield return null;

            Assert.IsFalse(autoScroll.IsFollowing);
            Assert.AreEqual(0f, autoScroll.Offset, PositionTolerance);
        }

        [UnityTest]
        public IEnumerator AutoScroll_DeliveryToABlockAlreadyInView_DoesNotMoveThePage()
        {
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;
            CvAutoScroll autoScroll = Object.FindAnyObjectByType<CvAutoScroll>();
            yield return null;

            waybill.DeliverLetters(waybill.Blocks[0].Block.Id, new[] { 0 }, HighlightSeconds);
            for (int frame = 0; frame < FramesToSettle; frame++)
                yield return null;

            Assert.AreEqual(0f, autoScroll.Offset, PositionTolerance);
        }

        [UnityTest]
        public IEnumerator AutoScroll_WaybillExpanded_IsSwitchedOffAndBackOnWhenCollapsed()
        {
            CvAutoScroll autoScroll = Object.FindAnyObjectByType<CvAutoScroll>();
            Button expandButton = GameObject.Find(ExpandButtonName).GetComponent<Button>();

            expandButton.onClick.Invoke();
            yield return null;
            Assert.IsFalse(autoScroll.enabled);

            expandButton.onClick.Invoke();
            yield return null;
            Assert.IsTrue(autoScroll.enabled);
        }

        [UnityTest]
        public IEnumerator Waybill_Booted_ShowsAnIconOfItsKindBesideEveryContact()
        {
            yield return null;
            CvChipView[] chips = Object.FindObjectsByType<CvChipView>(FindObjectsSortMode.None);
            var shownIcons = new System.Collections.Generic.List<string>();
            foreach (CvChipView chip in chips)
                if (chip.HasIcon)
                    shownIcons.Add(chip.ShownIcon.name);

            CollectionAssert.AreEquivalent(new[] { "ContactPhone", "ContactEmail", "ContactGithub", "ContactItch" }, shownIcons);
        }

        [UnityTest]
        public IEnumerator AutoButton_Clicked_StopsThePageFollowingUntilClickedAgain()
        {
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;
            CvAutoScroll autoScroll = Object.FindAnyObjectByType<CvAutoScroll>();
            Button autoButton = GameObject.Find(AutoButtonName).GetComponent<Button>();
            yield return null;
            Sprite onLook = autoButton.GetComponent<Image>().sprite;

            autoButton.onClick.Invoke();
            DeliverToTheLastBlock(waybill);
            for (int frame = 0; frame < FramesToSettle; frame++)
                yield return null;

            Assert.IsFalse(autoScroll.IsSwitchedOn);
            Assert.IsFalse(autoScroll.IsFollowing);
            Assert.AreEqual(0f, autoScroll.Offset, PositionTolerance);
            Assert.AreNotSame(onLook, autoButton.GetComponent<Image>().sprite);

            autoButton.onClick.Invoke();
            DeliverToTheLastBlock(waybill);
            for (int frame = 0; frame < MaxFramesToWait && autoScroll.Offset < FarDownThePage; frame++)
                yield return null;

            Assert.IsTrue(autoScroll.IsSwitchedOn);
            Assert.GreaterOrEqual(autoScroll.Offset, FarDownThePage);
            Assert.AreSame(onLook, autoButton.GetComponent<Image>().sprite);
        }

        [UnityTest]
        public IEnumerator AutoButton_WaybillExpanded_CannotBeClicked()
        {
            Button autoButton = GameObject.Find(AutoButtonName).GetComponent<Button>();
            yield return null;
            Assert.IsTrue(autoButton.interactable);

            GameObject.Find(ExpandButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;
            yield return null;

            Assert.IsFalse(autoButton.interactable);
        }

        [UnityTest]
        public IEnumerator Waybill_ContactNotDeliveredYet_KeepsItsIconHiddenUntilTheFirstLetterArrives()
        {
            CvRevealState waybill = Object.FindAnyObjectByType<CvScreen>().RevealState;
            yield return null;
            CvChipView contactChip = null;
            foreach (CvChipView chip in Object.FindObjectsByType<CvChipView>(FindObjectsSortMode.None))
                if (chip.HasIcon)
                    contactChip = chip;

            Assert.IsNotNull(contactChip);
            Assert.IsFalse(contactChip.IsIconVisible);

            waybill.DeliverLetters(contactChip.Revealable.Reveal.Block.Id, new[] { 0 }, HighlightSeconds);

            Assert.IsTrue(contactChip.IsIconVisible);
        }

        private static void DeliverToTheLastBlock(CvRevealState waybill)
        {
            CvBlock lastBlock = waybill.Blocks[waybill.Blocks.Count - 1].Block;
            waybill.DeliverLetters(lastBlock.Id, new[] { 0 }, HighlightSeconds);
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

        private static void TapOn(Box box)
        {
            var centre = new Vector2(box.Origin.X + box.Size.Width * 0.5f, box.Origin.Y + box.Size.Height * 0.5f);
            Object.FindAnyObjectByType<WarehouseBotSelector>().SelectAt(centre);
        }

        private Box FirstBox(System.Predicate<Box> isWanted)
        {
            foreach (Box box in demo.Simulation.Layout.Boxes)
                if (isWanted(box))
                    return box;

            Assert.Fail("The warehouse has no such box.");
            return null;
        }

        private static bool IsPartlyEmptiedLightBox(Box box)
        {
            return box.WeightClass == WeightClass.Light && !box.IsEmpty && box.RemainingPieces < box.TotalPieces;
        }

        private int IndexOfFirstBox(System.Predicate<Box> isWanted)
        {
            for (int index = 0; index < demo.Simulation.Layout.Boxes.Count; index++)
                if (isWanted(demo.Simulation.Layout.Boxes[index]))
                    return index;

            return -1;
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
