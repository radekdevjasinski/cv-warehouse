using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class TapDetector
    {
        private Vector2 pressPoint;
        private float furthestTravel;

        public void Press(Vector2 point)
        {
            pressPoint = point;
            furthestTravel = 0f;
        }

        public void Move(Vector2 point)
        {
            furthestTravel = Mathf.Max(furthestTravel, Vector2.Distance(pressPoint, point));
        }

        public bool IsTap(float maxTravel)
        {
            return furthestTravel <= maxTravel;
        }
    }
}
