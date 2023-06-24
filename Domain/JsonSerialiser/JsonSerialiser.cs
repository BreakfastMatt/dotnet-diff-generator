using Models.Interfaces.Services.JsonSerialiser;
using Newtonsoft.Json;

namespace Domain.JsonSerialiser;

public class JsonSerialiser : IJsonSerialiser
{
  public T DeserializeObject<T>(string json)
  {
    // Deserialise the JSON into an object
    var deserialisedContent = JsonConvert.DeserializeObject<T>(json);
    return deserialisedContent;
  }

  public string SerializeObject<T>(T obj)
  {
    // JSON serialiser settings
    var jsonSerialiserSettings = new JsonSerializerSettings
    {
      NullValueHandling = NullValueHandling.Ignore,
      DefaultValueHandling = DefaultValueHandling.Ignore,
    };

    // Serialise and return the json content in string format
    var serialisedObject = JsonConvert.SerializeObject(obj, Formatting.None, jsonSerialiserSettings);
    return serialisedObject;
  }
}