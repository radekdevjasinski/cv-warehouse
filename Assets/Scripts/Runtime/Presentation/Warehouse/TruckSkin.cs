using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Truck Skin", fileName = "TruckSkin")]
    public sealed class TruckSkin : ScriptableObject
    {
        [SerializeField] private Sprite cabFront;
        [SerializeField] private Sprite cabBack;
        [SerializeField] private Sprite trailerMiddle;
        [SerializeField] private Sprite trailerRear;
        [SerializeField] private Sprite wheel;
        [SerializeField] private Sprite logo;
        [SerializeField] private Sprite loadingDoor;
        [SerializeField, Min(0)] private int doorExtraColumns = 2;

        public Sprite Wheel => wheel;

        public Sprite Logo => logo;

        public Sprite LoadingDoor => loadingDoor;

        public int DoorExtraColumns => doorExtraColumns;

        public Sprite SpriteOf(TruckPart part)
        {
            switch (part)
            {
                case TruckPart.CabFront:
                    return cabFront;
                case TruckPart.CabBack:
                    return cabBack;
                case TruckPart.TrailerRear:
                    return trailerRear;
                default:
                    return trailerMiddle;
            }
        }
    }
}
