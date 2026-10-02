using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvHeader
    {
        public string Name { get; internal set; }
        public string Title { get; internal set; }
        public IReadOnlyList<CvContact> Contacts { get; internal set; }
    }
}
