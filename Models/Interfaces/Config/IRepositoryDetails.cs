namespace Models.Interfaces.Config;

/// <summary>
/// The repository details as configured in the config.json file
/// </summary>
public interface IRepositoryDetails
{
  /// <summary>
  /// The name of the repository (used for debugging & display purposes).
  /// </summary>
  public string Name { get; set; }

  /// <summary>
  /// The path of the repository as configured by the user.
  /// </summary>
  public string Path { get; set; }

  /// <summary>
  /// The main branch to be used for the repository.
  /// </summary>
  public string MainBranchName { get; set; }
}