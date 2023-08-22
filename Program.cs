// See https://aka.ms/new-console-template for more information

using MongoDB.Driver;
using MongoDB.Bson;
using DotNetEnv;
using FFTCG_collection;

string projectRoot = GetProjectRoot()!;
Environment.CurrentDirectory = projectRoot;

Env.Load();

string dbCluster = Environment.GetEnvironmentVariable("ATLAS_URI")!;

MongoClient client = new(dbCluster);

var cardCollection = client.GetDatabase("FFCollection").GetCollection<BsonDocument>("cards");

bool repeat;
do
{
    Console.WriteLine("Welcome to the FFTCG collection app. How may I help you today? (Please enter the following number to select the respective menu item)");
    Console.WriteLine("1. Add card to database");
    try
    {
        int invalid = Convert.ToInt32(Console.ReadLine()?.Trim());
        if (invalid >= 0 && invalid <= 5)
        {
            repeat = false;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter the respective number in menu\n");
            repeat = true;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"\n{e.Message}\nPlease enter valid input.");
        repeat = true;
    }

    //cardCollection.InsertOne(Card.CardAdd());
    //Console.WriteLine($"Adding card to collection...");

    //Console.WriteLine("\nDo you want to add another card? (type y for yes and n for no)");
    //char valid = Convert.ToChar(Console.ReadLine()!.Trim().ToLower());
    //if (valid != 'y' && valid != 'n')
    //{
    //    Console.WriteLine("Invalid, please type y or n.");
    //    repeat = true;
    //}
    //else if (valid == 'n')
    //{
    //    repeat = false;
    //}
    //else
    //{
    //    repeat = true;
    //}
}
while (repeat == true);

static string? GetProjectRoot()
{
    string currentDirectory = Directory.GetCurrentDirectory();
    DirectoryInfo directory = new(currentDirectory);

    // Traverse up the directory tree (from /bin/Debug/net6.0) until the solution file (e.g., .sln) is found
    while (directory != null && !directory.GetFiles("*.sln").Any())
    {
        directory = directory.Parent!;
    }

    return directory?.FullName;
}