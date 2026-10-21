using System;
using CvWarehouse.Core.Menu;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Menu
{
    public sealed class CardPagerTests
    {
        private const int CardCount = 3;

        [Test]
        public void Constructor_NewPager_StartsOnTheFirstCard()
        {
            CardPager pager = new CardPager(CardCount);

            Assert.AreEqual(0, pager.CurrentCard);
            Assert.IsFalse(pager.HasPrevious);
            Assert.IsTrue(pager.HasNext);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_NoCards_Throws(int cardCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CardPager(cardCount));
        }

        [Test]
        public void ShowNext_OnTheLastCard_StaysOnTheLastCard()
        {
            CardPager pager = new CardPager(CardCount);

            for (int step = 0; step < CardCount + 2; step++)
                pager.ShowNext();

            Assert.AreEqual(CardCount - 1, pager.CurrentCard);
            Assert.IsFalse(pager.HasNext);
            Assert.IsTrue(pager.HasPrevious);
        }

        [Test]
        public void ShowPrevious_OnTheFirstCard_StaysOnTheFirstCard()
        {
            CardPager pager = new CardPager(CardCount);

            pager.ShowPrevious();

            Assert.AreEqual(0, pager.CurrentCard);
        }

        [Test]
        public void ShowPrevious_AfterShowNext_GoesBackOneCard()
        {
            CardPager pager = new CardPager(CardCount);
            pager.ShowNext();
            pager.ShowNext();

            pager.ShowPrevious();

            Assert.AreEqual(1, pager.CurrentCard);
        }

        [Test]
        public void ShowFirst_FromTheLastCard_GoesBackToTheStart()
        {
            CardPager pager = new CardPager(CardCount);
            pager.ShowNext();
            pager.ShowNext();

            pager.ShowFirst();

            Assert.AreEqual(0, pager.CurrentCard);
        }

        [Test]
        public void HasPreviousAndHasNext_SingleCard_AreBothFalse()
        {
            CardPager pager = new CardPager(1);

            Assert.IsFalse(pager.HasPrevious);
            Assert.IsFalse(pager.HasNext);
        }
    }
}
