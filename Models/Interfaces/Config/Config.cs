using Models.Models.Config;

namespace Models.Interfaces.Config;

/// <inheritdoc/>
public class Config : IConfig
{
  /// <inheritdoc/>
  public string OutputPath { get; set; }

  /// <inheritdoc/>
  public List<RepositoryDetails> RepositoryDetails { get; set; }
}