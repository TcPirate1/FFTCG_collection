using Xunit;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.IO;
using System.Linq;

namespace FFTCG_collection.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void TestGetProjectRoot()
        {
            // Arrange
            string expectedRoot = "/home/tc/Desktop/Projects/Personal/FFTCG_collection";

            // Act
            string actualRoot = Program.GetProjectRoot();

            // Assert
            Assert.Equal(expectedRoot, actualRoot);
        }

        [Fact]
        public void TestCardAdd()
        {
            // Arrange
            var client = new MongoClient("mongodb://localhost:27017");
            var cardCollection = client.GetDatabase("FFCollection").GetCollection<BsonDocument>("cards");
            var initialCount = cardCollection.CountDocuments(FilterDefinition<BsonDocument>.Empty);

            // Act
            Card.CardAdd();

            // Assert
            var finalCount = cardCollection.CountDocuments(FilterDefinition<BsonDocument>.Empty);
            Assert.Equal(initialCount + 1, finalCount);
        }

        [Fact]
        public void TestCardFind()
        {
            // Arrange
            var client = new MongoClient("mongodb://localhost:27017");
            var cardCollection = client.GetDatabase("FFCollection").GetCollection<Card>("cards");
            var card = new Card { Name = "Test Card", Type = "Test Type" };
            cardCollection.InsertOne(card);

            // Act
            Card.CardFind(cardCollection);

            // Assert
            var foundCard = cardCollection.Find(x => x.Name == "Test Card").FirstOrDefault();
            Assert.NotNull(foundCard);
            Assert.Equal(card.Name, foundCard.Name);
            Assert.Equal(card.Type, foundCard.Type);

            // Clean up
            cardCollection.DeleteOne(x => x.Name == "Test Card");
        }

        [Fact]
        public void TestCardDelete()
        {
            // Arrange
            var client = new MongoClient("mongodb://localhost:27017");
            var cardCollection = client.GetDatabase("FFCollection").GetCollection<Card>("cards");
            var card = new Card { Name = "Test Card", Type = "Test Type" };
            cardCollection.InsertOne(card);

            // Act
            Card.CardDelete(cardCollection);

            // Assert
            var foundCard = cardCollection.Find(x => x.Name == "Test Card").FirstOrDefault();
            Assert.Null(foundCard);

            // Clean up
            cardCollection.DeleteOne(x => x.Name == "Test Card");
        }

        [Fact]
        public void TestCardUpdate()
        {
            // Arrange
            var client = new MongoClient("mongodb://localhost:27017");
            var cardCollection = client.GetDatabase("FFCollection").GetCollection<Card>("cards");
            var card = new Card { Name = "Test Card", Type = "Test Type" };
            cardCollection.InsertOne(card);

            // Act
            Card.CardUpdate(cardCollection);

            // Assert
            var updatedCard = cardCollection.Find(x => x.Name == "Test Card").FirstOrDefault();
            Assert.NotNull(updatedCard);
            Assert.Equal("Updated Type", updatedCard.Type);

            // Clean up
            cardCollection.DeleteOne(x => x.Name == "Test Card");
        }
    }
}