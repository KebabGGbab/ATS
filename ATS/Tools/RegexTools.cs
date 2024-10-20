using System.Text.RegularExpressions;

namespace ATS.Tools
{
    public static partial class RegexTools
    {
        [GeneratedRegex("\\d")]
        public static partial Regex CheckContainsDigit();

        [GeneratedRegex("^\\+\\d\\s\\(\\d{3}\\)\\s\\d{3}-\\d{2}-\\d{2}$")]
        public static partial Regex CheckStringIsPhoneNumber();

        public static bool StringContainsDigit(string valid)
        {
            return !string.IsNullOrEmpty(CheckContainsDigit().Match(valid).Value);
        }

        public static bool StringIsPhoneNumber(string valid)
        {
            return !string.IsNullOrEmpty(CheckStringIsPhoneNumber().Match(valid).Value);
        }
    }
}
