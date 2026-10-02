using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public static class CvTestBlocks
    {
        public const string ProfileId = "section.0.entry.0";
        public const string NameAndProfileCv =
            "{\"name\":\"Ada Lovelace\",\"sections\":[{\"title\":\"Profile\",\"text\":\"Hello big world\"}]}";

        public static List<CvBlock> Build(string json)
        {
            CvParseResult result = new CvParser().Parse(json);
            Assert.IsTrue(result.IsValid, result.Error);
            return new CvBlockBuilder().Build(result.Document);
        }
    }
}
