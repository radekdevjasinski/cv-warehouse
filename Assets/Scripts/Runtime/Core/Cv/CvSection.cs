using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvSection
    {
        public string Title { get; internal set; }
        public CvSectionType Type { get; internal set; }
        public CvListStyle Style { get; internal set; }
        public IReadOnlyList<CvEntry> Entries { get; internal set; }
    }
}
