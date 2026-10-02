using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public interface ICvTextSource
    {
        Awaitable<CvTextResponse> LoadAsync();
    }
}
