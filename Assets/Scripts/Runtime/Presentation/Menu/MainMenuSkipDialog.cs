using CvWarehouse.Presentation.Warehouse;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Menu
{
    public sealed class MainMenuSkipDialog : MonoBehaviour
    {
        [SerializeField] private Button viewCvButton;
        [SerializeField] private GameObject dialog;
        [SerializeField] private GameObject hiddenWhileOpen;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private GameStartRequest startRequest;
        [SerializeField] private string gameSceneName = "WarehouseDemo";

        public bool IsOpen => dialog.activeSelf;

        public void Open()
        {
            hiddenWhileOpen.SetActive(false);
            dialog.SetActive(true);
        }

        public void Close()
        {
            dialog.SetActive(false);
            hiddenWhileOpen.SetActive(true);
        }

        private void Awake()
        {
            dialog.SetActive(false);
        }

        private void OnEnable()
        {
            viewCvButton.onClick.AddListener(Open);
            confirmButton.onClick.AddListener(SkipToCv);
            cancelButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            cancelButton.onClick.RemoveListener(Close);
            confirmButton.onClick.RemoveListener(SkipToCv);
            viewCvButton.onClick.RemoveListener(Open);
        }

        private void SkipToCv()
        {
            confirmButton.interactable = false;
            startRequest.RequestSkip();
            SceneManager.LoadSceneAsync(gameSceneName);
        }
    }
}
