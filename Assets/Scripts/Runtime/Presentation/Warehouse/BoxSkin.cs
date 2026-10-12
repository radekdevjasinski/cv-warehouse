using CvWarehouse.Core.Cv;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Box Skin", fileName = "BoxSkin")]
    public sealed class BoxSkin : ScriptableObject
    {
        [SerializeField] private BoxLook light;
        [SerializeField] private BoxLook medium;
        [SerializeField] private BoxLook heavy;

        public BoxLook LookOf(WeightClass weightClass)
        {
            switch (weightClass)
            {
                case WeightClass.Heavy:
                    return heavy;
                case WeightClass.Medium:
                    return medium;
                default:
                    return light;
            }
        }
    }
}
