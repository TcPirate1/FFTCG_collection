// See https://aka.ms/new-console-template for more information

using MongoDB.Driver;
using MongoDB.Bson;
using DotNetEnv;
using FFTCG_collection;

string projectRoot = GetProjectRoot();
Environment.CurrentDirectory = projectRoot;

Env.Load();

string dbCluster = Environment.GetEnvironmentVariable("ATLAS_URI");

MongoClient client = new(dbCluster);

//List<string> databases = client.ListDatabaseNames().ToList();

//foreach (string database in databases)
//{
//    Console.WriteLine(database);
//}

//List<string> collections = client.GetDatabase("FFCollection").ListCollectionNames().ToList();

//foreach (string collection in collections)
//{
//    Console.WriteLine(collection);
//}

//var cardPlaylist = client.GetDatabase("FFCollection").GetCollection<Card>("cards");
bool invalid;
do
{
    Console.WriteLine("How many cards would you like to add?");
    var isNumber = int.TryParse(Console.ReadLine(), out int count);
    if (isNumber == true)
    {
        invalid = false;
        int i = 1;
        while (i <= count)
        {
            Card.CardAdd(i);
            i++;
        }
    }
    else
    {
        invalid = true;
        Console.WriteLine("Invalid, please enter a number of cards to add.\n");
    }
}
while (invalid == true);

static string GetProjectRoot()
{
    string currentDirectory = Directory.GetCurrentDirectory();
    DirectoryInfo directory = new(currentDirectory);

    // Traverse up the directory tree (from /bin/Debug/net6.0) until the solution file (e.g., .sln) is found
    while (directory != null && !directory.GetFiles("*.sln").Any())
    {
        directory = directory.Parent;
    }

    return directory?.FullName;
}