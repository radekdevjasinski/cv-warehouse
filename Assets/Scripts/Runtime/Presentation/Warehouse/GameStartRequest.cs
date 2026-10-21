using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Game Start Request", fileName = "GameStartRequest")]
    public sealed class GameStartRequest : ScriptableObject
    {
        private bool isSkipRequested;

        public void RequestSkip()
        {
            isSkipRequested = true;
        }

        public bool TakeSkipRequest()
        {
            bool wasRequested = isSkipRequested;
            isSkipRequested = false;
            return wasRequested;
        }
    }
}
