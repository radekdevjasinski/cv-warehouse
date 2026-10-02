using System.Collections.Generic;
using CvWarehouse.Core.Cv.Dto;

namespace CvWarehouse.Core.Cv
{
    internal sealed class CvSectionMapper
    {
        private readonly CvEntryMapper entryMapper;
        private readonly List<string> warnings;

        public CvSectionMapper(IReadOnlyList<CvCategory> categories, List<string> warnings)
        {
            entryMapper = new CvEntryMapper(categories, warnings);
            this.warnings = warnings;
        }

        public List<CvSection> MapAll(CvSectionDto[] sectionDtos)
        {
            var sections = new List<CvSection>();
            if (sectionDtos == null)
                return sections;

            for (int sectionIndex = 0; sectionIndex < sectionDtos.Length; sectionIndex++)
                AddSection(sections, sectionDtos[sectionIndex], sectionIndex);
            return sections;
        }

        private void AddSection(List<CvSection> sections, CvSectionDto sectionDto, int sectionIndex)
        {
            if (sectionDto == null)
                return;

            string title = CvText.Clean(sectionDto.title);
            CvSectionType type = ReadType(sectionDto, title);
            List<CvEntry> entries = type == CvSectionType.Paragraph
                ? entryMapper.MapParagraph(sectionDto.text, sectionIndex)
                : entryMapper.MapAll(sectionDto.entries, sectionIndex);
            if (entries.Count == 0)
            {
                warnings.Add("Section '" + title + "' is empty and was skipped.");
                return;
            }

            sections.Add(new CvSection
            {
                Title = title,
                Type = type,
                Style = ReadStyle(sectionDto, title),
                Entries = entries
            });
        }

        private CvSectionType ReadType(CvSectionDto sectionDto, string title)
        {
            string keyword = CvText.Clean(sectionDto.type);
            if (CvKeyword.TryParse(keyword, out CvSectionType type))
                return type;

            if (keyword.Length > 0)
                warnings.Add("Section '" + title + "' has unknown type '" + keyword + "'.");
            return CvText.Clean(sectionDto.text).Length > 0 ? CvSectionType.Paragraph : CvSectionType.List;
        }

        private CvListStyle ReadStyle(CvSectionDto sectionDto, string title)
        {
            string keyword = CvText.Clean(sectionDto.style);
            if (CvKeyword.TryParse(keyword, out CvListStyle style))
                return style;

            if (keyword.Length > 0)
                warnings.Add("Section '" + title + "' has unknown style '" + keyword + "'.");
            return CvListStyle.Rows;
        }
    }
}
