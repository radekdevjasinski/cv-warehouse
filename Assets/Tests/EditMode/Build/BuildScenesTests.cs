using NUnit.Framework;
using UnityEditor;

namespace CvWarehouse.Tests.EditMode.Build
{
    public sealed class BuildScenesTests
    {
        private const string MenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string GameScenePath = "Assets/Scenes/WarehouseDemo.unity";

        [Test]
        public void BuildSettings_Scenes_StartWithTheMainMenuFollowedByTheGame()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            Assert.AreEqual(2, scenes.Length);
            Assert.AreEqual(MenuScenePath, scenes[0].path);
            Assert.AreEqual(GameScenePath, scenes[1].path);
            Assert.IsTrue(scenes[0].enabled);
            Assert.IsTrue(scenes[1].enabled);
        }
    }
}
