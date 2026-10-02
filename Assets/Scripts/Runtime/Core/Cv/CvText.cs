namespace CvWarehouse.Core.Cv
{
    internal static class CvText
    {
        public static string Clean(string value)
        {
            return value == null ? string.Empty : value.Trim();
        }
    }
}
