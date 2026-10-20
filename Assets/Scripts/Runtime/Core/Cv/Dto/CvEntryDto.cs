using System;

namespace CvWarehouse.Core.Cv.Dto
{
    [Serializable]
    public sealed class CvEntryDto
    {
        public string title;
        public string subtitle;
        public string meta;
        public string description;
        public string[] bullets;
        public string link;
        public string importance;
        public string category;
    }
}
