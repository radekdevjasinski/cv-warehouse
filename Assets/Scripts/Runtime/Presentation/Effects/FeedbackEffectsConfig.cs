using UnityEngine;

namespace CvWarehouse.Presentation.Effects
{
    [CreateAssetMenu(menuName = "CV Warehouse/Feedback Effects Config", fileName = "FeedbackEffectsConfig")]
    public sealed class FeedbackEffectsConfig : ScriptableObject
    {
        [Header("New bot pop-in")]
        [SerializeField, Min(0.01f)] private float popInSeconds = 0.35f;
        [SerializeField, Min(0f)] private float popInOvershoot = 1.7f;

        [Header("Upgrade flash")]
        [SerializeField, Min(0.01f)] private float flashSeconds = 0.4f;
        [SerializeField] private Color flashColor = Color.white;

        [Header("Points counter pulse")]
        [SerializeField, Min(0.01f)] private float pulseSeconds = 0.25f;
        [SerializeField, Min(1f)] private float pulseScale = 1.25f;

        public float PopInSeconds => popInSeconds;

        public float PopInOvershoot => popInOvershoot;

        public float FlashSeconds => flashSeconds;

        public Color FlashColor => flashColor;

        public float PulseSeconds => pulseSeconds;

        public float PulseScale => pulseScale;
    }
}
