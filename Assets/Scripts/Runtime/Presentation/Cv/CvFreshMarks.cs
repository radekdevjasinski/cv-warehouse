using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvFreshMarks
    {
        private const int NoRun = -1;
        private const float SidePadding = 2f;

        private readonly TMP_Text text;
        private readonly CvHighlightPool pool;
        private readonly List<Image> shown = new List<Image>();

        public CvFreshMarks(TMP_Text text, CvHighlightPool pool)
        {
            this.text = text;
            this.pool = pool;
        }

        public void Show(CvBlockReveal reveal, int firstLetter)
        {
            Clear();
            if (!reveal.HasFreshLetters)
                return;

            TMP_TextInfo textInfo = text.textInfo;
            int runStart = NoRun;
            int runEnd = NoRun;
            int letter = firstLetter;
            for (int characterIndex = 0; characterIndex < textInfo.characterCount; characterIndex++)
            {
                if (!textInfo.characterInfo[characterIndex].isVisible)
                    continue;

                bool continuesRun = reveal.IsFresh(letter) && IsOnTheLineOf(textInfo, runStart, characterIndex);
                if (!continuesRun)
                {
                    Mark(textInfo, runStart, runEnd);
                    runStart = reveal.IsFresh(letter) ? characterIndex : NoRun;
                }

                runEnd = characterIndex;
                letter++;
            }

            Mark(textInfo, runStart, runEnd);
            FadeAway(reveal);
        }

        private static bool IsOnTheLineOf(TMP_TextInfo textInfo, int runStart, int characterIndex)
        {
            return runStart != NoRun
                && textInfo.characterInfo[runStart].lineNumber == textInfo.characterInfo[characterIndex].lineNumber;
        }

        private void Clear()
        {
            for (int index = 0; index < shown.Count; index++)
                pool.Return(shown[index]);
            shown.Clear();
        }

        private void FadeAway(CvBlockReveal reveal)
        {
            for (int index = 0; index < shown.Count; index++)
            {
                shown[index].canvasRenderer.SetAlpha(reveal.FreshFraction);
                shown[index].CrossFadeAlpha(0f, reveal.FreshSecondsLeft, true);
            }
        }

        private void Mark(TMP_TextInfo textInfo, int runStart, int runEnd)
        {
            if (runStart == NoRun)
                return;

            TMP_LineInfo line = textInfo.lineInfo[textInfo.characterInfo[runStart].lineNumber];
            float left = textInfo.characterInfo[runStart].bottomLeft.x - SidePadding;
            float right = textInfo.characterInfo[runEnd].topRight.x + SidePadding;
            Image mark = pool.Rent(text.transform);
            RectTransform area = mark.rectTransform;
            area.sizeDelta = new Vector2(right - left, line.ascender - line.descender);
            area.localPosition = new Vector3((left + right) * 0.5f, (line.ascender + line.descender) * 0.5f);
            shown.Add(mark);
        }
    }
}
