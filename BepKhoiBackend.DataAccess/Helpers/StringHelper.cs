using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BepKhoiBackend.DataAccess.Helpers
{
    public static class StringHelper
    {
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            text = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static void ValidateSearchInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Wrong input! The search keyword cannot be null or empty.");
            }

            if (!Regex.IsMatch(input, @"^[\p{L}\p{N}\s]+$"))
            {
                throw new ArgumentException("Wrong input! The search keyword contains special characters.");
            }
        }
    }
}
