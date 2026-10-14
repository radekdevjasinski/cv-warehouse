using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Box Skin", fileName = "BoxSkin")]
    public sealed class BoxSkin : ScriptableObject
    {
        [Header("Material, by importance")]
        [SerializeField] private BoxMaterialLooks cardboard;
        [SerializeField] private BoxMaterialLooks crate;
        [SerializeField] private BoxMaterialLooks steel;

        [Header("Selection frame, by size")]
        [SerializeField] private Sprite smallFrame;
        [SerializeField] private Sprite mediumFrame;
        [SerializeField] private Sprite largeFrame;

        public BoxLook LookOf(Box box)
        {
            return MaterialOf(box.Importance).LookOf(box.SizeClass);
        }

        public Sprite FrameOf(BoxSizeClass sizeClass)
        {
            switch (sizeClass)
            {
                case BoxSizeClass.Large:
                    return largeFrame;
                case BoxSizeClass.Medium:
                    return mediumFrame;
                default:
                    return smallFrame;
            }
        }

        private BoxMaterialLooks MaterialOf(Importance importance)
        {
            switch (importance)
            {
                case Importance.Key:
                    return steel;
                case Importance.Important:
                    return crate;
                default:
                    return cardboard;
            }
        }
    }
}
