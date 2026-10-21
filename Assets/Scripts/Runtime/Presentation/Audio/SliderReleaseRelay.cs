using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Audio
{
    [RequireComponent(typeof(Slider))]
    public sealed class SliderReleaseRelay : MonoBehaviour, IPointerUpHandler
    {
        public event Action Released;

        public void OnPointerUp(PointerEventData eventData)
        {
            Released?.Invoke();
        }
    }
}
