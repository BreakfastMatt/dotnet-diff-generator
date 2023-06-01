using Models.Interfaces.Config;

namespace Models.Models.Config;

public class RepositoryDetails : IRepositoryDetails
{
  public string Name { get; set; }
  public string Path { get; set; }
}