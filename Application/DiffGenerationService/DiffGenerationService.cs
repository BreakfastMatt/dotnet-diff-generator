using Models.Interfaces.Config;
using Models.Interfaces.Services.DiffGenerationService;
using Models.Interfaces.Services.GitCommandRunnerService;
using Models.Models.Commit;

namespace Application.DiffGenerationService;

public class DiffGenerationService : IDiffGenerationService
{
  // Inject relevant services
  private readonly IGitCommandRunnerService gitCommandRunnerService;
  public DiffGenerationService(IGitCommandRunnerService gitCommandRunnerService)
  {
    this.gitCommandRunnerService = gitCommandRunnerService;
  }

  public Task<List<Commit>> GenerateRawDiffForRepositoryAsync(IRepositoryDetails repoDetail, string fromReference, string toReference)
  {
    gitCommandRunnerService.SetGitRepoDetail(repoDetail);
    var rawDiffs = gitCommandRunnerService.GitLogAsync(fromReference, toReference);



    throw new NotImplementedException();
  }

  public async Task<bool> GenerateRepositoryDiffsAsync(IRepositoryDetails repoDetail, string build, string fromReference, string toReference)
  {
    throw new NotImplementedException();
  }

  public string GroupDiffsForRepository(List<Commit> commits)
  {
    throw new NotImplementedException();
  }
}