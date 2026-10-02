using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvEntry
    {
        public string Id { get; internal set; }
        public string Title { get; internal set; }
        public string Subtitle { get; internal set; }
        public string Meta { get; internal set; }
        public string Description { get; internal set; }
        public IReadOnlyList<string> Bullets { get; internal set; }
        public string Link { get; internal set; }
        public WeightClass Weight { get; internal set; }
        public string Category { get; internal set; }
        public string Effect { get; internal set; }
    }
}
