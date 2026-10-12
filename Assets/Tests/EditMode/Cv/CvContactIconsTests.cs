using CvWarehouse.Presentation.Cv;
using NUnit.Framework;
using UnityEditor;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvContactIconsTests
    {
        private const string AssetPath = "Assets/Settings/CvContactIcons.asset";

        private CvContactIcons icons;

        [SetUp]
        public void SetUp()
        {
            icons = AssetDatabase.LoadAssetAtPath<CvContactIcons>(AssetPath);
            Assert.IsNotNull(icons, "The contact icons asset is missing.");
        }

        [TestCase("phone", "ContactPhone")]
        [TestCase("email", "ContactEmail")]
        [TestCase("github", "ContactGithub")]
        [TestCase("itch", "ContactItch")]
        [TestCase("linkedin", "ContactLinkedin")]
        [TestCase("location", "ContactLocation")]
        public void IconOf_KnownKind_IsItsOwnIcon(string kind, string expectedSprite)
        {
            Assert.AreEqual(expectedSprite, icons.IconOf(kind).name);
        }

        [Test]
        public void IconOf_KindInAnotherLetterCase_IsStillFound()
        {
            Assert.AreEqual("ContactEmail", icons.IconOf("EMail").name);
        }

        [TestCase("carrier pigeon")]
        [TestCase("")]
        [TestCase(null)]
        public void IconOf_UnknownOrMissingKind_IsTheGenericLinkIcon(string kind)
        {
            Assert.AreEqual("ContactLink", icons.IconOf(kind).name);
        }
    }
}
