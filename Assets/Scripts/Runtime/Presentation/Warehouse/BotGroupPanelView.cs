using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotGroupPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject window;
        [SerializeField] private Button deleteButton;

        private WarehouseSimulation simulation;
        private BotSelection selection;

        public void Show(WarehouseSimulation shownSimulation, BotSelection botSelection)
        {
            simulation = shownSimulation;
            selection = botSelection;
            Refresh();
        }

        private void OnEnable()
        {
            deleteButton.onClick.AddListener(DeleteBots);
        }

        private void OnDisable()
        {
            deleteButton.onClick.RemoveListener(DeleteBots);
        }

        private void LateUpdate()
        {
            if (selection != null)
                Refresh();
        }

        private void DeleteBots()
        {
            IReadOnlyList<Bot> selectedBots = selection.Bots;
            for (int index = 0; index < selectedBots.Count; index++)
                simulation.TryRemoveBot(selectedBots[index]);
            selection.Clear();
        }

        private void Refresh()
        {
            if (window.activeSelf != selection.HasManyBots)
                window.SetActive(selection.HasManyBots);
            if (deleteButton.interactable != simulation.CanRemoveBot)
                deleteButton.interactable = simulation.CanRemoveBot;
        }
    }
}
