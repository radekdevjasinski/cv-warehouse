using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Cv;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseWaybill : MonoBehaviour
    {
        [SerializeField] private WarehouseDemo demo;
        [SerializeField] private CvScreen screen;
        [SerializeField] private string cvFileName = "cv_en.json";

        private CvDeliveryReveal reveal;

        private async void Start()
        {
            await screen.ShowAsync(new HttpCvTextSource(cvFileName));
            if (this == null || screen.RevealState == null)
                return;

            reveal = new CvDeliveryReveal(screen.RevealState, new CvPiecePicker(new System.Random(demo.Seed)));
        }

        private void LateUpdate()
        {
            if (reveal == null)
                return;

            WarehouseSimulation simulation = demo.Simulation;
            reveal.ShowProgress(simulation.Score.DeliveredPieces, simulation.Layout.TotalPieces);
        }
    }
}
