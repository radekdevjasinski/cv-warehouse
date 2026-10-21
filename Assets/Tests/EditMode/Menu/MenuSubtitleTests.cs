using CvWarehouse.Core.Menu;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Menu
{
    public sealed class MenuSubtitleTests
    {
        private const string NameColour = "#F4ECD8";

        [Test]
        public void For_Name_AddsTheHighlightedName()
        {
            string subtitle = MenuSubtitle.For("Ada Lovelace", NameColour);

            Assert.AreEqual("an interactive CV by <color=#F4ECD8><b>Ada Lovelace</b></color>", subtitle);
        }

        [Test]
        public void For_NameWithSpacesAround_TrimsTheName()
        {
            string subtitle = MenuSubtitle.For("  Ada  ", NameColour);

            StringAssert.Contains("<b>Ada</b>", subtitle);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void For_NoName_LeavesTheNameOut(string name)
        {
            Assert.AreEqual(MenuSubtitle.WithoutName, MenuSubtitle.For(name, NameColour));
        }
    }
}
