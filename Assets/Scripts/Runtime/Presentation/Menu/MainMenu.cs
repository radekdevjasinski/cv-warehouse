using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Menu;
using CvWarehouse.Presentation.Cv;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Menu
{
    public sealed class MainMenu : MonoBehaviour
    {
        private const string VersionSuffix = " · Unity Web";

        [SerializeField] private Button startShiftButton;
        [SerializeField] private TMP_Text subtitle;
        [SerializeField] private TMP_Text versionLabel;
        [SerializeField] private Color nameColour = new Color32(0xf4, 0xec, 0xd8, 0xff);
        [SerializeField] private string gameSceneName = "WarehouseDemo";
        [SerializeField] private string cvFileName = "cv_en.json";

        private void Awake()
        {
            subtitle.SetText(MenuSubtitle.WithoutName);
            versionLabel.SetText("v" + Application.version + VersionSuffix);
        }

        private async void Start()
        {
            CvTextResponse response = await new HttpCvTextSource(cvFileName).LoadAsync();
            if (this == null || !response.IsSuccess)
                return;

            CvParseResult result = new CvParser().Parse(response.Text);
            if (result.IsValid)
                subtitle.SetText(MenuSubtitle.For(result.Document.Header.Name, "#" + ColorUtility.ToHtmlStringRGB(nameColour)));
        }

        private void OnEnable()
        {
            startShiftButton.onClick.AddListener(StartShift);
        }

        private void OnDisable()
        {
            startShiftButton.onClick.RemoveListener(StartShift);
        }

        private void StartShift()
        {
            startShiftButton.interactable = false;
            SceneManager.LoadSceneAsync(gameSceneName);
        }
    }
}
