using System.Text.RegularExpressions;

namespace BepKhoiBackend.DataAccess.Helpers
{
    public class ProductValidator
    {
        // Method to check then input of Name or Id is 
        public static void VallidateStringProductNameOrIdInput(string input)
        {
            // check are white space or null
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Wrong input! That have null or white space");
            }

            // Check have Regex are speacial characters
            if(!Regex.IsMatch(input, @"^[\p{L}\p{N}\s]+$"))
            {
                throw new ArgumentException("Wrong input! That have speacial characters");
            }
        }

        // Method create a flag check ID are invalid a number or not
        public static bool IsPositiveInteger(string input)
        {
            return Regex.IsMatch(input, @"^\d+$"); // could be integer and is positive number
        }

        // Method to check ID are invalid, more than 0 or = 0
        public static void ValidatePositiveProductId(int input)
        {
            // Check input less or equal to 0
            if(input <= 0)
            {
                throw new ArgumentException("ID must be positive integer, please re-input");
            }
        }
    }
}
