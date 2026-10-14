using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Cv;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseWaybill : MonoBehaviour
    {
        private const string WarehouseNotBuiltMessage = "The warehouse could not be built for this CV file.";

        [SerializeField] private WarehouseDemo demo;
        [SerializeField] private CvScreen screen;
        [SerializeField] private CvProgressView progressView;
        [SerializeField] private string cvFileName = "cv_en.json";
        [SerializeField] private float highlightSeconds = 2f;

        private WarehouseScore score;

        private async void Start()
        {
            await screen.ShowAsync(new HttpCvTextSource(cvFileName));
            if (this == null || screen.RevealState == null)
                return;

            if (!demo.TryBegin(BlocksOf(screen.RevealState)))
            {
                screen.ShowError(WarehouseNotBuiltMessage);
                return;
            }

            progressView.Show(screen.RevealState);
            score = demo.Simulation.Score;
            if (isActiveAndEnabled)
                score.Delivered += ShowDelivery;
        }

        private void Update()
        {
            if (score != null)
                screen.RevealState.Advance(Time.deltaTime);
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
            screen.RevealState.DeliverLetters(block.Id, letters, highlightSeconds);
        }
    }
}
