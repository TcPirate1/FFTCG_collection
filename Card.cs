﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Text.RegularExpressions;

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

        public static BsonDocument CardAdd()
        {
            string input;
            Console.WriteLine("Name of card: ");
            string cardname = Console.ReadLine()!.Trim();
            Console.WriteLine("Image location: ");
            string image = Console.ReadLine()!.Trim();
            Console.WriteLine("What is the card's type?");
            string type = Console.ReadLine()!.Trim();
            Console.WriteLine("What is the card's cost?");
            int cost;
            input = Console.ReadLine()!.Trim();
            while (int.TryParse(input, out cost) != true || cost < 1 || cost > 11)
            {
                Console.WriteLine("Invalid cost or you inputted a number lower than 1 and higher than 11.\nThere is currently no higher cost than 11.\n Please input a number.");
            }
            Console.WriteLine("What is the card's special icons?\nEnter with spaces please.\n");
            string icons = Console.ReadLine()!.Trim();
            string[] iconsArray = icons.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Console.WriteLine("What is the card's elements?\nEnter with spaces please.\n");
            string elements = Console.ReadLine()!.Trim();
            string[] elementsArray = elements.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Console.WriteLine("What is the card's code?");
            string code = Console.ReadLine()!.Trim().ToUpper();
            while (CardRegex(code) != true)
            {
                Console.WriteLine("Please enter a valid card code.");
                code = Console.ReadLine()!.Trim().ToUpper();
            }
            Console.WriteLine("How many copies are there?");
            int copies;
            input = Console.ReadLine()!.Trim();
            while (int.TryParse(input, out copies) != true || copies == 0)
            {
                Console.WriteLine("Invalid number or 0 was inputted.\nThere shouldn't be 0 copies of a card in the collection.\nPlease enter a valid number.");
                input = Console.ReadLine()!.Trim();
            }
            Console.WriteLine("Is this card a foil?\nEnter \'y\' for yes and \'n\' for no.");
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
    }
}
