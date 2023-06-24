using Models.Models.Config;

namespace Models.Interfaces.Services.ReadFromConfigService;

/// <summary>
/// A service to read in the config.json file and extract the repository information
/// </summary>
public interface IReadFromConfigService
{
  /// <summary>
  /// Reads in the repostiory details from the config.json file and returns a list of repo details
  /// </summary>
  /// <returns>The config details</returns>
  IConfig ReadFromConfig();
}