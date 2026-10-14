using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class CvLetterMask : MonoBehaviour
    {
        private const int VerticesPerLetter = 4;
        private const byte VisibleAlpha = 255;
        private const byte HiddenAlpha = 0;

        [SerializeField] private TMP_Text text;

        private CvBlockReveal reveal;
        private CvFreshMarks freshMarks;
        private CvHiddenWordMarks hiddenWordMarks;
        private int firstGlyph;
        private bool areHiddenWordsStale;

        public int HiddenWordMarkCount => hiddenWordMarks == null ? 0 : hiddenWordMarks.Count;

        public void Bind(CvBlockReveal blockReveal, int firstGlyphInBlock)
        {
            reveal = blockReveal;
            firstGlyph = firstGlyphInBlock;
        }

        public void MarkFreshLettersWith(CvHighlightPool pool)
        {
            freshMarks = new CvFreshMarks(text, pool);
        }

        public void MarkHiddenWordsWith(CvHighlightPool pool)
        {
            hiddenWordMarks = new CvHiddenWordMarks(text, pool);
            areHiddenWordsStale = true;
        }

        public void Refresh()
        {
            if (!isActiveAndEnabled)
                return;

            ApplyVisibility(text.textInfo);
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            freshMarks?.Show(reveal, firstGlyph);
            ShowHiddenWords();
        }

        private void OnEnable()
        {
            text.OnPreRenderText += ApplyVisibility;
        }

        private void OnDisable()
        {
            text.OnPreRenderText -= ApplyVisibility;
        }

        private void LateUpdate()
        {
            if (areHiddenWordsStale)
                ShowHiddenWords();
        }

        private void ShowHiddenWords()
        {
            if (hiddenWordMarks == null || reveal == null)
                return;

            areHiddenWordsStale = false;
            hiddenWordMarks.Show(reveal, firstGlyph);
        }

        private void ApplyVisibility(TMP_TextInfo textInfo)
        {
            if (reveal == null)
                return;

            areHiddenWordsStale = true;
            int glyph = firstGlyph;
            for (int characterIndex = 0; characterIndex < textInfo.characterCount; characterIndex++)
            {
                ref TMP_CharacterInfo character = ref textInfo.characterInfo[characterIndex];
                if (!character.isVisible)
                    continue;

                bool isShown = reveal.IsRevealed(reveal.Block.LetterAt(glyph));
                SetAlpha(textInfo, ref character, isShown ? VisibleAlpha : HiddenAlpha);
                glyph++;
            }
        }

        private static void SetAlpha(TMP_TextInfo textInfo, ref TMP_CharacterInfo character, byte alpha)
        {
            Color32[] colors = textInfo.meshInfo[character.materialReferenceIndex].colors32;
            int firstVertex = character.vertexIndex;
            if (colors == null || firstVertex + VerticesPerLetter > colors.Length)
                return;

            for (int vertex = firstVertex; vertex < firstVertex + VerticesPerLetter; vertex++)
                colors[vertex].a = alpha;
        }
    }
}
