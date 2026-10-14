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

        public const int SmallEntryWords = 4;
        public const int MediumEntryWords = 14;
        public const int LargeEntryWords = 30;

        private static readonly string[] Importances = { "normal", "important", "key" };

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

        public static List<CvBlock> BuildCv(int largeEntries, int mediumEntries, int smallEntries)
        {
            var json = new StringBuilder("{\"name\":\"Ada Lovelace\",\"title\":\"Programmer\",");
            json.Append("\"contacts\":[{\"kind\":\"email\",\"text\":\"ada@example.com\"},{\"kind\":\"phone\",\"text\":\"123 456 789\"}],");
            json.Append("\"sections\":[{\"title\":\"Work\",\"entries\":[");
            AppendEntries(json, LargeEntryWords, largeEntries);
            AppendEntries(json, MediumEntryWords, mediumEntries);
            AppendEntries(json, SmallEntryWords, smallEntries);
            json.Length--;
            json.Append("]}]}");
            return Build(json.ToString());
        }

        public static string Words(int count)
        {
            var text = new StringBuilder("word");
            for (int word = 1; word < count; word++)
                text.Append(" word");
            return text.ToString();
        }

        private static void AppendEntries(StringBuilder json, int words, int count)
        {
            for (int entry = 0; entry < count; entry++)
                json.Append("{\"title\":\"").Append(Words(words))
                    .Append("\",\"importance\":\"").Append(Importances[entry % Importances.Length]).Append("\"},");
        }
    }
}
