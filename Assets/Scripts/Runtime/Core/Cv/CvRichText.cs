using System;

namespace CvWarehouse.Core.Cv
{
    internal static class CvRichText
    {
        private static readonly string[] TagNames = { "b", "i", "indent" };

        public static int MeasureTagAt(string text, int start)
        {
            if (text[start] != '<')
                return 0;

            int end = text.IndexOf('>', start);
            if (end < 0)
                return 0;

            int nameStart = start + 1;
            if (nameStart < end && text[nameStart] == '/')
                nameStart++;

            foreach (string tagName in TagNames)
                if (IsTagNamed(text, nameStart, end, tagName))
                    return end - start + 1;
            return 0;
        }

        private static bool IsTagNamed(string text, int nameStart, int end, string tagName)
        {
            int nameEnd = nameStart + tagName.Length;
            if (nameEnd > end)
                return false;
            if (string.Compare(text, nameStart, tagName, 0, tagName.Length, StringComparison.OrdinalIgnoreCase) != 0)
                return false;

            return nameEnd == end || text[nameEnd] == '=';
        }
    }
}
