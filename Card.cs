﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using MongoDB.Driver;

namespace FFTCG_collection
{
    internal class Card
    {
        public ObjectId _id { get; set; }
        private string Name { get; set; }
        private string Image { get; set; }
        private string Type { get; set; }
        private int Cost { get; set; }
        private string[] Special_icons { get; set; }
        private string[] Elements { get; set; }
        private string Code { get; set; }
        private int Copies { get; set; }
        private bool Foil { get; set; }

        public Card(string name, string image, string type, int cost, string[] special_icons, string[] elements, string code, int copies, bool foil)
        {
            Name = name;
            Image = image;
            Type = type;
            Cost = cost;
            Special_icons = special_icons;
            Elements = elements;
            Code = code;
            Copies = copies;
            Foil = foil;
        }
        
        // Method for adding cards to MongoDB
        public static BsonDocument CardAdd()
        {
            string errorMsg = "You might of mistakenly typed a non-alphanumeric character that isn't ' .\nPlease re-enter.";
            string icons;
            string[] iconsArray;
            string elements;
            string[] elementsArray;
            string input;
            int cost;

            Console.WriteLine("\nName of card: ");
            string cardname = Console.ReadLine()!.Trim();
            CheckEmptyString(cardname);
            FirstCharUpper(cardname);

            Console.WriteLine("\nImage location: ");
            string image = Console.ReadLine()!.Trim();
            CheckEmptyString(image);

            Console.WriteLine("\nWhat is the card's type?");
            string type = Console.ReadLine()!.Trim();
            CheckEmptyString(type);
            FirstCharUpper(type);

            Console.WriteLine("\nWhat is the card's cost?");
            input = Console.ReadLine()!.Trim();
            while (int.TryParse(input, out cost) != true || cost < 1 || cost > 11)
            {
                Console.WriteLine("\nInvalid cost or you inputted a number lower than 1 and higher than 11.\nThere is currently no higher cost than 11.\nPlease input a number.");
                input = Console.ReadLine()!.Trim();
            }

            //Ask user for special icons
            do
            {
                Console.WriteLine("\nWhat is the card's special icons?\nEnter with , please.\n");
                icons = Console.ReadLine()!.Trim();
                iconsArray = icons.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                FirstCharUpper(iconsArray);

                if (AccidentalCharacterCheck(iconsArray))
                {
                    Console.WriteLine(errorMsg);
                }
            }
            while (AccidentalCharacterCheck(iconsArray));

            //Ask user for elements
            do
            {
                Console.WriteLine("\nWhat is the card's elements?\nEnter with spaces please.\n");
                elements = Console.ReadLine()!.Trim();
                elementsArray = elements.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                FirstCharUpper(elementsArray);

                if (AccidentalCharacterCheck(elementsArray))
                {
                    Console.WriteLine(errorMsg);
                }
            }
            while (AccidentalCharacterCheck(elementsArray));

            Console.WriteLine("\nWhat is the card's code?");
            string code = Console.ReadLine()!.Trim().ToUpper();
            while (CardRegex(code) != true)
            {
                Console.WriteLine("\nWhat you entered did not match the pattern. Please enter a valid card code.");
                code = Console.ReadLine()!.Trim().ToUpper();
            }

            Console.WriteLine("How many copies are there?");
            int copies;
            input = Console.ReadLine()!.Trim();
            while (int.TryParse(input, out copies) != true || copies == 0)
            {
                Console.WriteLine("\nInvalid number or 0 was inputted.\nThere shouldn't be 0 copies of a card in the collection.\nPlease enter a valid number.");
                input = Console.ReadLine()!.Trim();
            }

            Console.WriteLine("\nIs this card a foil?\nEnter \'y\' for yes and \'n\' for no.");
            char foilValid = Convert.ToChar(Console.ReadLine()!.Trim().ToLower());
            while (foilValid != 'y' && foilValid != 'n')
            {
                Console.WriteLine("Invalid. Please enter either \'y\' or \'n\'");
                foilValid = Convert.ToChar(Console.ReadLine()!.Trim().ToLower());
            }
            bool foil = foilValid == 'y' ? true : false;

            Card newCard = new(cardname, image, type, cost, iconsArray, elementsArray, code, copies, foil);

            var newDocument = new BsonDocument
            {
                { "Card Name", newCard.Name },
                { "Image Location", newCard.Image},
                { "Type", newCard.Type},
                { "Cost", newCard.Cost},
                { "Special Icons", new BsonArray(newCard.Special_icons)},
                { "Elements", new BsonArray(newCard.Elements)},
                { "Card code", newCard.Code},
                { "Copies", newCard.Copies},
                { "Foil?", newCard.Foil}

            };

            return newDocument;
        }
        // Method to search for card in MongoDB
        internal static void CardFind(IMongoCollection<Card> card)
        {
            Console.WriteLine("\nWould you like to find by code or name? c = code, n = name");
            char input = Convert.ToChar(Console.ReadLine()!.ToLower());
            while (input != 'c' && input != 'n')
            {
                Console.WriteLine("Please enter \"c\" or \"n\"");
                input = Convert.ToChar(Console.ReadLine()!.ToLower());
            }
            if (input == 'c')
            {
                Console.WriteLine("Enter code of the card\n");
                string code = Console.ReadLine()!.Trim().ToUpper();
                while (CardRegex(code) != true)
                {
                    Console.WriteLine("Invalid card code. Please re-enter.");
                    code = Console.ReadLine()!.Trim().ToUpper();
                }
                var filter = Builders<Card>.Filter.Eq(card => card.Code, code);
                var searchResult = card.Find(filter).ToList();
                foreach (var cardResult in searchResult)
                {
                    Console.WriteLine(cardResult);
                }
            }
            if (input == 'n')
            {
                Console.WriteLine("Enter the name of the card\n");
                string name = Console.ReadLine()!.Trim().ToUpper();
                while (CardRegex(name) == true)
                {
                    Console.WriteLine("Invalid name. Please re-enter");
                    name = Console.ReadLine()!.Trim().ToUpper();
                }
                string notUppercase = FirstCharUpper(name.ToLower());
                var filter = Builders<Card>.Filter.Eq(card => card.Name, notUppercase);
                var searchResult = card.Find(filter).ToList();
                foreach (var cardResult in searchResult)
                {
                    Console.WriteLine(cardResult);
                }
            }
        }
        private static bool CardRegex(string regex)
        {
            string cardCodeRegex = @"^\d{1,2}-\d{3}[CRHLS]+$";
            Regex checkRegex = new(cardCodeRegex);
            if (checkRegex.IsMatch(regex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool AccidentalCharacterCheck(string[] iconsAndElements)
        {
            Console.WriteLine(iconsAndElements.Length);
            string spaceRegex = @"^[a-zA-Z0-9',]+$"; //Some card names uses ' and commas are the delimiter.

            return iconsAndElements.Any(iconsAndElement => Regex.IsMatch(iconsAndElement, spaceRegex));
            // Check every element in the array
        }
        private static string CheckEmptyString(string input)
        {
            while (String.IsNullOrEmpty(input))
            {
                Console.WriteLine("\nNo input detected. Please re-enter.");
                input = Console.ReadLine()!.Trim();
            }
            return input;
        }
        private static string FirstCharUpper(string input)
        {
            return $"{char.ToUpper(input[0])}{input[1..]}";
        }
        private static string[] FirstCharUpper(string[] input)
        {
            string[] result = new string[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                result[i] = FirstCharUpper(input[i]);
            }
            return result;
        }
    }
}
