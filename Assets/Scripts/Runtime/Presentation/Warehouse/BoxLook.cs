using System;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [Serializable]
    public sealed class BoxLook
    {
        [SerializeField] private Sprite closed;
        [SerializeField] private Sprite open;
        [SerializeField] private Sprite empty;

        public Sprite SpriteOf(Box box)
        {
            if (box.IsEmpty)
                return empty;

            bool isStarted = box.RemainingPieces < box.TotalPieces;
            return isStarted && open != null ? open : closed;
        }
    }
}
