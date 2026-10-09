using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvBlockBuilder
    {
        private readonly CvBlockCutter cutter = new CvBlockCutter();
        private readonly CvEntryTextFormatter formatter = new CvEntryTextFormatter();

        public List<CvBlock> Build(CvDocument document)
        {
            var blocks = new List<CvBlock>();
            AddBlock(blocks, cutter.Cut(CvItemId.Name, document.Header.Name), CvBlockRole.Name);
            AddBlock(blocks, cutter.Cut(CvItemId.JobTitle, document.Header.Title), CvBlockRole.JobTitle);
            foreach (CvContact contact in document.Header.Contacts)
                AddBlock(blocks, cutter.Cut(contact.Id, contact.Text), CvBlockRole.Contact);
            foreach (CvSection section in document.Sections)
                foreach (CvEntry entry in section.Entries)
                    AddBlock(blocks, CutEntry(entry, section.Style, document.Categories), CvBlockRole.Entry);
            return blocks;
        }

        private static string NameOf(string categoryId, IReadOnlyList<CvCategory> categories)
        {
            foreach (CvCategory category in categories)
                if (category.Id == categoryId)
                    return category.Name;

            return string.Empty;
        }

        private CvBlock CutEntry(CvEntry entry, CvListStyle style, IReadOnlyList<CvCategory> categories)
        {
            CvBlock block = CutEntryText(entry, style);
            block.Weight = entry.Weight;
            block.Category = entry.Category ?? string.Empty;
            block.CategoryName = NameOf(block.Category, categories);
            return block;
        }

        private CvBlock CutEntryText(CvEntry entry, CvListStyle style)
        {
            if (style == CvListStyle.Inline)
                return cutter.Cut(entry.Id, formatter.FormatChip(entry));

            CvEntryTexts texts = formatter.Format(entry, style);
            return cutter.Cut(entry.Id, texts.Heading, texts.Meta, texts.Body);
        }

        private static void AddBlock(List<CvBlock> blocks, CvBlock block, CvBlockRole role)
        {
            if (block.LetterCount == 0)
                return;

            block.Role = role;
            blocks.Add(block);
        }
    }
}
