using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvBootstrap : MonoBehaviour
    {
        [SerializeField] private CvScreen screen;
        [SerializeField] private string cvFileName = "cv_en.json";

        private async void Start()
        {
            await screen.ShowAsync(new HttpCvTextSource(cvFileName));
        }
    }
}
