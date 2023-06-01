using Models.Models.Config;

namespace Models.Interfaces.Config;

public class Config : IConfig
{
  public List<RepositoryDetails> Repositories { get; set; }
}