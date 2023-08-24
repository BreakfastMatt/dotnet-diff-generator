namespace Domain.Tests.JsonSerialiserTests;
using Domain.JsonSerialiser;

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

    public static bool IsEqual(JsonSerialiserTestModel obj1, JsonSerialiserTestModel obj2)
    {
      // Checks the underlying properties on the objects
      var equality = obj1.IntProp == obj2.IntProp
        && obj1.StringProp == obj2.StringProp
        && obj1.DecimalProp == obj2.DecimalProp
        && obj1.BoolProp == obj2.BoolProp;
      return equality;
    }
  }

  /// <summary>
  /// Tests the deserialisation of a JSON-string functionality
  /// </summary>
  [Fact]
  public void DeserializeObject_ForHardcodedModel_ShouldReturnExpectedOutput()
  {
    // Arrange
    var jsonSerialiser = new JsonSerialiser();
    var jsonString = "{\"StringProp\":\"Hello world!\"}";

    // Act
    var deserialisedResponse = jsonSerialiser.DeserializeObject<JsonSerialiserTestModel>(jsonString);

    // Assert
    var expected = new JsonSerialiserTestModel { StringProp = "Hello world!" };
    Assert.Equivalent(expected, deserialisedResponse);
    //Assert.True(JsonSerialiserTestModel.IsEqual(deserialisedResponse, expected));
  }

  /// <summary>
  /// Tests the serialisiation of a C# object to json functionality
  /// </summary>
  [Fact]
  public void SerializeObject_ForHardcodedModel_ShouldReturnExpectedOutput()
  {
    // Arrange
    var jsonSerialiser = new JsonSerialiser();
    var model = new JsonSerialiserTestModel { IntProp = 5 };

    // Act
    var serialisedResponse = jsonSerialiser.SerializeObject<JsonSerialiserTestModel>(model);

    // Assert
    var expected = "{\"IntProp\":5}";
    Assert.Equal(serialisedResponse, expected);
  }
}