using System.Collections.Generic;
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

        private WarehouseScore score;

        private async void Start()
        {
            await screen.ShowAsync(new HttpCvTextSource(cvFileName));
            if (this == null || screen.RevealState == null)
                return;

            demo.Begin(BlocksOf(screen.RevealState));
            score = demo.Simulation.Score;
            if (isActiveAndEnabled)
                score.Delivered += ShowDelivery;
        }

        private void OnEnable()
        {
            if (score != null)
                score.Delivered += ShowDelivery;
        }

        private void OnDisable()
        {
            if (score != null)
                score.Delivered -= ShowDelivery;
        }

        private static List<CvBlock> BlocksOf(CvRevealState revealState)
        {
            var blocks = new List<CvBlock>(revealState.Blocks.Count);
            foreach (CvBlockReveal block in revealState.Blocks)
                blocks.Add(block.Block);
            return blocks;
        }

        private void ShowDelivery(CvBlock block, IReadOnlyList<int> letters)
        {
            screen.RevealState.RevealLetters(block.Id, letters);
        }
    }
}
