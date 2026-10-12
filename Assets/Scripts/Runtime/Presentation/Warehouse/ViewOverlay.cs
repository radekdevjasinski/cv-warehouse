using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public static class ViewOverlay
    {
        public static void Cover(RawImage image, Texture texture)
        {
            image.texture = texture;
            image.raycastTarget = false;
            image.enabled = true;
            image.transform.SetAsFirstSibling();
        }
    }
}
