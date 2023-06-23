namespace Models.Models.Config;

/// <summary>
/// The JSON config file details
/// </summary>
public interface IConfig
{
  /// <summary>
  /// The path to save the repo details to
  /// </summary>
  public string OutputPath { get; set; }

  /// <summary>
  /// The configured repositories
  /// </summary>
  public List<RepositoryDetails> RepositoryDetails { get; set; } // TODO: do we have to use the concrete class in the interface like this?  (not ideal)
}