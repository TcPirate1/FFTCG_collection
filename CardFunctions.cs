using System.Text.RegularExpressions;
using Sprache;

namespace FFTCG_collection
{
    internal class CardFunctions
    {
        internal static string CheckEmptyString(string input)
        {
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("\nNo input detected. Please re-enter.");
                input = Console.ReadLine()!.Trim();
            }
            return input;
        }
        internal static string FirstCharUpper(string input)
        {
            // Regex to match input to start of a string and whenever there is a space, single quote or hyphen and replace the character with the upper case version.
            const string regex = @"\b[a-zA-Z]|(?<=[ '-])[a-zA-Z]";
            Regex capitalizeRegex = new(regex);
            return capitalizeRegex.Replace(input, m => m.Value.ToUpper());
        }

        internal static string[] FirstCharUpper(IEnumerable<string> input)
        {
            return input.Select(FirstCharUpper).ToArray();
        }

        internal static bool CardRegex(string regex)
        {
            // Regex for regular and promo card codes
            const string cardCodeRegex = @"^\d{1,2}-\d{3}[CRHLS]+$";
            const string promoCodeRegex = @"^PR-\d{3}";
            Regex normalRegex = new(cardCodeRegex);
            Regex promoRegex = new(promoCodeRegex);
            return normalRegex.IsMatch(regex) || promoRegex.IsMatch(regex);
        }
        internal static string GetValidCardCode()
        {
            string code = Console.ReadLine()!.Trim().ToUpper();
            return ValidateAndFormatCardCode(code);
        }
        internal static string GetValidName()
        {
            // If name is equal to the regex it is not a valid card name.
            string name = Console.ReadLine()!.Trim().ToUpper();
            while (CardRegex(name))
            {
                Console.WriteLine("Invalid name. Please re-enter");
                name = Console.ReadLine()!.Trim().ToUpper();
            }
            return FirstCharUpper(name.ToLower());
        }
        internal static string ValidateAndFormatCardCode(string code)
        {
            code = code.Trim().ToUpper();
            while (!CardRegex(code))
            {
                Console.WriteLine("Invalid card code. Please re-enter.");
                code = Console.ReadLine()!.Trim().ToUpper();
            }
            return code;
        }

        internal static int GetValidCost()
        {
            int cost;
            string input = Console.ReadLine()!.Trim();
            while (!int.TryParse(input, out cost) || cost < 1 || cost > 11)
            {
                Console.WriteLine("\nInvalid cost or you inputted a number lower than 1 and higher than 11.\nThere is currently no higher cost than 11.\nPlease input a number between the range.");
                input = Console.ReadLine()!.Trim();
            }
            return cost;
        }
        internal static int GetValidCopies()
        {
            int copies;
            string input = Console.ReadLine()!.Trim();
            while (!int.TryParse(input, out copies) || copies == 0)
            {
                Console.WriteLine("\nInvalid number or 0 was inputted.\nThere shouldn't be 0 copies of a card in the collection.\nPlease enter a valid number.");
                input = Console.ReadLine()!.Trim();
            }
            return copies;
        }

        internal static string[] ParseAndFormatInputArray(string input)
        {
            // Splits the array by commas and then capitalizes the words
            string[] inputArray = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return FirstCharUpper(inputArray);
        }

        internal static bool GetIsFoil()
        {
            char foilValid = char.ToLower(Console.ReadLine()!.Trim()[0]);
            while (foilValid != 'y' && foilValid != 'n')
            {
                Console.WriteLine("Invalid. Please enter either 'y' or 'n'");
                foilValid = char.ToLower(Console.ReadLine()!.Trim()[0]);
            }
            return foilValid == 'y';
        }
    }
}