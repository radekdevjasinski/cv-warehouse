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
        public void Toggle_BotNotSelected_AddsItToTheSelection()
        {
            selection.Select(FirstBot);

            selection.Toggle(SecondBot);

            Assert.IsTrue(selection.HasManyBots);
            Assert.IsTrue(selection.Contains(FirstBot));
            Assert.IsTrue(selection.Contains(SecondBot));
        }

        [Test]
        public void Toggle_BotAlreadySelected_RemovesOnlyThatBot()
        {
            selection.SelectAll(new List<Bot> { FirstBot, SecondBot });

            selection.Toggle(FirstBot);

            Assert.AreSame(SecondBot, selection.SingleBot);
        }

        [Test]
        public void Toggle_TheOnlySelectedBot_ClearsTheSelection()
        {
            selection.Select(FirstBot);

            selection.Toggle(FirstBot);

            Assert.AreEqual(0, selection.Count);
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

        [Test]
        public void Select_Bot_AnnouncesThatBot()
        {
            List<Bot> announced = RecordAnnouncements();

            selection.Select(FirstBot);

            CollectionAssert.AreEqual(new[] { FirstBot }, announced);
        }

        [Test]
        public void Toggle_BotAdded_AnnouncesOnlyTheAddedBot()
        {
            selection.Select(FirstBot);
            List<Bot> announced = RecordAnnouncements();

            selection.Toggle(SecondBot);

            CollectionAssert.AreEqual(new[] { SecondBot }, announced);
        }

        [Test]
        public void Toggle_BotRemoved_AnnouncesNothing()
        {
            selection.Select(FirstBot);
            List<Bot> announced = RecordAnnouncements();

            selection.Toggle(FirstBot);

            Assert.AreEqual(0, announced.Count);
        }

        [Test]
        public void SelectAll_ManyBots_AnnouncesThemTogether()
        {
            int announcements = 0;
            List<Bot> announced = RecordAnnouncements();
            selection.BotsSelected += bots => announcements++;

            selection.SelectAll(new List<Bot> { FirstBot, SecondBot });

            Assert.AreEqual(1, announcements);
            CollectionAssert.AreEqual(new[] { FirstBot, SecondBot }, announced);
        }

        [Test]
        public void SelectAll_NoBots_AnnouncesNothing()
        {
            List<Bot> announced = RecordAnnouncements();

            selection.SelectAll(new List<Bot>());

            Assert.AreEqual(0, announced.Count);
        }

        [Test]
        public void Clear_BotsSelected_AnnouncesNothing()
        {
            selection.Select(FirstBot);
            List<Bot> announced = RecordAnnouncements();

            selection.Clear();

            Assert.AreEqual(0, announced.Count);
        }

        private List<Bot> RecordAnnouncements()
        {
            var announced = new List<Bot>();
            selection.BotsSelected += bots => announced.AddRange(bots);
            return announced;
        }
    }
}
