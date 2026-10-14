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
    public sealed class WarehouseCameraSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const int MaxFramesToWait = 300;
        private const int SpritePixelsPerUnit = 16;
        private const float Tolerance = 0.001f;

        private WarehouseDemo demo;
        private Camera warehouseCamera;

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
            warehouseCamera = Object.FindAnyObjectByType<WarehouseCamera>().GetComponent<Camera>();
        }

        [UnityTest]
        public IEnumerator Scene_Booted_DrawsEverySpritePixelAsAWholeNumberOfScreenPixels()
        {
            yield return null;

            float screenPixelsPerSpritePixel = warehouseCamera.pixelHeight / (warehouseCamera.orthographicSize * 2f * SpritePixelsPerUnit);
            Assert.GreaterOrEqual(screenPixelsPerSpritePixel, 1f - Tolerance);
            Assert.AreEqual(Mathf.Round(screenPixelsPerSpritePixel), screenPixelsPerSpritePixel, Tolerance);
        }

        [UnityTest]
        public IEnumerator Scene_Booted_KeepsTheViewInsideTheWarehouse()
        {
            yield return null;

            WarehouseGrid grid = demo.Simulation.Layout.Grid;
            Vector3 bottomLeft = warehouseCamera.ViewportToWorldPoint(Vector3.zero);
            Vector3 topRight = warehouseCamera.ViewportToWorldPoint(Vector3.one);
            Assert.GreaterOrEqual(bottomLeft.x, -Tolerance);
            Assert.GreaterOrEqual(bottomLeft.y, -Tolerance);
            Assert.LessOrEqual(topRight.x, grid.Width + Tolerance);
            Assert.LessOrEqual(topRight.y, grid.Height + Tolerance);
        }
    }
}
