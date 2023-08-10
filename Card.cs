using System;
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
        private string Name { get; set; } = null!;
        private string Image { get; set; } = null!;
        private string Type { get; set; } = null!;
        private double Cost { get; set; }
        private string[] Special_icons { get; set; } = null!;
        private string[] Elements { get; set; } = null!;
        private string Code { get; set; } = null!;
        private bool Foil;

        public Card(string name, string image, string type, double cost, string[] special_icons, string[] elements, string code, bool foil)
        {
            Name = name;
            Image = image;
            Type = type;
            Cost = cost;
            Special_icons = special_icons;
            Elements = elements;
            Code = code;
            Foil = foil;
        }

        public static void CardAdd(int count)
        {
            //Card newCard = new();
            Console.WriteLine($"\nAdding card {count}");
            Console.WriteLine("Name of card: ");
            string cardname1 = Console.ReadLine().Trim();
            Console.WriteLine("Image location: ");
            string image1 = Console.ReadLine().Trim();
            Console.WriteLine("What is the card's type?");
            string type1 = Console.ReadLine().Trim();
            Console.WriteLine("What is the card's cost?");
            double cost1 = Convert.ToDouble(Console.ReadLine().Trim());
            Console.WriteLine("What is the card's special icons?\nEnter with spaces please.\n");
            string icons1 = Console.ReadLine().Trim();
            string[] iconsArray1 = icons1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string icon in iconsArray1)
            {
                Console.WriteLine($"{icon}");
            }
            Console.WriteLine("What is the card's elements?\nEnter with spaces please.\n");
            string elements1 = Console.ReadLine().Trim();
            string[] elementsArray1 = elements1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Console.WriteLine("What is the card's code?");
            string code1 = Console.ReadLine().Trim();
            while (CardRegex(code1) != true)
            {
                Console.WriteLine("Please enter a valid card code.");
                code1 = Console.ReadLine();
            }
            Console.WriteLine("Is this card a foil?\nEnter \'y\' for yes and \'n\' for no.");
            char foil = Convert.ToChar(Console.ReadLine().Trim().ToLower());
            while (foil != 'y' && foil != 'n')
            {
                Console.WriteLine("Invalid. Please enter either \'y\' or \'n\'");
                foil = Convert.ToChar(Console.ReadLine().Trim().ToLower());
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
    }
}
