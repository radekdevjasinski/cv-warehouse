using System.Collections;
using CvWarehouse.Core.Cv;
using CvWarehouse.Presentation.Cv;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace CvWarehouse.Tests.PlayMode.Cv
{
    public sealed class CvScreenTests
    {
        private const string ScreenPrefabPath = "Assets/Prefabs/Cv/CvScreen.prefab";
        private const string NamePath = "CvCanvas/ScrollView/Viewport/Page/Header/Name";
        private const string ButtonsPath = "DebugCanvas/Buttons/";
        private const int BlockCount = 7;
        private const int NameLetterCount = 11;
        private const string ValidCv =
            "{\"name\":\"Ada Lovelace\",\"title\":\"Programmer\","
            + "\"contacts\":[{\"kind\":\"email\",\"text\":\"ada@example.com\",\"link\":\"mailto:ada@example.com\"}],"
            + "\"sections\":["
            + "{\"title\":\"Profile\",\"type\":\"paragraph\",\"text\":\"Hello\"},"
            + "{\"title\":\"Projects\",\"type\":\"list\",\"style\":\"bullets\",\"entries\":[{\"title\":\"One\"},{\"title\":\"Two\"}]},"
            + "{\"title\":\"Languages\",\"type\":\"list\",\"style\":\"inline\",\"entries\":[{\"title\":\"English\"}]}]}";

        private CvScreen screen;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<CvScreen>(ScreenPrefabPath);
            screen = Object.Instantiate(prefab);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(screen.gameObject);
        }

        [UnityTest]
        public IEnumerator ShowAsync_ValidCv_BuildsOneHiddenViewPerBlock()
        {
            yield return Show(CvTextResponse.Success(ValidCv));

            Assert.AreEqual(BlockCount, screen.RevealState.Blocks.Count);
            Assert.AreEqual(BlockCount, screen.GetComponentsInChildren<CvRevealable>(true).Length);
            Assert.AreEqual(0, screen.RevealState.RevealedLetterCount);
            Assert.AreEqual(3, screen.GetComponentsInChildren<CvSectionView>(true).Length);
            Assert.IsFalse(screen.GetComponentInChildren<CvErrorView>().IsVisible);
        }

        [UnityTest]
        public IEnumerator RevealLetter_OneLetterOfTheName_DrawsOnlyThatLetter()
        {
            yield return Show(CvTextResponse.Success(ValidCv));
            TMP_Text nameText = screen.transform.Find(NamePath).GetComponent<TMP_Text>();
            Graphic placeholder = screen.transform.Find(NamePath + "/Placeholder").GetComponent<Graphic>();
            Assert.IsTrue(placeholder.enabled);

            screen.RevealState.RevealLetter(CvItemId.Name, 1);
            yield return null;

            Assert.AreEqual("Ada Lovelace", nameText.text);
            Assert.AreEqual(0, ReadLetterAlpha(nameText, 0));
            Assert.AreEqual(255, ReadLetterAlpha(nameText, 1));
            Assert.IsFalse(placeholder.enabled);
        }

        [UnityTest]
        public IEnumerator RevealLetterButton_Clicked_RevealsOneLetterOfTheFirstBlock()
        {
            yield return Show(CvTextResponse.Success(ValidCv));

            Click("RevealLetterButton");

            Assert.AreEqual(1, screen.RevealState.RevealedLetterCount);
            Assert.AreEqual(1, GetBlock(CvItemId.Name).RevealedCount);
        }

        [UnityTest]
        public IEnumerator RevealWordButton_Clicked_RevealsOneWholeWordOfTheFirstBlock()
        {
            yield return Show(CvTextResponse.Success(ValidCv));

            Click("RevealWordButton");

            CvBlockReveal nameBlock = GetBlock(CvItemId.Name);
            Assert.IsTrue(nameBlock.IsWordComplete(0) != nameBlock.IsWordComplete(1));
        }

        [UnityTest]
        public IEnumerator RevealBlockButton_Clicked_CompletesTheFirstIncompleteBlock()
        {
            yield return Show(CvTextResponse.Success(ValidCv));

            Click("RevealBlockButton");
            Click("RevealBlockButton");

            Assert.IsTrue(GetBlock(CvItemId.Name).IsComplete);
            Assert.IsTrue(GetBlock(CvItemId.JobTitle).IsComplete);
            Assert.AreEqual(NameLetterCount + "Programmer".Length, screen.RevealState.RevealedLetterCount);
        }

        [UnityTest]
        public IEnumerator RevealAllButton_Clicked_CompletesTheCv()
        {
            yield return Show(CvTextResponse.Success(ValidCv));

            Click("RevealAllButton");

            Assert.IsTrue(screen.RevealState.IsComplete);
        }

        [UnityTest]
        public IEnumerator ResetButton_ClickedAfterReveal_HidesEverythingAgain()
        {
            yield return Show(CvTextResponse.Success(ValidCv));
            Graphic placeholder = screen.transform.Find(NamePath + "/Placeholder").GetComponent<Graphic>();
            Click("RevealAllButton");

            Click("ResetButton");

            Assert.AreEqual(0, screen.RevealState.RevealedLetterCount);
            Assert.IsTrue(placeholder.enabled);
        }

        [UnityTest]
        public IEnumerator ShowAsync_InvalidCv_ShowsReadableError()
        {
            yield return Show(CvTextResponse.Success("{ broken"));

            CvErrorView errorView = screen.GetComponentInChildren<CvErrorView>();
            Assert.IsTrue(errorView.IsVisible);
            StringAssert.StartsWith("The CV file is not valid JSON.", errorView.Message);
            Assert.IsNull(screen.RevealState);
        }

        [UnityTest]
        public IEnumerator ShowAsync_SourceFails_ShowsItsError()
        {
            yield return Show(CvTextResponse.Failure("Could not load the CV file."));

            CvErrorView errorView = screen.GetComponentInChildren<CvErrorView>();
            Assert.IsTrue(errorView.IsVisible);
            Assert.AreEqual("Could not load the CV file.", errorView.Message);
        }

        private IEnumerator Show(CvTextResponse response)
        {
            Awaitable showing = screen.ShowAsync(new FakeCvTextSource(response));
            yield return null;
            Assert.IsTrue(showing.IsCompleted);
        }

        private void Click(string buttonName)
        {
            screen.transform.Find(ButtonsPath + buttonName).GetComponent<Button>().onClick.Invoke();
        }

        private CvBlockReveal GetBlock(string id)
        {
            Assert.IsTrue(screen.RevealState.TryGetBlock(id, out CvBlockReveal block));
            return block;
        }

        private static int ReadLetterAlpha(TMP_Text text, int characterIndex)
        {
            TMP_CharacterInfo character = text.textInfo.characterInfo[characterIndex];
            return text.textInfo.meshInfo[character.materialReferenceIndex].colors32[character.vertexIndex].a;
        }
    }
}
