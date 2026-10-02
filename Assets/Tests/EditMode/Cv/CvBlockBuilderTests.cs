using System.Collections.Generic;
using System.IO;
using CvWarehouse.Core.Cv;
using NUnit.Framework;
using UnityEngine;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvBlockBuilderTests
    {
        [Test]
        public void Build_NameAndParagraph_CutsLettersWithoutSpaces()
        {
            List<CvBlock> blocks = CvTestBlocks.Build(CvTestBlocks.NameAndProfileCv);

            Assert.AreEqual(2, blocks.Count);
            Assert.AreEqual(CvItemId.Name, blocks[0].Id);
            Assert.AreEqual(11, blocks[0].LetterCount);
            Assert.AreEqual(CvTestBlocks.ProfileId, blocks[1].Id);
            Assert.AreEqual(13, blocks[1].LetterCount);
        }

        [Test]
        public void Build_NameAndParagraph_CutsWordsAtSpaces()
        {
            IReadOnlyList<CvWord> words = CvTestBlocks.Build(CvTestBlocks.NameAndProfileCv)[1].Words;

            Assert.AreEqual(3, words.Count);
            Assert.AreEqual(5, words[1].FirstLetter);
            Assert.AreEqual(3, words[1].LetterCount);
        }

        [Test]
        public void Build_Entry_HasHeadingMetaAndBodySegmentsWithTagsNotCounted()
        {
            CvBlock block = CvTestBlocks.Build(
                "{\"sections\":[{\"entries\":[{\"title\":\"Game\",\"meta\":\"2026\",\"description\":\"A <i>fine</i> one\"}]}]}")[0];

            CollectionAssert.AreEqual(new[] { "<b>Game</b>", "2026", "A <i>fine</i> one" }, block.Segments);
            CollectionAssert.AreEqual(new[] { 0, 4, 8 }, block.SegmentFirstLetters);
            Assert.AreEqual(16, block.LetterCount);
            Assert.AreEqual(5, block.Words.Count);
        }

        [Test]
        public void Build_UnknownTag_IsCountedAsLetters()
        {
            CvBlock block = CvTestBlocks.Build("{\"sections\":[{\"text\":\"a <x> b\"}]}")[0];

            Assert.AreEqual(5, block.LetterCount);
        }

        [Test]
        public void Build_KeyValueEntry_KeepsKeyAndValueAsSeparateWords()
        {
            CvBlock block = CvTestBlocks.Build(
                "{\"sections\":[{\"style\":\"keyValue\",\"entries\":[{\"title\":\"Tools\",\"description\":\"Git\"}]}]}")[0];

            Assert.AreEqual(2, block.Words.Count);
        }

        [Test]
        public void Build_InlineEntry_HasOneSegment()
        {
            CvBlock block = CvTestBlocks.Build(
                "{\"sections\":[{\"style\":\"inline\",\"entries\":[{\"title\":\"English (B2)\"}]}]}")[0];

            CollectionAssert.AreEqual(new[] { "English (B2)" }, block.Segments);
        }

        [Test]
        public void Build_MissingNameAndTitle_HaveNoBlocks()
        {
            List<CvBlock> blocks = CvTestBlocks.Build("{\"sections\":[{\"text\":\"Hello\"}]}");

            Assert.AreEqual(1, blocks.Count);
            Assert.AreEqual(CvTestBlocks.ProfileId, blocks[0].Id);
        }

        [Test]
        public void Build_SampleCvFile_HasOneBlockPerBoxWithUniqueIds()
        {
            string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "cv_en.json"));

            List<CvBlock> blocks = CvTestBlocks.Build(json);

            Assert.AreEqual(28, blocks.Count);
            Assert.AreEqual(CvItemId.Name, blocks[0].Id);
            Assert.AreEqual(16, blocks[0].LetterCount);
            var ids = new List<string>();
            foreach (CvBlock block in blocks)
                ids.Add(block.Id);
            CollectionAssert.AllItemsAreUnique(ids);
        }
    }
}
