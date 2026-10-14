using System;
using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class CrewListView : MonoBehaviour
    {
        private const string AddToSelectionActionName = "Player/AddToSelection";

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private CrewChipView chipPrefab;
        [SerializeField] private RectTransform chipsRoot;
        [SerializeField] private WarehouseSettings settings;

        private CrewChipView[] chips = Array.Empty<CrewChipView>();
        private IReadOnlyList<Bot> bots;
        private BotSelection selection;
        private BoxSelection boxSelection;
        private InputAction addToSelectionAction;

        public int VisibleChipCount { get; private set; }

        public CrewChipView Chip(int index)
        {
            return chips[index];
        }

        public void Show(WarehouseSimulation simulation, BotSelection botSelection, BoxSelection inspectedBox)
        {
            bots = simulation.Bots;
            selection = botSelection;
            boxSelection = inspectedBox;
            chips = new CrewChipView[simulation.MaxBots];
            for (int index = 0; index < chips.Length; index++)
            {
                chips[index] = Instantiate(chipPrefab, chipsRoot);
                chips[index].gameObject.SetActive(false);
                if (isActiveAndEnabled)
                    chips[index].Picked += Pick;
            }

            Refresh();
        }

        private void Awake()
        {
            addToSelectionAction = inputActions.FindAction(AddToSelectionActionName, true);
        }

        private void OnEnable()
        {
            addToSelectionAction.Enable();
            foreach (CrewChipView chip in chips)
                chip.Picked += Pick;
        }

        private void OnDisable()
        {
            foreach (CrewChipView chip in chips)
                chip.Picked -= Pick;
            addToSelectionAction.Disable();
        }

        private void LateUpdate()
        {
            if (bots != null)
                Refresh();
        }

        private void Pick(Bot bot)
        {
            boxSelection.Clear();
            if (addToSelectionAction.IsPressed())
                selection.Toggle(bot);
            else if (selection.SingleBot == bot)
                selection.Clear();
            else
                selection.Select(bot);
        }

        private void Refresh()
        {
            VisibleChipCount = Mathf.Min(bots.Count, chips.Length);
            for (int index = 0; index < chips.Length; index++)
            {
                bool isVisible = index < VisibleChipCount;
                GameObject chipObject = chips[index].gameObject;
                if (chipObject.activeSelf != isVisible)
                    chipObject.SetActive(isVisible);
                if (isVisible)
                    chips[index].Show(bots[index], settings.CrewColor(bots[index].CrewSlot), selection.Contains(bots[index]));
            }
        }
    }
}
