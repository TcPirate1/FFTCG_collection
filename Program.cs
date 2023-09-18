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
// Works for adding new document

var cardSearch = client.GetDatabase("FFCollection").GetCollection<Card>("cards");
// Works for searching through a document (refer to this SO answer: https://stackoverflow.com/questions/67341056/mongodb-filterdefinition-and-interfaces-in-c-sharp)

bool repeat;
do
{
    Console.WriteLine("\nWelcome to the FFTCG collection app. How may I help you today? (Please enter the following number to select the respective menu item)");
    Console.WriteLine("0. Exit program.");
    Console.WriteLine("1. Add card to database.");
    Console.WriteLine("2. Search for card in database.");
    try
    {
        int option = Convert.ToInt32(Console.ReadLine()?.Trim());
        switch (option)
        {
            case 0:
                repeat = false;
                break;
            case 1:
                cardCollection.InsertOne(Card.CardAdd());
                Console.WriteLine("\nAdding card to collection...");
                Console.WriteLine("\nCard added, returning to menu.");
                repeat = true;
                break;
            case 2:
                Card.CardFind(cardSearch);
                repeat = true;
                break;
            default:
                repeat = false;
                break;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"\n{e.Message}\nPlease enter valid input.\n");
        repeat = true;
    }
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