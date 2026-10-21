using System;

namespace CvWarehouse.Core.Menu
{
    public sealed class CardPager
    {
        public CardPager(int cardCount)
        {
            if (cardCount < 1)
                throw new ArgumentOutOfRangeException(nameof(cardCount), "A pager needs at least one card.");

            CardCount = cardCount;
        }

        public int CardCount { get; }

        public int CurrentCard { get; private set; }

        public bool HasPrevious => CurrentCard > 0;

        public bool HasNext => CurrentCard < CardCount - 1;

        public void ShowFirst()
        {
            CurrentCard = 0;
        }

        public void ShowNext()
        {
            if (HasNext)
                CurrentCard++;
        }

        public void ShowPrevious()
        {
            if (HasPrevious)
                CurrentCard--;
        }
    }
}
