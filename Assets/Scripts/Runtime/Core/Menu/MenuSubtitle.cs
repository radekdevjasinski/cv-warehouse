namespace CvWarehouse.Core.Menu
{
    public static class MenuSubtitle
    {
        public const string WithoutName = "an interactive CV";

        public static string For(string name, string nameColour)
        {
            string trimmedName = name == null ? string.Empty : name.Trim();
            if (trimmedName.Length == 0)
                return WithoutName;

            return WithoutName + " by <color=" + nameColour + "><b>" + trimmedName + "</b></color>";
        }
    }
}
