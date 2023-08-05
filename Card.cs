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
    }
}
