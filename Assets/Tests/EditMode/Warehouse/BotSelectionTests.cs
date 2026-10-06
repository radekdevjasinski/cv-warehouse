using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BotSelectionTests
    {
        private static readonly Bot FirstBot = new Bot(0, new GridPosition(0, 0));
        private static readonly Bot SecondBot = new Bot(1, new GridPosition(1, 0));

        private BotSelection selection;

        [SetUp]
        public void SetUp()
        {
            selection = new BotSelection();
        }

        [Test]
        public void Count_NothingSelected_IsZeroAndThereIsNoSingleBot()
        {
            Assert.AreEqual(0, selection.Count);
            Assert.IsFalse(selection.HasSingleBot);
            Assert.IsFalse(selection.HasManyBots);
            Assert.IsNull(selection.SingleBot);
        }

        [Test]
        public void Select_OneBot_MakesItTheSingleBot()
        {
            selection.Select(FirstBot);

            Assert.IsTrue(selection.HasSingleBot);
            Assert.IsFalse(selection.HasManyBots);
            Assert.AreSame(FirstBot, selection.SingleBot);
        }

        [Test]
        public void Select_AnotherBot_ReplacesTheSelection()
        {
            selection.Select(FirstBot);

            selection.Select(SecondBot);

            Assert.AreEqual(1, selection.Count);
            Assert.AreSame(SecondBot, selection.SingleBot);
        }

        [Test]
        public void SelectAll_TwoBots_HasManyBotsAndNoSingleBot()
        {
            selection.SelectAll(new List<Bot> { FirstBot, SecondBot });

            Assert.AreEqual(2, selection.Count);
            Assert.IsTrue(selection.HasManyBots);
            Assert.IsFalse(selection.HasSingleBot);
            Assert.IsNull(selection.SingleBot);
        }

        [Test]
        public void SelectAll_OneBot_MakesItTheSingleBot()
        {
            selection.SelectAll(new List<Bot> { SecondBot });

            Assert.AreSame(SecondBot, selection.SingleBot);
        }

        [Test]
        public void SelectAll_NoBots_ClearsTheSelection()
        {
            selection.Select(FirstBot);

            selection.SelectAll(new List<Bot>());

            Assert.AreEqual(0, selection.Count);
        }

        [Test]
        public void SelectAll_ListChangedAfterwards_KeepsItsOwnCopy()
        {
            var picked = new List<Bot> { FirstBot, SecondBot };
            selection.SelectAll(picked);

            picked.Clear();

            Assert.AreEqual(2, selection.Count);
        }

        [Test]
        public void Clear_ManyBotsSelected_SelectsNothing()
        {
            selection.SelectAll(new List<Bot> { FirstBot, SecondBot });

            selection.Clear();

            Assert.AreEqual(0, selection.Count);
        }
    }
}
