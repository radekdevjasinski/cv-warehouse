using CvWarehouse.Core.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Menu
{
    public sealed class HowToPlayPanel : MonoBehaviour
    {
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject hiddenWhileOpen;
        [SerializeField] private GameObject[] cards;
        [SerializeField] private Image[] cardDots;
        [SerializeField] private Color currentDotColour = new Color32(0xf2, 0xc9, 0x4c, 0xff);
        [SerializeField] private Color otherDotColour = new Color32(0x4a, 0x44, 0x66, 0xff);
        [SerializeField] private GameObject mouseControls;
        [SerializeField] private GameObject touchControls;

        private CardPager pager;

        public bool IsOpen => panel.activeSelf;

        public int CurrentCard => pager.CurrentCard;

        public void Open()
        {
            pager.ShowFirst();
            ShowCurrentCard();
            panel.SetActive(true);
            hiddenWhileOpen.SetActive(false);
        }

        public void Close()
        {
            panel.SetActive(false);
            hiddenWhileOpen.SetActive(true);
        }

        public void ShowNext()
        {
            pager.ShowNext();
            ShowCurrentCard();
        }

        public void ShowPrevious()
        {
            pager.ShowPrevious();
            ShowCurrentCard();
        }

        private void Awake()
        {
            pager = new CardPager(cards.Length);
            bool isTouchDevice = Application.isMobilePlatform;
            mouseControls.SetActive(!isTouchDevice);
            touchControls.SetActive(isTouchDevice);
            panel.SetActive(false);
        }

        private void OnEnable()
        {
            openButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(Close);
            previousButton.onClick.AddListener(ShowPrevious);
            nextButton.onClick.AddListener(ShowNext);
        }

        private void OnDisable()
        {
            nextButton.onClick.RemoveListener(ShowNext);
            previousButton.onClick.RemoveListener(ShowPrevious);
            closeButton.onClick.RemoveListener(Close);
            openButton.onClick.RemoveListener(Open);
        }

        private void ShowCurrentCard()
        {
            for (int card = 0; card < cards.Length; card++)
                cards[card].SetActive(card == pager.CurrentCard);
            for (int dot = 0; dot < cardDots.Length; dot++)
                cardDots[dot].color = dot == pager.CurrentCard ? currentDotColour : otherDotColour;
            previousButton.interactable = pager.HasPrevious;
            nextButton.interactable = pager.HasNext;
        }
    }
}
