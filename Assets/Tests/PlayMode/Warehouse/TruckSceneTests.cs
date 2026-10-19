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

namespace CvWarehouse.Tests.PlayMode.Warehouse
{
    public sealed class TruckSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const int MaxFramesToWait = 300;
        private const float PositionTolerance = 0.001f;
        private const float TruckHeight = 38f / 16f;

        private WarehouseDemo demo;
        private TruckView truckView;

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
            truckView = Object.FindAnyObjectByType<TruckView>();
            Assert.IsNotNull(truckView, "The warehouse scene has no truck view.");
        }

        [Test]
        public void Truck_Booted_DrawsOnePartPerTruckColumnStandingOnTheFloorLine()
        {
            WarehouseLayout layout = demo.Simulation.Layout;

            Assert.AreEqual(layout.TruckSize.Width, truckView.Parts.Count);
            for (int column = 0; column < truckView.Parts.Count; column++)
            {
                SpriteRenderer part = truckView.Parts[column];
                Assert.IsNotNull(part.sprite, "Truck column " + column + " has no sprite.");
                Assert.AreEqual(layout.TruckOrigin.X + column + 0.5f, part.transform.position.x, PositionTolerance);
                Assert.AreEqual(layout.TruckOrigin.Y, part.bounds.min.y + 2f / DepthSorting.PixelsPerUnit, PositionTolerance);
            }
        }

        [Test]
        public void Truck_Booted_ShowsTheCabAtTheFrontTheRearAtTheBackAndAWheelForEveryPlannedCentre()
        {
            WarehouseLayout layout = demo.Simulation.Layout;
            var plan = new TruckPlan(layout.TruckSize.Width);

            Assert.AreEqual("TruckCabFront", truckView.Parts[0].sprite.name);
            Assert.AreEqual("TruckTrailerRear", truckView.Parts[truckView.Parts.Count - 1].sprite.name);
            Assert.AreEqual(plan.WheelCentres.Count, truckView.Wheels.Count);
            for (int wheel = 0; wheel < plan.WheelCentres.Count; wheel++)
            {
                Assert.AreEqual(layout.TruckOrigin.X + plan.WheelCentres[wheel], truckView.Wheels[wheel].transform.position.x, PositionTolerance);
                Assert.AreEqual(layout.TruckOrigin.Y, truckView.Wheels[wheel].bounds.min.y, PositionTolerance);
            }
        }

        [Test]
        public void TruckLogo_Booted_SitsOnTheBoxInFrontOfTheBody()
        {
            WarehouseLayout layout = demo.Simulation.Layout;
            var plan = new TruckPlan(layout.TruckSize.Width);

            Assert.IsNotNull(truckView.Logo, "The truck has no logo.");
            Assert.AreEqual("TruckLogo", truckView.Logo.sprite.name);
            Assert.AreEqual(layout.TruckOrigin.X + plan.LogoCentre, truckView.Logo.transform.position.x, PositionTolerance);
            Assert.Greater(truckView.Logo.sortingOrder, truckView.Parts[0].sortingOrder);
        }

        [Test]
        public void LoadingDoor_Booted_IsWiderAndTallerThanTheTruckAndFitsInTheWall()
        {
            WarehouseLayout layout = demo.Simulation.Layout;
            Bounds door = truckView.Door.bounds;

            Assert.Greater(door.size.x, layout.TruckSize.Width);
            Assert.Greater(door.size.y, TruckHeight);
            Assert.LessOrEqual(door.size.y, layout.TruckSize.Height);
            Assert.AreEqual(layout.TruckOrigin.Y, door.min.y, PositionTolerance);
            Assert.AreEqual(layout.TruckOrigin.X + layout.TruckSize.Width * 0.5f, door.center.x, PositionTolerance);
        }

        [UnityTest]
        public IEnumerator Truck_Booted_IsDrawnInFrontOfTheDoorAndBehindTheBots()
        {
            WarehouseBotsView botsView = Object.FindAnyObjectByType<WarehouseBotsView>();
            yield return null;

            foreach (SpriteRenderer part in truckView.Parts)
            {
                Assert.Greater(part.sortingOrder, truckView.Door.sortingOrder);
                Assert.Less(part.sortingOrder, botsView.Figure(0).SortingOrder);
            }
        }
    }
}
