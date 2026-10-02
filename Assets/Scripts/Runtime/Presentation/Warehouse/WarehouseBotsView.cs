using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseBotsView : MonoBehaviour
    {
        private const int BotSortingOrder = 2;
        private const int CargoSortingOrder = 3;
        private const float CellCentre = 0.5f;

        [SerializeField] private Color botColor = new Color(0.35f, 0.75f, 0.95f);
        [SerializeField] private Color cargoColor = Color.white;
        [SerializeField] private float botScale = 0.7f;
        [SerializeField] private float cargoScale = 0.45f;

        private IReadOnlyList<Bot> bots;
        private GameObject[] botObjects;
        private GameObject[] cargoObjects;

        public int VisibleBotCount { get; private set; }

        public void Show(WarehouseSimulation simulation, SquareShapeFactory shapes)
        {
            bots = simulation.Bots;
            botObjects = new GameObject[simulation.MaxBots];
            cargoObjects = new GameObject[simulation.MaxBots];
            for (int index = 0; index < botObjects.Length; index++)
                CreateBot(index, shapes);
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
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target.activeSelf != isActive)
                target.SetActive(isActive);
        }

        private static Vector3 CentreOf(GridPosition cell)
        {
            return new Vector3(cell.X + CellCentre, cell.Y + CellCentre);
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
            botObjects[index].transform.localPosition = Vector3.Lerp(CentreOf(bot.Cell), CentreOf(bot.NextCell), bot.MoveFraction);
            SetActive(cargoObjects[index], bot.CargoPieces > 0);
        }
    }
}
