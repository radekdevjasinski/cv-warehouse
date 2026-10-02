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
        private int firstLetter;

        public void Bind(CvBlockReveal blockReveal, int firstLetterInBlock)
        {
            reveal = blockReveal;
            firstLetter = firstLetterInBlock;
        }

        public void Refresh()
        {
            if (!isActiveAndEnabled)
                return;

            ApplyVisibility(text.textInfo);
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        private void OnEnable()
        {
            text.OnPreRenderText += ApplyVisibility;
        }

        private void OnDisable()
        {
            text.OnPreRenderText -= ApplyVisibility;
        }

        private void ApplyVisibility(TMP_TextInfo textInfo)
        {
            if (reveal == null)
                return;

            int letter = firstLetter;
            for (int characterIndex = 0; characterIndex < textInfo.characterCount; characterIndex++)
            {
                ref TMP_CharacterInfo character = ref textInfo.characterInfo[characterIndex];
                if (!character.isVisible)
                    continue;

                SetAlpha(textInfo, ref character, reveal.IsRevealed(letter) ? VisibleAlpha : HiddenAlpha);
                letter++;
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
