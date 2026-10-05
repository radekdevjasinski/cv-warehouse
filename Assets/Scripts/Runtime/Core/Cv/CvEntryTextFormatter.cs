using System.Text;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvEntryTextFormatter
    {
        private const string BulletPrefix = "• ";
        private const string BulletBodyIndent = "<indent=1em>";

        private readonly StringBuilder builder = new StringBuilder();

        public CvEntryTexts Format(CvEntry entry, CvListStyle style)
        {
            return new CvEntryTexts(FormatHeading(entry, style), FormatMeta(entry), FormatBody(entry, style));
        }

        public string FormatChip(CvEntry entry)
        {
            return entry.Title.Length > 0 ? entry.Title : entry.Description;
        }

        private static string FormatHeading(CvEntry entry, CvListStyle style)
        {
            if (entry.Title.Length == 0)
                return string.Empty;

            string title = "<b>" + entry.Title + "</b>";
            if (style != CvListStyle.Bullets)
                return title;

            return entry.Subtitle.Length == 0
                ? BulletPrefix + title
                : BulletPrefix + title + " — " + entry.Subtitle;
        }

        private static string FormatMeta(CvEntry entry)
        {
            return entry.Meta.Length > 0 ? entry.Meta : entry.Link;
        }

        private string FormatBody(CvEntry entry, CvListStyle style)
        {
            builder.Clear();
            AppendDescription(entry, style);

            foreach (string bullet in entry.Bullets)
                AppendLine(BulletPrefix + bullet);

            if (style == CvListStyle.Bullets && builder.Length > 0)
                builder.Insert(0, BulletBodyIndent);
            return builder.ToString();
        }

        private void AppendDescription(CvEntry entry, CvListStyle style)
        {
            bool isSubtitleInHeading = style == CvListStyle.Bullets && entry.Title.Length > 0;
            if (!isSubtitleInHeading && entry.Subtitle.Length > 0)
                AppendLine("<i>" + entry.Subtitle + "</i>");
            AppendLine(entry.Description);
        }

        private void AppendLine(string line)
        {
            if (line.Length == 0)
                return;

            if (builder.Length > 0)
                builder.Append('\n');
            builder.Append(line);
        }
    }
}
