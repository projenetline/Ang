using System;

namespace PazarYeri.BusinessLayer.Helpers
{
    public static class MyExtensions
    {
        public static string ToSqlString(this string value, string logoFirmNr, string logoFirmPer)
        {
            return value.
                Replace("_001", $"_{logoFirmNr}").
                Replace("_01_", $"_{logoFirmPer}_");
        }

        public static bool CaseInsensitiveContains(
            this string text,
            string value,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }
}
