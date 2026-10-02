using System;

namespace CvWarehouse.Core.Cv.Dto
{
    [Serializable]
    public sealed class CvSectionDto
    {
        public string title;
        public string type;
        public string style;
        public string text;
        public CvEntryDto[] entries;
    }
}
