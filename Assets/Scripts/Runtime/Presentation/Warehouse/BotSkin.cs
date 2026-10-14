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

        [Header("Extraction progress bar")]
        [SerializeField] private Color progressFrameColor = new Color32(0x1a, 0x1c, 0x2c, 0xff);
        [SerializeField] private Color progressFillColor = new Color32(0xff, 0xcd, 0x75, 0xff);

        public Color ProgressFrameColor => progressFrameColor;

        public Color ProgressFillColor => progressFillColor;

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
