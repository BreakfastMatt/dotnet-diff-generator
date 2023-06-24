using Models.Interfaces.Config;

namespace Models.Interfaces.Services.DelegatorService;

/// <summary>
/// A service that calls the relevant services to generate and format the git diffs for the configured repos
/// </summary>
public interface IDelegatorService
{
  /// <summary>
  /// Repeatedly runs the CallServicesAsync function until the user prompts to stop.
  /// </summary>
  Task DelegateAsync();

  /// <summary>
  /// Calls the various services to generate and format the git diffs
  /// </summary>
  /// <param name="config">The parsed json config</param>
  /// <param name="names">The name of the branches or tags entered by the user</param>
  /// <returns>True if the function executed successfully, otherwise false</returns>
  Task<bool> CallServicesAsync(IConfig config, List<string> names);
}