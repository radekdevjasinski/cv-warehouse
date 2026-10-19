using System.Collections.Generic;
using CvWarehouse.Core.Effects;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Effects;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseBotsView : MonoBehaviour
    {
        [SerializeField] private WarehouseSettings settings;
        [SerializeField] private BotSkin skin;
        [SerializeField] private BotAnimation botAnimation;
        [SerializeField] private Sprite selectionFrame;
        [SerializeField] private FeedbackEffectsConfig effects;

        private WarehouseSimulation simulation;
        private BotSelection selection;
        private BotFigure[] figures;
        private SpriteRenderer[] highlights;
        private EffectTimers popIns;
        private EffectTimers flashes;

        public int VisibleBotCount { get; private set; }

        public int VisibleHighlightCount { get; private set; }

        public Vector3 HighlightPosition(int index)
        {
            return highlights[index].transform.localPosition;
        }

        public BotFigure Figure(int index)
        {
            return figures[index];
        }

        public void Show(WarehouseSimulation shownSimulation, BotSelection botSelection, SquareShapeFactory shapes)
        {
            Unsubscribe();
            simulation = shownSimulation;
            selection = botSelection;
            figures = new BotFigure[simulation.MaxBots];
            highlights = new SpriteRenderer[simulation.MaxBots];
            popIns = new EffectTimers(simulation.MaxBots);
            flashes = new EffectTimers(simulation.MaxBots);
            for (int index = 0; index < figures.Length; index++)
            {
                figures[index] = new BotFigure("Bot " + index, transform, skin, shapes);
                CreateHighlight(index, shapes);
            }

            if (isActiveAndEnabled)
                Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void LateUpdate()
        {
            if (simulation == null)
                return;

            AdvanceEffects(Time.deltaTime);
            IReadOnlyList<Bot> bots = simulation.Bots;
            VisibleBotCount = Mathf.Min(bots.Count, figures.Length);
            for (int index = 0; index < figures.Length; index++)
            {
                if (index < VisibleBotCount)
                    ShowFigure(figures[index], bots[index]);
                else
                    figures[index].Hide();
            }

            ShowHighlights();
        }

        private void Subscribe()
        {
            if (simulation == null)
                return;

            simulation.BotAdded += StartPopIn;
            simulation.Upgraded += StartFlash;
        }

        private void Unsubscribe()
        {
            if (simulation == null)
                return;

            simulation.BotAdded -= StartPopIn;
            simulation.Upgraded -= StartFlash;
        }

        private void StartPopIn(Bot bot)
        {
            popIns.Start(bot.Id);
        }

        private void StartFlash(Bot bot, BotUpgrade upgrade)
        {
            flashes.Start(bot.Id);
        }

        private void AdvanceEffects(float deltaSeconds)
        {
            popIns.DurationSeconds = effects.PopInSeconds;
            flashes.DurationSeconds = effects.FlashSeconds;
            popIns.Advance(deltaSeconds);
            flashes.Advance(deltaSeconds);
        }

        private void ShowFigure(BotFigure figure, Bot bot)
        {
            Color color = settings.CrewColor(bot.CrewSlot);
            if (flashes.TryGetProgress(bot.Id, out float flashProgress))
                color = Color.Lerp(color, effects.FlashColor, EffectCurves.Flash(flashProgress));
            figure.Show(bot, PoseOf(bot), color);

            float scale = popIns.TryGetProgress(bot.Id, out float popInProgress)
                ? EffectCurves.PopIn(popInProgress, effects.PopInOvershoot)
                : 1f;
            figure.SetScale(scale);
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target.activeSelf != isActive)
                target.SetActive(isActive);
        }

        private BotPose PoseOf(Bot bot)
        {
            return new BotPose(
                botAnimation.IsStepping(bot.IsMoving, simulation.TickCount),
                botAnimation.IsBlinking(bot.CrewSlot, Time.time));
        }

        private void CreateHighlight(int index, SquareShapeFactory shapes)
        {
            highlights[index] = shapes.CreateSquare("Selection Highlight " + index, transform, DepthSorting.OverEverythingOrder);
            highlights[index].sprite = selectionFrame;
            highlights[index].gameObject.SetActive(false);
        }

        private void ShowHighlights()
        {
            IReadOnlyList<Bot> selectedBots = selection.Bots;
            VisibleHighlightCount = Mathf.Min(selectedBots.Count, highlights.Length);
            for (int index = 0; index < highlights.Length; index++)
            {
                bool isVisible = index < VisibleHighlightCount;
                SetActive(highlights[index].gameObject, isVisible);
                if (isVisible)
                    highlights[index].transform.localPosition = new Vector3(selectedBots[index].CentreX, selectedBots[index].CentreY);
            }
        }
    }
}
