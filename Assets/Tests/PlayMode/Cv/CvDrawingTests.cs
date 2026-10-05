using System.Collections;
using System.Collections.Generic;
using CvWarehouse.Presentation.Cv;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace CvWarehouse.Tests.PlayMode.Cv
{
    public sealed class CvDrawingTests
    {
        private const string ScreenPrefabPath = "Assets/Prefabs/Cv/CvScreen.prefab";
        private const string SampleCvFolder = "Assets/Tests/PlayMode/Cv/SampleCvs/";
        private const string PanelPath = "CvCanvas/Panel";
        private const string ValueTextName = "Body";
        private const string KeyValueCv =
            "{\"name\":\"Ada\",\"sections\":[{\"title\":\"Skills\",\"style\":\"keyValue\",\"entries\":["
            + "{\"title\":\"C\",\"description\":\"Low level\"},"
            + "{\"title\":\"Game development\",\"description\":\"Unity\"}]}]}";
        private const float SidePanelLeftEdge = 0.62f;
        private const float FullScreenLeftEdge = 0f;
        private const float TolerancePixels = 1f;
        private const int LayoutFrames = 3;

        private static readonly string[] SampleCvs =
        {
            "nurse", "civil_engineer", "lawyer", "graphic_designer", "researcher", "chef", "minimal", "stress"
        };

        private static readonly float[] PanelLeftEdges = { SidePanelLeftEdge, FullScreenLeftEdge };

        private readonly List<TMP_Text> texts = new List<TMP_Text>();
        private readonly List<Rect> drawnAreas = new List<Rect>();
        private CvScreen screen;

        [SetUp]
        public void SetUp()
        {
            screen = Object.Instantiate(AssetDatabase.LoadAssetAtPath<CvScreen>(ScreenPrefabPath));
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(screen.gameObject);
        }

        [UnityTest]
        public IEnumerator Show_SampleCv_DrawsNoTextOverAnotherOrOutsideThePage(
            [ValueSource(nameof(SampleCvs))] string cvName,
            [ValueSource(nameof(PanelLeftEdges))] float panelLeftEdge)
        {
            ((RectTransform)screen.transform.Find(PanelPath)).anchorMin = new Vector2(panelLeftEdge, 0f);
            yield return ShowRevealed(AssetDatabase.LoadAssetAtPath<TextAsset>(SampleCvFolder + cvName + ".json").text);
            RectTransform page = screen.GetComponentInChildren<ScrollRect>().content;
            CollectDrawnAreas(page);

            Assert.Greater(drawnAreas.Count, 0, "The CV drew no text.");
            AssertEveryTextIsInsideThePage(page);
            AssertNoTextCoversAnother(page.lossyScale.x * TolerancePixels);
        }

        [UnityTest]
        public IEnumerator Show_KeyValueSection_StartsEveryValueInTheSameColumn()
        {
            yield return ShowRevealed(KeyValueCv);
            var corners = new Vector3[4];
            var valueLeftEdges = new List<float>();
            foreach (TMP_Text text in screen.GetComponentInChildren<ScrollRect>().content.GetComponentsInChildren<TMP_Text>(false))
            {
                if (text.name != ValueTextName)
                    continue;

                text.rectTransform.GetWorldCorners(corners);
                valueLeftEdges.Add(corners[0].x);
            }

            Assert.AreEqual(2, valueLeftEdges.Count);
            Assert.AreEqual(valueLeftEdges[0], valueLeftEdges[1], TolerancePixels);
        }

        private IEnumerator ShowRevealed(string cvJson)
        {
            Awaitable showing = screen.ShowAsync(new FakeCvTextSource(CvTextResponse.Success(cvJson)));
            yield return null;
            Assert.IsTrue(showing.IsCompleted);
            Assert.IsNotNull(screen.RevealState, "The CV was rejected.");

            screen.RevealState.RevealAll();
            for (int frame = 0; frame < LayoutFrames; frame++)
                yield return null;
            Canvas.ForceUpdateCanvases();
        }

        private void CollectDrawnAreas(RectTransform page)
        {
            texts.Clear();
            drawnAreas.Clear();
            foreach (TMP_Text text in page.GetComponentsInChildren<TMP_Text>(false))
            {
                Bounds bounds = text.textBounds;
                if (text.textInfo.characterCount == 0 || bounds.size.x <= 0f || bounds.size.y <= 0f)
                    continue;

                Vector2 lowerLeft = text.transform.TransformPoint(bounds.min);
                Vector2 upperRight = text.transform.TransformPoint(bounds.max);
                texts.Add(text);
                drawnAreas.Add(Rect.MinMaxRect(lowerLeft.x, lowerLeft.y, upperRight.x, upperRight.y));
            }
        }

        private void AssertEveryTextIsInsideThePage(RectTransform page)
        {
            var corners = new Vector3[4];
            page.GetWorldCorners(corners);
            float tolerance = page.lossyScale.x * TolerancePixels;
            for (int index = 0; index < texts.Count; index++)
            {
                Assert.GreaterOrEqual(drawnAreas[index].xMin, corners[0].x - tolerance, Describe(index) + " sticks out on the left.");
                Assert.LessOrEqual(drawnAreas[index].xMax, corners[2].x + tolerance, Describe(index) + " sticks out on the right.");
            }
        }

        private void AssertNoTextCoversAnother(float tolerance)
        {
            for (int first = 0; first < texts.Count; first++)
                for (int second = first + 1; second < texts.Count; second++)
                    Assert.IsFalse(
                        Shrink(drawnAreas[first], tolerance).Overlaps(Shrink(drawnAreas[second], tolerance)),
                        Describe(first) + " is drawn over " + Describe(second) + ".");
        }

        private static Rect Shrink(Rect area, float amount)
        {
            return Rect.MinMaxRect(area.xMin + amount, area.yMin + amount, area.xMax - amount, area.yMax - amount);
        }

        private string Describe(int index)
        {
            return texts[index].name + " '" + texts[index].GetParsedText() + "' " + drawnAreas[index];
        }
    }
}
