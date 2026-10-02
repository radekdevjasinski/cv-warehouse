using System;

namespace CvWarehouse.Core.Cv.Dto
{
    [Serializable]
    public sealed class CvFileDto
    {
        public string name;
        public string title;
        public CvContactDto[] contacts;
        public CvCategoryDto[] categories;
        public CvSectionDto[] sections;
        public string footer;
    }
}
