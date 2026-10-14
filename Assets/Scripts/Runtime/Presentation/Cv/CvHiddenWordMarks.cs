using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvHiddenWordMarks
    {
        private const int NoRun = -1;
        private const float LineInsetShare = 0.18f;
        private const float WordGapInset = 1.5f;

        private readonly TMP_Text text;
        private readonly CvHighlightPool pool;
        private readonly List<Image> shown = new List<Image>();

        public CvHiddenWordMarks(TMP_Text text, CvHighlightPool pool)
        {
            this.text = text;
            this.pool = pool;
        }

        public int Count => shown.Count;

        public void Show(CvBlockReveal reveal, int firstGlyph)
        {
            Clear();
            TMP_TextInfo textInfo = text.textInfo;
            int runStart = NoRun;
            int runEnd = NoRun;
            int glyph = firstGlyph;
            for (int characterIndex = 0; characterIndex < textInfo.characterCount; characterIndex++)
            {
                if (!textInfo.characterInfo[characterIndex].isVisible)
                    continue;

                bool isHidden = !reveal.IsRevealed(reveal.Block.LetterAt(glyph));
                bool continuesRun = isHidden && runStart != NoRun && ContinuesWord(textInfo, runEnd, characterIndex);
                if (!continuesRun)
                {
                    Mark(textInfo, runStart, runEnd);
                    runStart = isHidden ? characterIndex : NoRun;
                }

                runEnd = characterIndex;
                glyph++;
            }

            Mark(textInfo, runStart, runEnd);
        }

        private static bool ContinuesWord(TMP_TextInfo textInfo, int previousIndex, int characterIndex)
        {
            return previousIndex == characterIndex - 1
                && textInfo.characterInfo[previousIndex].lineNumber == textInfo.characterInfo[characterIndex].lineNumber;
        }

        private void Clear()
        {
            for (int index = 0; index < shown.Count; index++)
                pool.Return(shown[index]);
            shown.Clear();
        }

        private void Mark(TMP_TextInfo textInfo, int runStart, int runEnd)
        {
            if (runStart == NoRun)
                return;

            TMP_LineInfo line = textInfo.lineInfo[textInfo.characterInfo[runStart].lineNumber];
            float inset = (line.ascender - line.descender) * LineInsetShare;
            float left = textInfo.characterInfo[runStart].bottomLeft.x + WordGapInset;
            float right = Mathf.Max(left + WordGapInset, textInfo.characterInfo[runEnd].topRight.x - WordGapInset);
            float top = line.ascender - inset;
            float bottom = line.descender + inset;
            Image mark = pool.Rent(text.transform);
            RectTransform area = mark.rectTransform;
            area.sizeDelta = new Vector2(right - left, top - bottom);
            area.localPosition = new Vector3((left + right) * 0.5f, (top + bottom) * 0.5f);
            shown.Add(mark);
        }
    }
}
