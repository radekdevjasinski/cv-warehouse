using System;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [Serializable]
    public struct CrewMember
    {
        [SerializeField] private string name;
        [SerializeField] private Color color;

        public CrewMember(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }

        public string Name => name;

        public Color Color => color;
    }
}
