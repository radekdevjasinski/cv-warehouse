using System;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [Serializable]
    public sealed class BotSkin
    {
        [SerializeField] private Sprite bodyStand;
        [SerializeField] private Sprite bodyStep;
        [SerializeField] private Sprite faceStand;
        [SerializeField] private Sprite faceStep;
        [SerializeField] private Sprite faceStandBlink;
        [SerializeField] private Sprite faceStepBlink;

        [Header("Cargo, from one sheet of paper up")]
        [SerializeField] private Sprite[] cargoStacks;

        public Sprite Body(BotPose pose)
        {
            return pose.IsStepping ? bodyStep : bodyStand;
        }

        public Sprite Face(BotPose pose)
        {
            if (pose.IsBlinking)
                return pose.IsStepping ? faceStepBlink : faceStandBlink;

            return pose.IsStepping ? faceStep : faceStand;
        }

        public Sprite Cargo(int words)
        {
            return cargoStacks[Mathf.Clamp(words, 1, cargoStacks.Length) - 1];
        }
    }
}
