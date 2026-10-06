using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseBotsView : MonoBehaviour
    {
        private const int BotSortingOrder = 2;
        private const int CargoSortingOrder = 3;
        private const int HighlightSortingOrder = 1;

        [SerializeField] private Color botColor = new Color(0.35f, 0.75f, 0.95f);
        [SerializeField] private Color cargoColor = Color.white;
        [SerializeField] private Color highlightColor = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private float botScale = 0.7f;
        [SerializeField] private float cargoScale = 0.45f;
        [SerializeField] private float highlightScale = 0.95f;

        private IReadOnlyList<Bot> bots;
        private BotSelection selection;
        private GameObject[] botObjects;
        private GameObject[] cargoObjects;
        private GameObject[] highlights;

        public int VisibleBotCount { get; private set; }

        public int VisibleHighlightCount { get; private set; }

        public Vector3 HighlightPosition(int index)
        {
            return highlights[index].transform.localPosition;
        }

        public void Show(WarehouseSimulation simulation, BotSelection botSelection, SquareShapeFactory shapes)
        {
            bots = simulation.Bots;
            selection = botSelection;
            botObjects = new GameObject[simulation.MaxBots];
            cargoObjects = new GameObject[simulation.MaxBots];
            highlights = new GameObject[simulation.MaxBots];
            for (int index = 0; index < botObjects.Length; index++)
            {
                CreateBot(index, shapes);
                CreateHighlight(index, shapes);
            }
        }

        private void LateUpdate()
        {
            if (bots == null)
                return;

            VisibleBotCount = Mathf.Min(bots.Count, botObjects.Length);
            for (int index = 0; index < botObjects.Length; index++)
            {
                bool isVisible = index < VisibleBotCount;
                SetActive(botObjects[index], isVisible);
                if (isVisible)
                    ShowBot(index, bots[index]);
            }

            ShowHighlights();
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target.activeSelf != isActive)
                target.SetActive(isActive);
        }

        private static Vector3 CentreOf(Bot bot)
        {
            return new Vector3(bot.CentreX, bot.CentreY);
        }

        private void CreateHighlight(int index, SquareShapeFactory shapes)
        {
            SpriteRenderer highlightRenderer = shapes.CreateSquare("Selection Highlight " + index, transform, HighlightSortingOrder);
            highlightRenderer.color = highlightColor;
            highlightRenderer.transform.localScale = new Vector3(highlightScale, highlightScale, 1f);
            highlights[index] = highlightRenderer.gameObject;
            highlights[index].SetActive(false);
        }

        private void ShowHighlights()
        {
            IReadOnlyList<Bot> selectedBots = selection.Bots;
            VisibleHighlightCount = Mathf.Min(selectedBots.Count, highlights.Length);
            for (int index = 0; index < highlights.Length; index++)
            {
                bool isVisible = index < VisibleHighlightCount;
                SetActive(highlights[index], isVisible);
                if (isVisible)
                    highlights[index].transform.localPosition = CentreOf(selectedBots[index]);
            }
        }

        private void CreateBot(int index, SquareShapeFactory shapes)
        {
            SpriteRenderer botRenderer = shapes.CreateSquare("Bot " + index, transform, BotSortingOrder);
            botRenderer.color = botColor;
            botRenderer.transform.localScale = new Vector3(botScale, botScale, 1f);

            SpriteRenderer cargoRenderer = shapes.CreateSquare("Cargo", botRenderer.transform, CargoSortingOrder);
            cargoRenderer.color = cargoColor;
            cargoRenderer.transform.localScale = new Vector3(cargoScale, cargoScale, 1f);

            botObjects[index] = botRenderer.gameObject;
            cargoObjects[index] = cargoRenderer.gameObject;
            botObjects[index].SetActive(false);
            cargoObjects[index].SetActive(false);
        }

        private void ShowBot(int index, Bot bot)
        {
            botObjects[index].transform.localPosition = CentreOf(bot);
            SetActive(cargoObjects[index], bot.CargoPieces > 0);
        }
    }
}
