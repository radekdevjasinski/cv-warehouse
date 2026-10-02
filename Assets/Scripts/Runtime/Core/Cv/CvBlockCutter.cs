using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    internal sealed class CvBlockCutter
    {
        private const int NoOpenWord = -1;

        public CvBlock Cut(string id, params string[] segments)
        {
            var words = new List<CvWord>();
            var segmentFirstLetters = new int[segments.Length];
            int letterCount = 0;
            for (int segmentIndex = 0; segmentIndex < segments.Length; segmentIndex++)
            {
                segmentFirstLetters[segmentIndex] = letterCount;
                letterCount = CutSegment(segments[segmentIndex], letterCount, words);
            }

            return new CvBlock
            {
                Id = id,
                Segments = segments,
                SegmentFirstLetters = segmentFirstLetters,
                Words = words,
                LetterCount = letterCount
            };
        }

        private static int CutSegment(string segment, int letterCount, List<CvWord> words)
        {
            int wordStart = NoOpenWord;
            for (int index = 0; index < segment.Length; index++)
            {
                int tagLength = CvRichText.MeasureTagAt(segment, index);
                if (tagLength > 0)
                {
                    index += tagLength - 1;
                }
                else if (char.IsWhiteSpace(segment[index]))
                {
                    CloseWord(words, wordStart, letterCount);
                    wordStart = NoOpenWord;
                }
                else
                {
                    if (wordStart == NoOpenWord)
                        wordStart = letterCount;
                    letterCount++;
                }
            }

            CloseWord(words, wordStart, letterCount);
            return letterCount;
        }

        private static void CloseWord(List<CvWord> words, int wordStart, int letterCount)
        {
            if (wordStart != NoOpenWord)
                words.Add(new CvWord(wordStart, letterCount - wordStart));
        }
    }
}
