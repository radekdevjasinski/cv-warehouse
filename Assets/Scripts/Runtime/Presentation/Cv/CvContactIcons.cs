using System;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    [CreateAssetMenu(menuName = "CV Warehouse/Contact Icons", fileName = "CvContactIcons")]
    public sealed class CvContactIcons : ScriptableObject
    {
        [SerializeField] private CvContactIcon[] icons = Array.Empty<CvContactIcon>();
        [SerializeField] private Sprite iconOfAnyOtherKind;

        public Sprite IconOf(string kind)
        {
            for (int index = 0; index < icons.Length; index++)
                if (string.Equals(icons[index].Kind, kind, StringComparison.OrdinalIgnoreCase))
                    return icons[index].Sprite;

            return iconOfAnyOtherKind;
        }
    }
}
