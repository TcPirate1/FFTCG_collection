// See https://aka.ms/new-console-template for more information

using MongoDB.Driver;
using MongoDB.Bson;
using DotNetEnv;

string projectRoot = GetProjectRoot();
Environment.CurrentDirectory = projectRoot;

Env.Load();

string dbCluster = Environment.GetEnvironmentVariable("ATLAS_URI");

MongoClient client = new(dbCluster);

List<string> databases = client.ListDatabaseNames().ToList();

foreach (string database in databases)
{
    Console.WriteLine(database);
}

static string GetProjectRoot()
{
    string currentDirectory = Directory.GetCurrentDirectory();
    DirectoryInfo directory = new(currentDirectory);

    // Traverse up the directory tree until the solution file (e.g., .sln) is found
    while (directory != null && !directory.GetFiles("*.sln").Any())
    {
        directory = directory.Parent;
    }

    return directory?.FullName;
}