namespace CvWarehouse.Core.Cv
{
    public static class CvItemId
    {
        public const string Name = "name";
        public const string JobTitle = "title";

        public static string ForContact(int contactIndex)
        {
            return "contact." + contactIndex;
        }

        public static string ForEntry(int sectionIndex, int entryIndex)
        {
            return "section." + sectionIndex + ".entry." + entryIndex;
        }
    }
}
