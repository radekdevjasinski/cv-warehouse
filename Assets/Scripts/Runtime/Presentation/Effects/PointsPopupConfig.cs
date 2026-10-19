using CvWarehouse.Core.Effects;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Effects
{
    [CreateAssetMenu(menuName = "CV Warehouse/Points Popup Config", fileName = "PointsPopupConfig")]
    public sealed class PointsPopupConfig : ScriptableObject
    {
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] private Material outlinedMaterial;
        [SerializeField] private Color color = new Color(0.91f, 0.77f, 0.28f, 1f);
        [SerializeField, Min(0.1f)] private float fontSize = 4f;
        [SerializeField] private float startHeight = 0.6f;
        [SerializeField, Min(1)] private int capacity = 12;
        [SerializeField, Min(0.1f)] private float durationSeconds = 1.2f;
        [SerializeField, Min(0f)] private float riseDistance = 1f;
        [SerializeField, Range(0f, 1f)] private float fadeStartFraction = 0.5f;
        [SerializeField, Min(0f)] private float overlapWidth = 1.5f;
        [SerializeField, Min(0f)] private float lineHeight = 0.6f;

        public TMP_FontAsset Font => font;

        public Material OutlinedMaterial => outlinedMaterial;

        public Color Color => color;

        public float FontSize => fontSize;

        public float StartHeight => startHeight;

        public int Capacity => capacity;

        public void CopyTo(PointsPopupSettings settings)
        {
            settings.DurationSeconds = durationSeconds;
            settings.RiseDistance = riseDistance;
            settings.FadeStartFraction = fadeStartFraction;
            settings.OverlapWidth = overlapWidth;
            settings.LineHeight = lineHeight;
        }
    }
}
