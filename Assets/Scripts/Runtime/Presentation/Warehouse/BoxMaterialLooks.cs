using System;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [Serializable]
    public sealed class BoxMaterialLooks
    {
        [SerializeField] private BoxLook small;
        [SerializeField] private BoxLook medium;
        [SerializeField] private BoxLook large;

        public BoxLook LookOf(BoxSizeClass sizeClass)
        {
            switch (sizeClass)
            {
                case BoxSizeClass.Large:
                    return large;
                case BoxSizeClass.Medium:
                    return medium;
                default:
                    return small;
            }
        }
    }
}
