using Models.Models.Config;

namespace Models.Interfaces.Config;

public class Config : IConfig
{
  public string OutputPath { get; set; }

  public List<RepositoryDetails> RepositoryDetails { get; set; }
}