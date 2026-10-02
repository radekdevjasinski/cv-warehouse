using System;

namespace CvWarehouse.Core.Cv
{
    internal static class CvKeyword
    {
        public static bool TryParse<TEnum>(string keyword, out TEnum value) where TEnum : struct
        {
            return Enum.TryParse(keyword, true, out value) && Enum.IsDefined(typeof(TEnum), value);
        }
    }
}
