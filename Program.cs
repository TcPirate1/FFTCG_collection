﻿// See https://aka.ms/new-console-template for more information
using MongoDB.Driver;
using MongoDB.Bson;
using FFTCG_collection;
using CommandLine;
using System.Globalization;

Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
    if (o.Add)
    {
        Card.CardAdd(AddCardCheckConnection());
    }
    else if (o.Search)
    {
        Card.CardFind(CheckConnection());
    }
    else if (o.Delete)
    {
        Card.CardDelete(CheckConnection());
    }
    else if (o.Update)
    {
        Card.CardUpdate(CheckConnection());
    }
    else
    {
        Console.WriteLine("Please enter a valid option.");
    }
});

static IMongoCollection<BsonDocument> AddCardCheckConnection()
{
    while (PasswordAndUsername.HasPassword == false && PasswordAndUsername.HasUsername == false)
        {
            MongoClient client;
            string connectionString;
            Console.WriteLine("Please enter your MongoDB Atlas username and password to connect to the DB.");
            Console.WriteLine("Username:");
            string username = Console.ReadLine()!;
            Console.WriteLine("Password:");
            string password = Console.ReadLine()!;
            PasswordAndUsername.HasUsername = true;
            PasswordAndUsername.HasPassword = true;
            try
            {
                connectionString = $"mongodb+srv://{username}:{password}@cluster0.pp95tii.mongodb.net/"!;
                client = new(connectionString);
                client.GetDatabase("FFCollection").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Successfully connected to MongoDB Atlas.");
                return client.GetDatabase("FFCollection").GetCollection<BsonDocument>("cards");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\nPlease enter a valid username and password.");
                PasswordAndUsername.HasUsername = false;
                PasswordAndUsername.HasPassword = false;
            }
        }
        return null!;
}
static IMongoCollection<Card> CheckConnection()
{
    while (PasswordAndUsername.HasPassword == false && PasswordAndUsername.HasUsername == false)
        {
            MongoClient client;
            string connectionString;
            Console.WriteLine("Please enter your MongoDB Atlas username and password to connect to the DB.");
            Console.WriteLine("Username:");
            string username = Console.ReadLine()!;
            Console.WriteLine("Password:");
            string password = Console.ReadLine()!;
            PasswordAndUsername.HasUsername = true;
            PasswordAndUsername.HasPassword = true;
            try
            {
                connectionString = $"mongodb+srv://{username}:{password}@cluster0.pp95tii.mongodb.net/"!;
                client = new(connectionString);
                client.GetDatabase("FFCollection").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Successfully connected to MongoDB Atlas.");
                return client.GetDatabase("FFCollection").GetCollection<Card>("cards");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\nPlease enter a valid username and password.");
                PasswordAndUsername.HasUsername = false;
                PasswordAndUsername.HasPassword = false;
            }
        }
        return null!;
}
// Adding a document and finding documents use different interfaces (refer to this SO answer: https://stackoverflow.com/questions/67341056/mongodb-filterdefinition-and-interfaces-in-c-sharp)

public static class PasswordAndUsername
{
    public static bool? HasUsername { get; set; } = false;
    public static bool? HasPassword { get; set; } = false;
}

public class Options
{
    [Option('a', "add", Required = false, HelpText = "Add a card to the database.")]
    public bool Add { get; set; }
    
    [Option('s', "search", Required = false, HelpText = "Search for a card in the database.")]
    public bool Search { get; set; }
    [Option('d', "delete", Required = false, HelpText = "Delete a card from the database.")]
    public bool Delete { get; set; }
    [Option('u', "update", Required = false, HelpText = "Update a card in the database.")]
    public bool Update { get; set; }
}


// string projectRoot = GetProjectRoot()!;
// Environment.CurrentDirectory = projectRoot;

// Console.WriteLine("Please enter your MongoDB Atlas username and password to connect to the DB.");
// Console.WriteLine("Username:");
// string username = Console.ReadLine()!;
// Console.WriteLine("Password:");
// string password = Console.ReadLine()!;

// string dbCluster = $"mongodb+srv://{username}:{password}@cluster0.pp95tii.mongodb.net/"!;

// MongoClient client = new(dbCluster);

// var cardCollection = client.GetDatabase("FFCollection").GetCollection<BsonDocument>("cards");
// // Works for adding new document

// var cardSearch = client.GetDatabase("FFCollection").GetCollection<Card>("cards");
// // Works for searching through a document (refer to this SO answer: https://stackoverflow.com/questions/67341056/mongodb-filterdefinition-and-interfaces-in-c-sharp)

// try {
//     client.GetDatabase("FFCollection").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
//     Console.WriteLine("Connected to MongoDB Atlas.");
// }
// catch (Exception e)
// {
//     Console.WriteLine(e.Message);
// }

// bool repeat;
// do
// {
//     Console.WriteLine("\nWelcome to the FFTCG collection app.\n(Please enter one of the following numbers to select the respective menu item)");
//     Console.WriteLine("0. Exit program.");
//     Console.WriteLine("1. Add card to database.");
//     Console.WriteLine("2. Search for card in database.");
//     Console.WriteLine("3. Remove card from database.");
//     Console.WriteLine("4. Update card in database.");
//     try
//     {
//         int option = Convert.ToInt32(Console.ReadLine()?.Trim());
//         switch (option)
//         {
//             case 0:
//                 repeat = false;
//                 break;
//             case 1:
//                 if (!Card.CardAdd(cardCollection))
//                 {
//                     Console.WriteLine("Returning to main menu.");
//                 }
//                 else
//                 {
//                     Console.WriteLine("Card added successfully!");
//                 }
//                 repeat = true;
//                 break;
//             case 2:
//                 Card.CardFind(cardSearch);
//                 repeat = true;
//                 break;
//             case 3:
//                 Card.CardDelete(cardSearch);
//                 repeat = true;
//                 break;
//             case 4:
//                 Card.CardUpdate(cardSearch);
//                 repeat = true;
//                 break;
//             default:
//                 Console.WriteLine("\nPlease enter one of the above numbers.\n");
//                 repeat = true;
//                 break;
//         }
//     }
//     catch (Exception e)
//     {
//         Console.WriteLine($"\n{e.Message}\nPlease enter valid input.\n");
//         repeat = true;
//     }
// }
// while (repeat == true);

// // Following URL explains why Resharper suggests a solo return statement here
// // https://blog.jetbrains.com/dotnet/2023/05/22/resharper-2023-2-eap-2/
// static string? GetProjectRoot()
// {
//     string currentDirectory = Directory.GetCurrentDirectory();
//     DirectoryInfo directory = new(currentDirectory);

//     // Traverse up the directory tree (from /bin/Debug/net<version#>) until the solution file (e.g., .sln) is found
//     while (directory != null && directory.GetFiles("*.sln").Length == 0)
//     {
//         directory = directory.Parent!;
//     }

//     return directory?.FullName;
// }