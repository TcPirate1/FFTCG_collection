using MongoDB.Bson;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace FFTCG_collection
{
    internal class Card
    {
        [BsonId]
        private ObjectId Id { get; set; }

        [BsonElement("Card_name")]
        private string Name { get; set; }

        [BsonElement("Image_location")]
        private string Image { get; set; }

        [BsonElement("Type")]
        private string Type { get; set; }

        [BsonElement("Cost")]
        private int Cost { get; set; }

        [BsonElement("Special_icons")]
        private string[] SpecialIcons { get; set; }

        [BsonElement("Elements")]
        private string[] Elements { get; set; }

        [BsonElement("Card_code")]
        private string Code { get; set; }

        [BsonElement("Copies")]
        private int Copies { get; set; }

        [BsonElement("Foil?")]
        private bool IsFoil { get; set; }

        public Card(string name, string image, string type, int cost, string[] specialIcons, string[] elements, string code, int copies, bool isFoil)
        {
            Name = FirstCharUpper(name);
            Image = image.Trim();
            Type = FirstCharUpper(type);
            Cost = cost;
            SpecialIcons = FirstCharUpper(specialIcons);
            Elements = FirstCharUpper(elements);
            Code = ValidateAndFormatCardCode(code);
            Copies = copies;
            IsFoil = isFoil;
        }

        internal static BsonDocument CardAdd()
        {
            Console.WriteLine("\nName of card: ");
            string cardName = Console.ReadLine()!.Trim();
            cardName = FirstCharUpper(CheckEmptyString(cardName));

            Console.WriteLine("\nImage location: ");
            string image = Console.ReadLine()!.Trim();
            CheckEmptyString(image);

            Console.WriteLine("\nWhat is the card's type?");
            string type = Console.ReadLine()!.Trim();
            type = FirstCharUpper(CheckEmptyString(type));

            Console.WriteLine("\nWhat is the card's cost?");
            int cost = GetValidCost();

            Console.WriteLine("\nWhat is the card's special icons?\nEnter with , please.\n");
            string icons = Console.ReadLine()!.Trim();
            string[] iconsArray = ParseAndFormatInputArray(icons);

            Console.WriteLine("\nWhat is the card's elements?\nEnter with , please.\n");
            string elements = Console.ReadLine()!.Trim();
            string[] elementsArray = ParseAndFormatInputArray(elements);

            Console.WriteLine("\nWhat is the card's code?");
            string code = GetValidCardCode();

            Console.WriteLine("How many copies are there?");
            int copies = GetValidCopies();

            Console.WriteLine("\nIs this card a foil?\nEnter 'y' for yes and 'n' for no.");
            bool isFoil = GetIsFoil();

            var newCard = new Card(cardName, image, type, cost, iconsArray, elementsArray, code, copies, isFoil);

            var newDocument = new BsonDocument
            {
                { "Card_name", newCard.Name },
                { "Image_location", newCard.Image},
                { "Type", newCard.Type},
                { "Cost", newCard.Cost},
                { "Special_icons", new BsonArray(newCard.SpecialIcons)},
                { "Elements", new BsonArray(newCard.Elements)},
                { "Card_code", newCard.Code},
                { "Copies", newCard.Copies},
                { "Foil?", newCard.IsFoil}
            };

            return newDocument;
        }

        internal static void CardFind(IMongoCollection<Card> cardCollection)
        {
            Console.WriteLine("\nWould you like to find by code or name? c = code, n = name");
            char input = Console.ReadLine()!.Trim().ToLower()[0];
            while (input != 'c' && input != 'n')
            {
                Console.WriteLine("Please enter 'c' or 'n'");
                input = Console.ReadLine()!.Trim().ToLower()[0];
            }

            if (input == 'c')
            {
                Console.WriteLine("Enter code of the card\n");
                string code = GetValidCardCode();
                var filter = Builders<Card>.Filter.Eq(card => card.Code, code);
                var searchResult = cardCollection.Find(filter).ToList();
                DisplaySearchResults(searchResult);
            }
            else
            {
                Console.WriteLine("Enter the name of the card\n");
                string name = GetValidName();
                var filter = Builders<Card>.Filter.Eq(card => card.Name, name);
                var searchResult = cardCollection.Find(filter).ToList();
                DisplaySearchResults(searchResult);
            }
        }

        private static void DisplaySearchResults(List<Card> searchResult)
        {
            // Find any document in database that matches the filter.
            if (searchResult.Any())
            {
                foreach (var cardResult in searchResult)
                {
                    Console.WriteLine(cardResult.ToString());
                }
            }
            else
            {
                Console.WriteLine("No matching cards found.");
            }
        }

        private static string CheckEmptyString(string input)
        {
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("\nNo input detected. Please re-enter.");
                input = Console.ReadLine()!.Trim();
            }
            return input;
        }
        private static string FirstCharUpper(string input)
        {
            return string.Join(" ", input.Split(new[] { ' ', '\'' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => $"{char.ToUpper(s[0])}{s[1..]}"));
        }

        private static string[] FirstCharUpper(string[] input)
        {
            return input.Select(FirstCharUpper).ToArray();
        }

        private static bool CardRegex(string regex)
        {
            // Regex for regular and promo card codes
            string cardCodeRegex = @"^\d{1,2}-\d{3}[CRHLS]+$";
            string promoCodeRegex = @"^PR-\d{3}";
            Regex normalRegex = new(cardCodeRegex);
            Regex promoRegex = new(promoCodeRegex);
            return normalRegex.IsMatch(regex) || promoRegex.IsMatch(regex);
        }
        private static string GetValidCardCode()
        {
            string code = Console.ReadLine()!.Trim().ToUpper();
            return ValidateAndFormatCardCode(code);
        }
        private static string GetValidName()
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
        private static string ValidateAndFormatCardCode(string code)
        {
            code = code.Trim().ToUpper();
            while (!CardRegex(code))
            {
                Console.WriteLine("Invalid card code. Please re-enter.");
                code = Console.ReadLine()!.Trim().ToUpper();
            }
            return code;
        }

        private static int GetValidCost()
        {
            int cost;
            string input;
            input = Console.ReadLine()!.Trim();
            while (!int.TryParse(input, out cost) || cost < 1 || cost > 11)
            {
                Console.WriteLine("\nInvalid cost or you inputted a number lower than 1 and higher than 11.\nThere is currently no higher cost than 11.\nPlease input a number between the range.");
                input = Console.ReadLine()!.Trim();
            }
            return cost;
        }
        private static int GetValidCopies()
        {
            int copies;
            string input;
            input = Console.ReadLine()!.Trim();
            while (!int.TryParse(input, out copies) || copies == 0)
            {
                Console.WriteLine("\nInvalid number or 0 was inputted.\nThere shouldn't be 0 copies of a card in the collection.\nPlease enter a valid number.");
                input = Console.ReadLine()!.Trim();
            }
            return copies;
        }

        private static string[] ParseAndFormatInputArray(string input)
        {
            // Splits the array by commas and then capatalizes the words
            string[] inputArray = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return FirstCharUpper(inputArray);
        }

        private static bool GetIsFoil()
        {
            char foilValid = char.ToLower(Console.ReadLine()!.Trim()[0]);
            while (foilValid != 'y' && foilValid != 'n')
            {
                Console.WriteLine("Invalid. Please enter either 'y' or 'n'");
                foilValid = char.ToLower(Console.ReadLine()!.Trim()[0]);
            }
            return foilValid == 'y';
        }

        public override string ToString()
        {
            // Return result as a string
            string foilStatus = IsFoil ? "they're foil." : "they are not foil.";
            return $"There are {Copies} copies of {Name}({Code}), {foilStatus}";
        }
    }
}

