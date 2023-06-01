namespace Models.Interfaces.Services.JsonSerialiser;

/// <summary>
/// The logic to serialise and deserialise JSON content
/// </summary>
public interface IJsonSerialiser
{
  /// <summary>
  /// Deserialises the supplied json string into the provided object
  /// </summary>
  /// <typeparam name="T">The model type of the object you are deserialising into</typeparam>
  /// <param name="json">The JSON file content in string format</param>
  /// <returns>The JSON content in object format</returns>
  T DeserializeObject<T>(string json);


  /// <summary>
  /// Serialises the provided object into a JSON string
  /// </summary>
  /// <typeparam name="T">The model type of the object you are serialising from</typeparam>
  /// <param name="obj">The instance of the object you are serialising from</param>
  /// <returns>The JSON content in string format</returns>
  string SerializeObject<T>(T obj);
}