using NUnit.Framework;

[TestFixture]
public class CardTests
{
    [Test]
    public void GetName_ReturnsCorrectName()
    {
        // Arrange
        string expectedName = "Test Card";
        Card card = new Card(expectedName);

        // Act
        string actualName = card.GetName();

        // Assert
        Assert.AreEqual(expectedName, actualName);
    }
}