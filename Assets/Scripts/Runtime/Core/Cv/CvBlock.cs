using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvBlock
    {
        public const int DecorationGlyph = -1;

        public string Id { get; internal set; }
        public CvBlockRole Role { get; internal set; }
        public Importance Importance { get; internal set; }
        public string Category { get; internal set; }
        public string CategoryName { get; internal set; }
        public IReadOnlyList<string> Segments { get; internal set; }
        public IReadOnlyList<int> SegmentFirstGlyphs { get; internal set; }
        public IReadOnlyList<int> GlyphLetters { get; internal set; }
        public IReadOnlyList<CvWord> Words { get; internal set; }
        public int LetterCount { get; internal set; }

        public int LetterAt(int glyph)
        {
            return glyph >= 0 && glyph < GlyphLetters.Count ? GlyphLetters[glyph] : DecorationGlyph;
        }
    }
}
