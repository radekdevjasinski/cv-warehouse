using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvEntryTextFormatterTests
    {
        private const string FullEntryCv =
            "{\"sections\":[{\"entries\":[{\"title\":\"Game\",\"subtitle\":\"Unity\",\"meta\":\"2026\","
            + "\"description\":\"A shooter.\",\"bullets\":[\"First\",\"Second\"]}]}]}";

        private CvEntryTextFormatter formatter;

        [SetUp]
        public void SetUp()
        {
            formatter = new CvEntryTextFormatter();
        }

        [Test]
        public void Format_RowsStyle_PutsSubtitleAndBulletsInBody()
        {
            CvEntryTexts texts = formatter.Format(ParseFirstEntry(FullEntryCv), CvListStyle.Rows);

            Assert.AreEqual("<b>Game</b>", texts.Heading);
            Assert.AreEqual("2026", texts.Meta);
            Assert.AreEqual("<i>Unity</i>\nA shooter.\n• First\n• Second", texts.Body);
        }

        [Test]
        public void Format_BulletsStyle_PutsSubtitleInHeadingAndIndentsBody()
        {
            CvEntryTexts texts = formatter.Format(ParseFirstEntry(FullEntryCv), CvListStyle.Bullets);

            Assert.AreEqual("• <b>Game</b> — Unity", texts.Heading);
            Assert.AreEqual("<indent=1em>A shooter.\n• First\n• Second", texts.Body);
        }

        [Test]
        public void Format_KeyValueStyle_PutsTitleAndDescriptionOnOneLine()
        {
            CvEntry entry = ParseFirstEntry("{\"sections\":[{\"entries\":[{\"title\":\"Tools\",\"description\":\"Git\"}]}]}");

            CvEntryTexts texts = formatter.Format(entry, CvListStyle.KeyValue);

            Assert.AreEqual(string.Empty, texts.Heading);
            Assert.AreEqual("<b>Tools</b> <indent=24%>Git</indent>", texts.Body);
        }

        [Test]
        public void Format_ParagraphEntry_HasOnlyBody()
        {
            CvEntry entry = ParseFirstEntry("{\"sections\":[{\"text\":\"Hello\"}]}");

            CvEntryTexts texts = formatter.Format(entry, CvListStyle.Rows);

            Assert.AreEqual(string.Empty, texts.Heading);
            Assert.AreEqual(string.Empty, texts.Meta);
            Assert.AreEqual("Hello", texts.Body);
        }

        [Test]
        public void Format_EntryWithLinkButNoMeta_ShowsLinkAsMeta()
        {
            CvEntry entry = ParseFirstEntry(
                "{\"sections\":[{\"entries\":[{\"title\":\"Game\",\"link\":\"https://itch.io/game\"}]}]}");

            Assert.AreEqual("https://itch.io/game", formatter.Format(entry, CvListStyle.Rows).Meta);
        }

        [Test]
        public void FormatChip_EntryWithoutTitle_FallsBackToDescription()
        {
            CvEntry entry = ParseFirstEntry("{\"sections\":[{\"entries\":[{\"description\":\"Polish\"}]}]}");

            Assert.AreEqual("Polish", formatter.FormatChip(entry));
        }

        private static CvEntry ParseFirstEntry(string json)
        {
            CvParseResult result = new CvParser().Parse(json);
            Assert.IsTrue(result.IsValid, result.Error);
            return result.Document.Sections[0].Entries[0];
        }
    }
}
