namespace Models.Interfaces.Services.Delegator;

/// <summary>
/// A service that calls the relevant services to generate and format the git diffs for the configured repos
/// </summary>
public interface IDelegatorService
{
  /// <summary>
  /// Calls the various services to generate and format the git diffs
  /// </summary>
  Task GetResponseAsync();
}