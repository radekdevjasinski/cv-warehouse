using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class GameMenuView : MonoBehaviour
    {
        private const string MenuActionName = "Player/Menu";

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Button openButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button showCvButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private WarehouseWaybill waybill;
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private WarehouseSimulation simulation;
        private InputAction menuAction;

        public bool IsOpen => panel.activeSelf;

        public void Show(WarehouseSimulation shownSimulation)
        {
            simulation = shownSimulation;
            if (IsOpen)
                simulation.Hold();
        }

        public void Open()
        {
            panel.SetActive(true);
            if (simulation != null)
                simulation.Hold();
        }

        public void Close()
        {
            panel.SetActive(false);
            if (simulation != null)
                simulation.Release();
        }

        private void Awake()
        {
            menuAction = inputActions.FindAction(MenuActionName, true);
            panel.SetActive(false);
        }

        private void OnEnable()
        {
            menuAction.Enable();
            menuAction.performed += ToggleFromKey;
            openButton.onClick.AddListener(Open);
            resumeButton.onClick.AddListener(Close);
            restartButton.onClick.AddListener(Restart);
            showCvButton.onClick.AddListener(ShowCv);
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        private void OnDisable()
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
            showCvButton.onClick.RemoveListener(ShowCv);
            restartButton.onClick.RemoveListener(Restart);
            resumeButton.onClick.RemoveListener(Close);
            openButton.onClick.RemoveListener(Open);
            menuAction.performed -= ToggleFromKey;
            menuAction.Disable();
        }

        private void ToggleFromKey(InputAction.CallbackContext context)
        {
            if (IsOpen)
                Close();
            else
                Open();
        }

        private void Restart()
        {
            SceneManager.LoadSceneAsync(gameObject.scene.name);
        }

        private void ShowCv()
        {
            Close();
            waybill.ShowFullCv();
        }

        private void GoToMainMenu()
        {
            SceneManager.LoadSceneAsync(mainMenuSceneName);
        }
    }
}
