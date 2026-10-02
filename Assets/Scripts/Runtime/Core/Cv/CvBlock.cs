using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvBlock
    {
        public string Id { get; internal set; }
        public IReadOnlyList<string> Segments { get; internal set; }
        public IReadOnlyList<int> SegmentFirstLetters { get; internal set; }
        public IReadOnlyList<CvWord> Words { get; internal set; }
        public int LetterCount { get; internal set; }
    }
}
