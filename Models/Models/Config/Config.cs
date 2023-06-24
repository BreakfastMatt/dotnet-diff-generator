using Models.Interfaces.Config;

namespace Models.Models.Config;

public class Config : IConfig
{
    public string OutputPath
    {
        get; set;
    }

    public List<RepositoryDetails> RepositoryDetails
    {
        get; set;
    }
}