using Models.Interfaces.Config;
using Models.Interfaces.Services.JsonSerialiser;
using Models.Interfaces.Services.ReadFromConfig;
using Models.Models.Config;

namespace Application.ReadFromConfigService;

/// <inheritdoc/>
public class ReadFromConfigService : IReadFromConfigService
{
  private readonly IJsonSerialiser jsonSerialiser;
  public ReadFromConfigService(IJsonSerialiser jsonSerialiser)
  {
    this.jsonSerialiser = jsonSerialiser;
  }

  /// <inheritdoc/>
  public IConfig ReadFromConfig()
  {
    // Read in the contents from the config.json file
    var basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName; // TODO: need a better way to do this
    var configFilePath = Path.Combine(basePath, "config.json");
    var configJson = File.ReadAllText(configFilePath) ?? string.Empty;

    // Deserialise the JSON to an object
    var deserialisedObject = jsonSerialiser.DeserializeObject<Config>(configJson) ?? new Config();
    return deserialisedObject;
  }
}