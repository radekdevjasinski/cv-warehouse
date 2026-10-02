using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class HttpCvTextSource : ICvTextSource
    {
        private readonly string fileName;

        public HttpCvTextSource(string fileName)
        {
            this.fileName = fileName;
        }

        public async Awaitable<CvTextResponse> LoadAsync()
        {
            using UnityWebRequest request = UnityWebRequest.Get(BuildUrl());
            await request.SendWebRequest();

            return request.result == UnityWebRequest.Result.Success
                ? CvTextResponse.Success(request.downloadHandler.text)
                : CvTextResponse.Failure("Could not load the CV file '" + fileName + "'. " + request.error);
        }

        private string BuildUrl()
        {
            string location = Application.streamingAssetsPath + "/" + fileName;
            if (location.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return location + "?v=" + DateTime.UtcNow.Ticks;

            return new Uri(location).AbsoluteUri;
        }
    }
}
