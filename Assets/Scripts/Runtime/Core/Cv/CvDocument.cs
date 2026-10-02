using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvDocument
    {
        public CvHeader Header { get; internal set; }
        public IReadOnlyList<CvCategory> Categories { get; internal set; }
        public IReadOnlyList<CvSection> Sections { get; internal set; }
        public string Footer { get; internal set; }
    }
}
