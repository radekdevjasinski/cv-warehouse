using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class GameMenuView : MonoBehaviour
    {
        private const string MenuActionName = "Player/Menu";
        private const string ShowCvLabel = "Show CV";
        private const string HideCvLabel = "Hide CV";

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Button openButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button showCvButton;
        [SerializeField] private TMP_Text showCvButtonLabel;
        [SerializeField] private Button skipGameButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private GameObject menuWindow;
        [SerializeField] private GameObject skipConfirmWindow;
        [SerializeField] private Button confirmSkipButton;
        [SerializeField] private Button cancelSkipButton;
        [SerializeField] private WarehouseWaybill waybill;
        [SerializeField] private WarehouseScreenSplit screenSplit;
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private WarehouseSimulation simulation;
        private InputAction menuAction;

        public bool IsOpen => panel.activeSelf;

        public bool IsAskingToSkip => skipConfirmWindow.activeSelf;

        public void Show(WarehouseSimulation shownSimulation)
        {
            simulation = shownSimulation;
            if (IsOpen)
                simulation.Hold();
        }

        public void Open()
        {
            showCvButtonLabel.SetText(screenSplit.IsWaybillExpanded ? HideCvLabel : ShowCvLabel);
            ShowMenuWindow();
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
            showCvButton.onClick.AddListener(ToggleCv);
            skipGameButton.onClick.AddListener(AskToSkip);
            confirmSkipButton.onClick.AddListener(SkipGame);
            cancelSkipButton.onClick.AddListener(ShowMenuWindow);
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        private void OnDisable()
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
            cancelSkipButton.onClick.RemoveListener(ShowMenuWindow);
            confirmSkipButton.onClick.RemoveListener(SkipGame);
            skipGameButton.onClick.RemoveListener(AskToSkip);
            showCvButton.onClick.RemoveListener(ToggleCv);
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

        private void ToggleCv()
        {
            Close();
            if (screenSplit.IsWaybillExpanded)
                screenSplit.CollapseWaybill();
            else
                screenSplit.ExpandWaybill();
        }

        private void AskToSkip()
        {
            menuWindow.SetActive(false);
            skipConfirmWindow.SetActive(true);
        }

        private void ShowMenuWindow()
        {
            skipConfirmWindow.SetActive(false);
            menuWindow.SetActive(true);
        }

        private void SkipGame()
        {
            Close();
            waybill.SkipToFullCv();
        }

        private void GoToMainMenu()
        {
            SceneManager.LoadSceneAsync(mainMenuSceneName);
        }
    }
}
