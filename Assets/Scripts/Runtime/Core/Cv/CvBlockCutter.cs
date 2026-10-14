using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    internal sealed class CvBlockCutter
    {
        private const int NoToken = -1;

        private readonly List<int> glyphLetters = new List<int>();
        private List<CvWord> words;
        private int letterCount;
        private int tokenStart;
        private bool tokenHasLetter;

        public CvBlock Cut(string id, params string[] segments)
        {
            glyphLetters.Clear();
            words = new List<CvWord>();
            letterCount = 0;
            var segmentFirstGlyphs = new int[segments.Length];
            for (int segmentIndex = 0; segmentIndex < segments.Length; segmentIndex++)
            {
                segmentFirstGlyphs[segmentIndex] = glyphLetters.Count;
                CutSegment(segments[segmentIndex]);
            }

            return new CvBlock
            {
                Id = id,
                Category = string.Empty,
                CategoryName = string.Empty,
                Segments = segments,
                SegmentFirstGlyphs = segmentFirstGlyphs,
                GlyphLetters = glyphLetters.ToArray(),
                Words = words,
                LetterCount = letterCount
            };
        }

        private void CutSegment(string segment)
        {
            tokenStart = NoToken;
            for (int index = 0; index < segment.Length; index++)
            {
                int tagLength = CvRichText.MeasureTagAt(segment, index);
                if (tagLength > 0)
                    index += tagLength - 1;
                else if (char.IsWhiteSpace(segment[index]))
                    CloseToken();
                else
                    AddGlyph(segment[index]);
            }

            CloseToken();
        }

        private void AddGlyph(char glyph)
        {
            if (tokenStart == NoToken)
            {
                tokenStart = glyphLetters.Count;
                tokenHasLetter = false;
            }

            tokenHasLetter |= char.IsLetterOrDigit(glyph);
            glyphLetters.Add(CvBlock.DecorationGlyph);
        }

        private void CloseToken()
        {
            if (tokenStart == NoToken)
                return;

            if (tokenHasLetter)
                MakeWord(tokenStart);
            tokenStart = NoToken;
        }

        private void MakeWord(int firstGlyph)
        {
            int firstLetter = letterCount;
            for (int glyph = firstGlyph; glyph < glyphLetters.Count; glyph++)
                glyphLetters[glyph] = letterCount++;
            words.Add(new CvWord(firstLetter, letterCount - firstLetter));
        }
    }
}
