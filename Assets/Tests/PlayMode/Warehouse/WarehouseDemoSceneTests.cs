using System.Collections;
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
        private const string UpgradeCarryButtonName = "UpgradeCarryButton";
        private const string PointsTextName = "PointsText";
        private const string RemoveBotButtonName = "RemoveBotButton";
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
            Assert.IsFalse(GameObject.Find(UpgradeCarryButtonName).GetComponent<Button>().interactable);
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
        public IEnumerator RemoveBotButton_Clicked_RemovesABot()
        {
            GameObject.Find(RemoveBotButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(StartingBots - 1, Object.FindAnyObjectByType<WarehouseBotsView>().VisibleBotCount);
        }

        [UnityTest]
        public IEnumerator UpgradeCarryButton_ClickedWithEnoughPoints_MakesBotsCarryWords()
        {
            Button upgradeCarryButton = GameObject.Find(UpgradeCarryButtonName).GetComponent<Button>();
            TickUntil(() => demo.Shop.CanUpgradeCarry);
            yield return null;
            Assert.IsTrue(upgradeCarryButton.interactable);

            upgradeCarryButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(CarrySize.Word, demo.Simulation.CarrySize);
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
