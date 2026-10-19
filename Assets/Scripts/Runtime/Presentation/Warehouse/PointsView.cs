using CvWarehouse.Core.Effects;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Effects;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class PointsView : MonoBehaviour
    {
        private const int NothingShown = -1;

        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private FeedbackEffectsConfig effects;

        private readonly EffectTimer pulse = new EffectTimer();

        private BotShop shop;
        private int shownPoints = NothingShown;

        public void Show(BotShop shownShop)
        {
            shop = shownShop;
            Refresh();
        }

        public float PulseScale => pointsText.rectTransform.localScale.x;

        private void LateUpdate()
        {
            if (shop == null)
                return;

            Refresh();
            ShowPulse(Time.deltaTime);
        }

        private void Refresh()
        {
            if (shownPoints == shop.Balance)
                return;

            if (shownPoints != NothingShown && shop.Balance > shownPoints)
                pulse.Start();
            shownPoints = shop.Balance;
            pointsText.SetText("{0}", shownPoints);
        }

        private void ShowPulse(float deltaSeconds)
        {
            if (!pulse.IsRunning)
                return;

            pulse.DurationSeconds = effects.PulseSeconds;
            pulse.Advance(deltaSeconds);
            float scale = pulse.IsRunning ? EffectCurves.Pulse(pulse.Progress, effects.PulseScale) : 1f;
            pointsText.rectTransform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
