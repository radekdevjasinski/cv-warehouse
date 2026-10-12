using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    [RequireComponent(typeof(Button))]
    public sealed class CvAutoScrollToggle : MonoBehaviour
    {
        [SerializeField] private CvAutoScroll autoScroll;
        [SerializeField] private Image background;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Color followingTint = Color.white;
        [SerializeField] private Color waitingTint = new Color(0.72f, 0.72f, 0.72f, 1f);

        private Button button;
        private bool isShownAvailable;
        private bool isShownOn;
        private bool isShownFollowing;
        private bool hasShownAnything;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(autoScroll.Toggle);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(autoScroll.Toggle);
        }

        private void LateUpdate()
        {
            bool isAvailable = autoScroll.isActiveAndEnabled;
            bool isOn = isAvailable && autoScroll.IsSwitchedOn;
            bool isFollowing = isOn && autoScroll.IsFollowing;
            if (hasShownAnything && isAvailable == isShownAvailable && isOn == isShownOn && isFollowing == isShownFollowing)
                return;

            hasShownAnything = true;
            isShownAvailable = isAvailable;
            isShownOn = isOn;
            isShownFollowing = isFollowing;
            button.interactable = isAvailable;
            background.sprite = isOn ? onSprite : offSprite;
            background.color = isFollowing || !isOn ? followingTint : waitingTint;
        }
    }
}
