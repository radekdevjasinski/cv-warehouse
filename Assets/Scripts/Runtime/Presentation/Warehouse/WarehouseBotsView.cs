using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseBotsView : MonoBehaviour
    {
        [SerializeField] private WarehouseSettings settings;
        [SerializeField] private BotSkin skin;
        [SerializeField] private BotAnimation botAnimation;
        [SerializeField] private Sprite selectionFrame;

        private WarehouseSimulation simulation;
        private BotSelection selection;
        private BotFigure[] figures;
        private SpriteRenderer[] highlights;

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
            simulation = shownSimulation;
            selection = botSelection;
            figures = new BotFigure[simulation.MaxBots];
            highlights = new SpriteRenderer[simulation.MaxBots];
            for (int index = 0; index < figures.Length; index++)
            {
                figures[index] = new BotFigure("Bot " + index, transform, skin, shapes);
                CreateHighlight(index, shapes);
            }
        }

        private void LateUpdate()
        {
            if (simulation == null)
                return;

            IReadOnlyList<Bot> bots = simulation.Bots;
            VisibleBotCount = Mathf.Min(bots.Count, figures.Length);
            for (int index = 0; index < figures.Length; index++)
            {
                if (index < VisibleBotCount)
                    figures[index].Show(bots[index], PoseOf(bots[index]), settings.CrewColor(bots[index].CrewSlot));
                else
                    figures[index].Hide();
            }

            ShowHighlights();
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
