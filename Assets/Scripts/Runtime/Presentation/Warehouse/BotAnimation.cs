using System;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [Serializable]
    public sealed class BotAnimation
    {
        [SerializeField] private int ticksPerStepFrame = 8;
        [SerializeField] private float secondsBetweenBlinks = 3.2f;
        [SerializeField] private float blinkSeconds = 0.12f;
        [SerializeField] private float blinkDelayPerCrewSlot = 0.7f;

        public bool IsStepping(bool isMoving, int tickCount)
        {
            return isMoving && tickCount / ticksPerStepFrame % 2 == 1;
        }

        public bool IsBlinking(int crewSlot, float seconds)
        {
            return Mathf.Repeat(seconds + crewSlot * blinkDelayPerCrewSlot, secondsBetweenBlinks) < blinkSeconds;
        }
    }
}
