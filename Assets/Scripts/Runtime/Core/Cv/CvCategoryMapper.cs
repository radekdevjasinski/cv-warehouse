using System.Collections.Generic;
using CvWarehouse.Core.Cv.Dto;

namespace CvWarehouse.Core.Cv
{
    internal sealed class CvCategoryMapper
    {
        private readonly List<string> warnings;
        private readonly HashSet<string> knownIds = new HashSet<string>();

        public CvCategoryMapper(List<string> warnings)
        {
            this.warnings = warnings;
        }

        public List<CvCategory> MapAll(CvCategoryDto[] categoryDtos)
        {
            var categories = new List<CvCategory>();
            if (categoryDtos == null)
                return categories;

            foreach (CvCategoryDto categoryDto in categoryDtos)
                AddCategory(categories, categoryDto);
            return categories;
        }

        private void AddCategory(List<CvCategory> categories, CvCategoryDto categoryDto)
        {
            string id = categoryDto == null ? string.Empty : CvText.Clean(categoryDto.id);
            if (id.Length == 0 || !knownIds.Add(id))
            {
                warnings.Add("A category with a missing or repeated id '" + id + "' was skipped.");
                return;
            }

            string name = CvText.Clean(categoryDto.name);
            categories.Add(new CvCategory { Id = id, Name = name.Length == 0 ? id : name });
        }
    }
}
