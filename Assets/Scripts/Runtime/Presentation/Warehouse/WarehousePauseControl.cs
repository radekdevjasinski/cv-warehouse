using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehousePauseControl : MonoBehaviour
    {
        private const string PauseActionName = "Player/Pause";
        private const string PauseLabel = "Pause (Space)";
        private const string ResumeLabel = "Resume (Space)";

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Button pauseButton;
        [SerializeField] private TMP_Text pauseLabel;
        [SerializeField] private GameObject pausedBadge;

        private WarehouseSimulation simulation;
        private InputAction pauseAction;

        public void Show(WarehouseSimulation shownSimulation)
        {
            simulation = shownSimulation;
            Refresh();
        }

        public void TogglePause()
        {
            if (simulation == null)
                return;

            simulation.TogglePause();
            Refresh();
        }

        private void Awake()
        {
            pauseAction = inputActions.FindAction(PauseActionName, true);
        }

        private void OnEnable()
        {
            pauseAction.Enable();
            pauseAction.performed += TogglePauseFromKey;
            pauseButton.onClick.AddListener(TogglePause);
        }

        private void OnDisable()
        {
            pauseButton.onClick.RemoveListener(TogglePause);
            pauseAction.performed -= TogglePauseFromKey;
            pauseAction.Disable();
        }

        private void TogglePauseFromKey(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        private void Refresh()
        {
            pauseLabel.SetText(simulation.IsPaused ? ResumeLabel : PauseLabel);
            pausedBadge.SetActive(simulation.IsPaused);
        }
    }
}
