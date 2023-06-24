using Domain.JsonSerialiser;

namespace Tests.DomainServiceTests.JsonSerialiserTests;

[TestFixture]
public class JsonSerialiserTests
{
  /// <summary>
  /// A basic c# model to test the JSON serialiser logic
  /// </summary>
  private class JsonSerialiserTestModel
  {
    public int? IntProp { get; set; }
    public string? StringProp { get; set; }
    public decimal? DecimalProp { get; set; }
    public bool? BoolProp { get; set; }
  }
  private static bool JsonSerialiserTestModelEquality(JsonSerialiserTestModel obj1, JsonSerialiserTestModel obj2)
  {
    // Checks the underlying properties on the objects
    var equality = obj1.IntProp == obj1.IntProp
      && obj1.StringProp == obj2.StringProp
      && obj1.DecimalProp == obj2.DecimalProp
      && obj1.BoolProp == obj2.BoolProp;
    return equality;
  }

  /// <summary>
  /// Tests the deserialisation of a JSON-string functionality
  /// </summary>
  [Test]
  public void JsonDeserialiserTest()
  {
    // Arrange
    var jsonSerialiser = new JsonSerialiser();
    var jsonString = "{\"StringProp\":\"Hello world!\"}";

    // Act
    var deserialisedResponse = jsonSerialiser.DeserializeObject<JsonSerialiserTestModel>(jsonString);

    // Assert
    var expected = new JsonSerialiserTestModel { StringProp = "Hello world!" };
    Assert.That(JsonSerialiserTestModelEquality(deserialisedResponse, expected), Is.True);
  }

  /// <summary>
  /// Tests the serialisiation of a C# object to json functionality
  /// </summary>
  [Test]
  public void JsonSerialiserTest()
  {
    // Arrange
    var jsonSerialiser = new JsonSerialiser();
    var model = new JsonSerialiserTestModel { IntProp = 5 };

    // Act
    var serialisedResponse = jsonSerialiser.SerializeObject<JsonSerialiserTestModel>(model);

    // Assert
    var expected = "{\"IntProp\":5}";
    Assert.That(serialisedResponse?.Equals(expected) ?? false, Is.True);
  }
}