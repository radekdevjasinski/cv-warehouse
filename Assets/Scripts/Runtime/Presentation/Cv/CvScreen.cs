using CvWarehouse.Core.Cv;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvScreen : MonoBehaviour
    {
        [SerializeField] private CvDocumentView documentView;
        [SerializeField] private CvErrorView errorView;
        [SerializeField] private CvDebugRevealPanel debugRevealPanel;

        private readonly CvParser parser = new CvParser();

        public CvRevealState RevealState { get; private set; }

        public async Awaitable ShowAsync(ICvTextSource source)
        {
            CvTextResponse response = await source.LoadAsync();
            if (this == null)
                return;

            if (!response.IsSuccess)
            {
                errorView.Show(response.Error);
                return;
            }

            ShowParsed(parser.Parse(response.Text));
        }

        private void ShowParsed(CvParseResult result)
        {
            if (!result.IsValid)
            {
                errorView.Show(result.Error);
                return;
            }

            foreach (string warning in result.Warnings)
                Debug.LogWarning("CV file: " + warning);

            RevealState = new CvRevealState(new CvBlockBuilder().Build(result.Document));
            documentView.Show(result.Document, RevealState);
            debugRevealPanel.Bind(RevealState);
        }
    }
}
