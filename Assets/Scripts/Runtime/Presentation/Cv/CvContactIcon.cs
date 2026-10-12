using System;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    [Serializable]
    public sealed class CvContactIcon
    {
        [SerializeField] private string kind;
        [SerializeField] private Sprite sprite;

        public string Kind => kind;

        public Sprite Sprite => sprite;
    }
}
