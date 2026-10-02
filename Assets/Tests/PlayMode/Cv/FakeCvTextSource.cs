using CvWarehouse.Presentation.Cv;
using UnityEngine;

namespace CvWarehouse.Tests.PlayMode.Cv
{
    public sealed class FakeCvTextSource : ICvTextSource
    {
        private readonly CvTextResponse response;

        public FakeCvTextSource(CvTextResponse response)
        {
            this.response = response;
        }

        public Awaitable<CvTextResponse> LoadAsync()
        {
            var completion = new AwaitableCompletionSource<CvTextResponse>();
            completion.SetResult(response);
            return completion.Awaitable;
        }
    }
}
