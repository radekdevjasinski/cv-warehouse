using NUnit.Framework;
using UnityEditor;

namespace CvWarehouse.Tests.EditMode.Build
{
    public sealed class BuildScenesTests
    {
        private const string GameScenePath = "Assets/Scenes/WarehouseDemo.unity";

        [Test]
        public void BuildSettings_Scenes_ContainOnlyTheGameScene()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            Assert.AreEqual(1, scenes.Length);
            Assert.AreEqual(GameScenePath, scenes[0].path);
            Assert.IsTrue(scenes[0].enabled);
        }
    }
}
