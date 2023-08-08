using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

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
            Console.WriteLine($"\nAdding card {count}");
            Console.WriteLine("Name of card: ");
            string cardname1 = Console.ReadLine();
            Console.WriteLine("Image location: ");
            string image1 = Console.ReadLine();
            Console.WriteLine("What is the card's type?");
            string type1 = Console.ReadLine();
            Console.WriteLine($"What is the card's cost?");
            double cost1 = Convert.ToDouble(Console.ReadLine());
            // Arrays, how to do this?
        }
    }
}
