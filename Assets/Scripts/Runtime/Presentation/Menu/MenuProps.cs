using CvWarehouse.Presentation.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Menu
{
    public sealed class MenuProps : MonoBehaviour
    {
        private const int PropLayer = 0;

        [SerializeField] private SpriteRenderer[] standingProps;
        [SerializeField] private SpriteRenderer[] flatProps;

        private void Awake()
        {
            foreach (SpriteRenderer prop in standingProps)
                prop.sortingOrder = DepthSorting.OrderAt(prop.bounds.min.y, PropLayer);
            foreach (SpriteRenderer prop in flatProps)
                prop.sortingOrder = DepthSorting.OnTheFloorOrder;
        }
    }
}
