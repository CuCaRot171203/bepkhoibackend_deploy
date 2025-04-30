using System.Globalization;
using System.Text;

namespace BepKhoiBackend.BusinessObject.Helpers
{
    public static class StringHelper
    {
        // Remove vietnamese and unicode
        public static string RemoveDiacritics(string input)
        {
            if(string.IsNullOrEmpty(input))
                return string.Empty;

            // Splits basic characters and accents into separate components
            var normalizedString = input.Normalize(NormalizationForm.FormD);

            // filter not by base
            var stringBuilder = new StringBuilder();

            foreach(var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                // keep only original characters (remove accents)
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // standardlize to composed form
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
