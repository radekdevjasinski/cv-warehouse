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
            AddBlock(blocks, cutter.Cut(CvItemId.Name, document.Header.Name));
            AddBlock(blocks, cutter.Cut(CvItemId.JobTitle, document.Header.Title));
            foreach (CvContact contact in document.Header.Contacts)
                AddBlock(blocks, cutter.Cut(contact.Id, contact.Text));
            foreach (CvSection section in document.Sections)
                foreach (CvEntry entry in section.Entries)
                    AddBlock(blocks, CutEntry(entry, section.Style));
            return blocks;
        }

        private CvBlock CutEntry(CvEntry entry, CvListStyle style)
        {
            if (style == CvListStyle.Inline)
                return cutter.Cut(entry.Id, formatter.FormatChip(entry));

            CvEntryTexts texts = formatter.Format(entry, style);
            return cutter.Cut(entry.Id, texts.Heading, texts.Meta, texts.Body);
        }

        private static void AddBlock(List<CvBlock> blocks, CvBlock block)
        {
            if (block.LetterCount > 0)
                blocks.Add(block);
        }
    }
}
