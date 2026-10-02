using System.IO;
using CvWarehouse.Core.Cv;
using NUnit.Framework;
using UnityEngine;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvParserTests
    {
        private const string MinimalCv =
            "{\"name\":\"Ada\",\"sections\":[{\"title\":\"Profile\",\"text\":\"Hello\"}]}";

        private CvParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new CvParser();
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Parse_EmptyText_ReturnsReadableError(string json)
        {
            CvParseResult result = parser.Parse(json);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("The CV file is empty.", result.Error);
        }

        [TestCase("{\"name\": ")]
        [TestCase("not json at all")]
        [TestCase("[1, 2, 3]")]
        public void Parse_MalformedJson_ReturnsErrorInsteadOfThrowing(string json)
        {
            CvParseResult result = parser.Parse(json);

            Assert.IsFalse(result.IsValid);
            StringAssert.StartsWith("The CV file is not valid JSON.", result.Error);
        }

        [TestCase("{}")]
        [TestCase("{\"name\":\"Ada\",\"sections\":[]}")]
        [TestCase("{\"name\":\"Ada\",\"sections\":[{\"title\":\"Empty\",\"type\":\"list\",\"entries\":[]}]}")]
        public void Parse_NoSectionWithContent_ReturnsError(string json)
        {
            CvParseResult result = parser.Parse(json);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("The CV file has no sections with content.", result.Error);
        }

        [Test]
        public void Parse_MinimalFile_FillsDefaults()
        {
            CvDocument document = ParseValid(MinimalCv);

            Assert.AreEqual("Ada", document.Header.Name);
            Assert.AreEqual(string.Empty, document.Header.Title);
            Assert.AreEqual(string.Empty, document.Footer);
            Assert.IsEmpty(document.Header.Contacts);
            Assert.IsEmpty(document.Categories);
        }

        [Test]
        public void Parse_MissingName_WarnsAndStaysValid()
        {
            CvParseResult result = parser.Parse("{\"sections\":[{\"title\":\"Profile\",\"text\":\"Hello\"}]}");

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.Document.Header.Name);
            CollectionAssert.Contains(result.Warnings, "The CV file has no name.");
        }

        [Test]
        public void Parse_ParagraphSection_BecomesSingleEntry()
        {
            CvSection section = ParseValid(MinimalCv).Sections[0];

            Assert.AreEqual(CvSectionType.Paragraph, section.Type);
            Assert.AreEqual(1, section.Entries.Count);
            Assert.AreEqual("Hello", section.Entries[0].Description);
            Assert.AreEqual("section.0.entry.0", section.Entries[0].Id);
        }

        [Test]
        public void Parse_SectionWithoutType_IsInferredFromContent()
        {
            CvDocument document = ParseValid(
                "{\"sections\":[{\"text\":\"Hello\"},{\"entries\":[{\"title\":\"One\"}]}]}");

            Assert.AreEqual(CvSectionType.Paragraph, document.Sections[0].Type);
            Assert.AreEqual(CvSectionType.List, document.Sections[1].Type);
        }

        [Test]
        public void Parse_EntryWithOnlyTitle_GetsDefaultsForEverythingElse()
        {
            CvEntry entry = ParseValid("{\"sections\":[{\"entries\":[{\"title\":\"One\"}]}]}").Sections[0].Entries[0];

            Assert.AreEqual("One", entry.Title);
            Assert.AreEqual(string.Empty, entry.Subtitle);
            Assert.AreEqual(string.Empty, entry.Meta);
            Assert.AreEqual(string.Empty, entry.Description);
            Assert.AreEqual(string.Empty, entry.Link);
            Assert.AreEqual(string.Empty, entry.Category);
            Assert.AreEqual(string.Empty, entry.Effect);
            Assert.AreEqual(WeightClass.Medium, entry.Weight);
            Assert.IsEmpty(entry.Bullets);
        }

        [Test]
        public void Parse_UnknownKeywords_FallBackWithWarnings()
        {
            CvParseResult result = parser.Parse(
                "{\"name\":\"Ada\",\"sections\":[{\"title\":\"S\",\"type\":\"table\",\"style\":\"fancy\","
                + "\"entries\":[{\"title\":\"One\",\"weight\":\"huge\",\"category\":\"missing\"}]}]}");

            CvSection section = result.Document.Sections[0];
            Assert.AreEqual(CvSectionType.List, section.Type);
            Assert.AreEqual(CvListStyle.Rows, section.Style);
            Assert.AreEqual(WeightClass.Medium, section.Entries[0].Weight);
            Assert.AreEqual(string.Empty, section.Entries[0].Category);
            Assert.AreEqual(4, result.Warnings.Count);
        }

        [Test]
        public void Parse_KeywordsInAnyLetterCase_AreRecognised()
        {
            CvSection section = ParseValid(
                "{\"sections\":[{\"type\":\"LIST\",\"style\":\"keyvalue\","
                + "\"entries\":[{\"title\":\"One\",\"weight\":\"Heavy\"}]}]}").Sections[0];

            Assert.AreEqual(CvListStyle.KeyValue, section.Style);
            Assert.AreEqual(WeightClass.Heavy, section.Entries[0].Weight);
        }

        [Test]
        public void Parse_EmptySectionAndEmptyEntry_AreSkippedWithoutShiftingIds()
        {
            CvDocument document = ParseValid(
                "{\"sections\":[{\"title\":\"Empty\",\"entries\":[]},"
                + "{\"title\":\"Full\",\"entries\":[{\"meta\":\"2020\"},{\"title\":\"Kept\",\"bullets\":[\" \",\"Point\"]}]}]}");

            Assert.AreEqual(1, document.Sections.Count);
            CvEntry entry = document.Sections[0].Entries[0];
            Assert.AreEqual("section.1.entry.1", entry.Id);
            CollectionAssert.AreEqual(new[] { "Point" }, entry.Bullets);
        }

        [Test]
        public void Parse_Contacts_SkipsOnesWithoutText()
        {
            CvDocument document = ParseValid(
                "{\"contacts\":[{\"kind\":\"email\"},{\"kind\":\"github\",\"text\":\"github.com/ada\"}],"
                + "\"sections\":[{\"text\":\"Hello\"}]}");

            Assert.AreEqual(1, document.Header.Contacts.Count);
            Assert.AreEqual("contact.1", document.Header.Contacts[0].Id);
            Assert.AreEqual(string.Empty, document.Header.Contacts[0].Link);
        }

        [Test]
        public void Parse_Categories_SkipsRepeatedIdsAndDefaultsName()
        {
            CvDocument document = ParseValid(
                "{\"categories\":[{\"id\":\"technical\"},{\"id\":\"technical\",\"name\":\"Again\"},{\"name\":\"No id\"}],"
                + "\"sections\":[{\"entries\":[{\"title\":\"One\",\"category\":\"technical\"}]}]}");

            Assert.AreEqual(1, document.Categories.Count);
            Assert.AreEqual("technical", document.Categories[0].Name);
            Assert.AreEqual("technical", document.Sections[0].Entries[0].Category);
        }

        [Test]
        public void Parse_SampleCvFile_IsValidWithoutWarnings()
        {
            string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "cv_en.json"));

            CvParseResult result = parser.Parse(json);

            Assert.IsTrue(result.IsValid, result.Error);
            Assert.IsEmpty(result.Warnings);
            Assert.AreEqual("Radosław Jasiński", result.Document.Header.Name);
            Assert.AreEqual(4, result.Document.Header.Contacts.Count);
            Assert.AreEqual(8, result.Document.Sections.Count);
        }

        private CvDocument ParseValid(string json)
        {
            CvParseResult result = parser.Parse(json);
            Assert.IsTrue(result.IsValid, result.Error);
            return result.Document;
        }
    }
}
