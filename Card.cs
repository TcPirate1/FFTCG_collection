using MongoDB.Bson;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using Sprache;

namespace FFTCG_collection
{
    internal class Card
    {
        [BsonId]
        private ObjectId Id { get; set; }

        [BsonElement("Card_name")]
        private string Name { get; }

        [BsonElement("Image_location")]
        private string Image { get; }

        [BsonElement("Type")]
        private string Type { get; }

        [BsonElement("Cost")]
        private int Cost { get; }

        [BsonElement("Special_icons")]
        private string[] SpecialIcons { get; }

        [BsonElement("Elements")]
        private string[] Elements { get; }

        [BsonElement("Card_code")]
        private string Code { get; }

        [BsonElement("Copies")]
        private int Copies { get; }

        [BsonElement("Foil?")]
        private bool IsFoil { get; }

        public Card(string name, string image, string type, int cost, IEnumerable<string> specialIcons, IEnumerable<string> elements, string code, int copies, bool isFoil)
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

        internal static void CardDelete(IMongoCollection<Card> cardCollection)
        {
            Console.WriteLine("Enter the card's code you want to DELETE\n");
            string code = GetValidCardCode();
            var filter = Builders<Card>.Filter.Eq(card => card.Code, code);
            var searchResult = cardCollection.Find(filter).ToList();
            if (DeleteCardConfirmation(searchResult) == 'y')
            {
                cardCollection.DeleteOne(filter);
                Console.WriteLine("Card deleted.");
            }
            else
            {
                Console.WriteLine("No cards deleted.");
            }
        }

        internal static void CardUpdate(IMongoCollection<Card> cardCollection)
        {
            Console.WriteLine("Enter the card's code you want to UPDATE\n");
            string code = GetValidCardCode();
            var filter = Builders<Card>.Filter.Eq(card => card.Code, code);
            var searchResult = cardCollection.Find(filter).ToList();
            if (searchResult.Any())
            {
                UpdateCard(cardCollection, filter, code);
            }
            else
            {
                Console.WriteLine("No matching cards found.");
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

        private static char DeleteCardConfirmation(List<Card> searchResult)
        {
            if (searchResult.Any())
            {
                foreach(var card in searchResult)
                {
                    Console.WriteLine($"You are about to delete {card.Name}({card.Code}).\nAre you sure? y = yes, n = no");
                    char input = Console.ReadLine()!.Trim().ToLower()[0];
                    while (input != 'y' && input != 'n')
                    {
                        Console.WriteLine("Please enter 'y' or 'n'");
                        input = Console.ReadLine()!.Trim().ToLower()[0];
                    }
                    return input;
                }
            }
            else
            {
                Console.WriteLine("No matching cards found.");
            }
            return 'n';
        }

        private static void UpdateCard(IMongoCollection<Card> cardCollection, FilterDefinition<Card> filter, string code)
        {
            Console.WriteLine("Choose the field you want to update from the list below:\n");
            Console.WriteLine("1. Name");
            Console.WriteLine("2. Image location");
            Console.WriteLine("3. Type");
            Console.WriteLine("4. Cost");
            Console.WriteLine("5. Special icons");
            Console.WriteLine("6. Elements");
            Console.WriteLine("7. Code");
            Console.WriteLine("8. Copies");
            Console.WriteLine("9. Foil status");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                string fieldToUpdate;
                string newValue;

                switch (choice)
                {
                    case 1:
                        fieldToUpdate = "Name";
                        Console.WriteLine("Enter the new card name:");
                        newValue = FirstCharUpper(Console.ReadLine()!.Trim());
                        break;
                    case 2:
                        fieldToUpdate = "Image";
                        Console.WriteLine("Enter the new URL for the card:");
                        newValue = Console.ReadLine()!.Trim();
                        break;
                    case 3:
                        fieldToUpdate = "Type";
                        Console.WriteLine("Enter the new type for the card:");
                        newValue = FirstCharUpper(Console.ReadLine()!.Trim());
                        break;
                    case 4:
                        fieldToUpdate = "Cost";
                        Console.WriteLine("Enter the new cost for the card:");
                        newValue = Console.ReadLine()!.Trim();
                        break;
                    case 5:
                        fieldToUpdate = "Special_icons";
                        Console.WriteLine("Enter the new Special Icons for the card:");
                        newValue = Console.ReadLine()!.Trim();
                        break;
                    case 6:
                        fieldToUpdate = "Elements";
                        Console.WriteLine("Enter the new Elements for the card:");
                        newValue = Console.ReadLine()!.Trim();
                        break;
                    case 7:
                        fieldToUpdate = "Card_code";
                        Console.WriteLine("Enter the new Code for the card:");
                        newValue = Console.ReadLine()!.Trim();
                        break;
                    case 8:
                        fieldToUpdate = "Copies";
                        Console.WriteLine("Enter the new value for number of Copies of this card:");
                        newValue = Console.ReadLine()!.Trim();
                        break;
                    case 9:
                        fieldToUpdate = "Foil?";
                        Console.WriteLine("Enter the new Foil Status of this card:");
                        newValue = Console.ReadLine()!.Trim();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. No field updated.");
                        return;
                }

                // Create an update definition to specify the field to update and the new value.
                var update = Builders<Card>.Update.Set(fieldToUpdate, newValue);

                // Use the UpdateMany method to update all matching documents in the collection.
                var updateResult = cardCollection.UpdateMany(filter, update);

                Console.WriteLine(updateResult.ModifiedCount <= 0
                    ? "No documents were updated."
                    : $"Updated the '{fieldToUpdate}' field for {updateResult.ModifiedCount} cards with code {code}.");
            }
            else
            {
                Console.WriteLine("Invalid choice. No field updated.");
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
            // Regex to match input to start of a string and whenever there is a space, single quote or hyphen and replace the character with the upper case version.
            const string regex = @"\b[a-zA-Z]|(?<=[ '-])[a-zA-Z]";
            Regex capitalizeRegex = new(regex);
            return capitalizeRegex.Replace(input, m => m.Value.ToUpper());
        }

        private static string[] FirstCharUpper(IEnumerable<string> input)
        {
            return input.Select(FirstCharUpper).ToArray();
        }

        private static bool CardRegex(string regex)
        {
            // Regex for regular and promo card codes
            const string cardCodeRegex = @"^\d{1,2}-\d{3}[CRHLS]+$";
            const string promoCodeRegex = @"^PR-\d{3}";
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
            string input = Console.ReadLine()!.Trim();
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
            string input = Console.ReadLine()!.Trim();
            while (!int.TryParse(input, out copies) || copies == 0)
            {
                Console.WriteLine("\nInvalid number or 0 was inputted.\nThere shouldn't be 0 copies of a card in the collection.\nPlease enter a valid number.");
                input = Console.ReadLine()!.Trim();
            }
            return copies;
        }

        private static string[] ParseAndFormatInputArray(string input)
        {
            // Splits the array by commas and then capitalizes the words
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

