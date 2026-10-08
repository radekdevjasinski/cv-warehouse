using System.Collections.Generic;
using System.Text;
using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public static class CvTestBlocks
    {
        public const string ProfileId = "section.0.entry.0";
        public const string NameAndProfileCv =
            "{\"name\":\"Ada Lovelace\",\"sections\":[{\"title\":\"Profile\",\"text\":\"Hello big world\"}]}";

        private static readonly string[] EntryTexts = { "Unity", "Shipped two games", "Led a team of five people", "C#" };

        public static List<CvBlock> Build(string json)
        {
            CvParseResult result = new CvParser().Parse(json);
            Assert.IsTrue(result.IsValid, result.Error);
            return new CvBlockBuilder().Build(result.Document);
        }

        public static CvBlock FromText(string text)
        {
            return Build("{\"sections\":[{\"text\":\"" + text + "\"}]}")[0];
        }

        public static CvBlock WithLetters(int letterCount)
        {
            return FromText(new string('a', letterCount));
        }

        public static List<CvBlock> BuildCv(int heavyEntries, int mediumEntries, int lightEntries)
        {
            var json = new StringBuilder("{\"name\":\"Ada Lovelace\",\"title\":\"Programmer\",");
            json.Append("\"contacts\":[{\"kind\":\"email\",\"text\":\"ada@example.com\"},{\"kind\":\"phone\",\"text\":\"123 456 789\"}],");
            json.Append("\"sections\":[{\"title\":\"Work\",\"entries\":[");
            AppendEntries(json, "heavy", heavyEntries);
            AppendEntries(json, "medium", mediumEntries);
            AppendEntries(json, "light", lightEntries);
            json.Length--;
            json.Append("]}]}");
            return Build(json.ToString());
        }

        private static void AppendEntries(StringBuilder json, string weight, int count)
        {
            for (int entry = 0; entry < count; entry++)
                json.Append("{\"title\":\"").Append(EntryTexts[entry % EntryTexts.Length])
                    .Append("\",\"weight\":\"").Append(weight).Append("\"},");
        }
    }
}
