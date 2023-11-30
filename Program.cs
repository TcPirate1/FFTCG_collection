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
    Console.WriteLine("\nWelcome to the FFTCG collection app.\n(Please enter one of the following numbers to select the respective menu item)");
    Console.WriteLine("0. Exit program.");
    Console.WriteLine("1. Add card to database.");
    Console.WriteLine("2. Search for card in database.");
    Console.WriteLine("3. Remove card from database.");
    Console.WriteLine("4. Update card in database.");
    try
    {
        int option = Convert.ToInt32(Console.ReadLine()?.Trim());
        switch (option)
        {
            case 0:
                repeat = false;
                break;
            case 1:
                if (!Card.CardAdd(cardCollection))
                {
                    Console.WriteLine("Returning to main menu.");
                }
                else
                {
                    Console.WriteLine("Card added successfully!");
                }
                repeat = true;
                break;
            case 2:
                Card.CardFind(cardSearch);
                repeat = true;
                break;
            case 3:
                Card.CardDelete(cardSearch);
                repeat = true;
                break;
            case 4:
                Card.CardUpdate(cardSearch);
                repeat = true;
                break;
            default:
                Console.WriteLine("\nPlease enter one of the above numbers.\n");
                repeat = true;
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

// Following URL explains why Resharper suggests a solo return statement here
// https://blog.jetbrains.com/dotnet/2023/05/22/resharper-2023-2-eap-2/
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