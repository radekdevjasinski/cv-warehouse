using System.Collections;
using CvWarehouse.Presentation.Cv;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace CvWarehouse.Tests.PlayMode.Cv
{
    public sealed class CvSceneBootTests
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const int MaxFramesToWait = 300;
        private const int SampleCvBlockCount = 28;

        [UnityTest]
        public IEnumerator Scene_Booted_ShowsSampleCvWithEveryBlockHidden()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return null;
            CvScreen screen = Object.FindAnyObjectByType<CvScreen>();

            for (int frame = 0; frame < MaxFramesToWait && screen.RevealState == null; frame++)
                yield return null;

            Assert.IsNotNull(screen.RevealState, "The CV was not loaded in time.");
            Assert.AreEqual(SampleCvBlockCount, screen.RevealState.Blocks.Count);
            Assert.AreEqual(SampleCvBlockCount, screen.GetComponentsInChildren<CvRevealable>(true).Length);
            Assert.AreEqual(0, screen.RevealState.RevealedLetterCount);
        }
    }
}
