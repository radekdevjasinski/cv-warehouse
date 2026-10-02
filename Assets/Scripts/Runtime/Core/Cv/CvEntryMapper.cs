using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv.Dto;

namespace CvWarehouse.Core.Cv
{
    internal sealed class CvEntryMapper
    {
        private readonly IReadOnlyList<CvCategory> categories;
        private readonly List<string> warnings;

        public CvEntryMapper(IReadOnlyList<CvCategory> categories, List<string> warnings)
        {
            this.categories = categories;
            this.warnings = warnings;
        }

        public List<CvEntry> MapParagraph(string text, int sectionIndex)
        {
            var entries = new List<CvEntry>();
            string description = CvText.Clean(text);
            if (description.Length > 0)
                entries.Add(Map(new CvEntryDto { description = description }, CvItemId.ForEntry(sectionIndex, 0)));
            return entries;
        }

        public List<CvEntry> MapAll(CvEntryDto[] entryDtos, int sectionIndex)
        {
            var entries = new List<CvEntry>();
            if (entryDtos == null)
                return entries;

            for (int entryIndex = 0; entryIndex < entryDtos.Length; entryIndex++)
            {
                CvEntry entry = Map(entryDtos[entryIndex], CvItemId.ForEntry(sectionIndex, entryIndex));
                if (HasContent(entry))
                    entries.Add(entry);
                else
                    warnings.Add("Entry '" + entry.Id + "' is empty and was skipped.");
            }
            return entries;
        }

        private CvEntry Map(CvEntryDto entryDto, string id)
        {
            CvEntryDto source = entryDto ?? new CvEntryDto();
            return new CvEntry
            {
                Id = id,
                Title = CvText.Clean(source.title),
                Subtitle = CvText.Clean(source.subtitle),
                Meta = CvText.Clean(source.meta),
                Description = CvText.Clean(source.description),
                Bullets = CleanBullets(source.bullets),
                Link = CvText.Clean(source.link),
                Weight = ReadWeight(source.weight, id),
                Category = ReadCategory(source.category, id),
                Effect = CvText.Clean(source.effect)
            };
        }

        private static bool HasContent(CvEntry entry)
        {
            return entry.Title.Length > 0
                || entry.Subtitle.Length > 0
                || entry.Description.Length > 0
                || entry.Bullets.Count > 0;
        }

        private static IReadOnlyList<string> CleanBullets(string[] bullets)
        {
            if (bullets == null)
                return Array.Empty<string>();

            var cleanBullets = new List<string>(bullets.Length);
            foreach (string bullet in bullets)
            {
                string cleanBullet = CvText.Clean(bullet);
                if (cleanBullet.Length > 0)
                    cleanBullets.Add(cleanBullet);
            }
            return cleanBullets;
        }

        private WeightClass ReadWeight(string weight, string id)
        {
            string keyword = CvText.Clean(weight);
            if (CvKeyword.TryParse(keyword, out WeightClass weightClass))
                return weightClass;

            if (keyword.Length > 0)
                warnings.Add("Entry '" + id + "' has unknown weight '" + keyword + "'.");
            return WeightClass.Medium;
        }

        private string ReadCategory(string category, string id)
        {
            string categoryId = CvText.Clean(category);
            if (categoryId.Length == 0)
                return categoryId;

            foreach (CvCategory knownCategory in categories)
                if (knownCategory.Id == categoryId)
                    return categoryId;

            warnings.Add("Entry '" + id + "' has unknown category '" + categoryId + "'.");
            return string.Empty;
        }
    }
}
