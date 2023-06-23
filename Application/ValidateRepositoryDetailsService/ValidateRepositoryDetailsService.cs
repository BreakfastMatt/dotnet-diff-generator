using Models.Interfaces.Config;
using Models.Interfaces.Services.GitCommandRunnerService;
using Models.Interfaces.Services.ValidateRepositoryDetails;
using Models.Models.Config;

namespace Application.ValidateRepositoryDetailsService;

/// <inheritdoc/>
public class ValidateRepositoryDetailsService : IValidateRepositoryDetailsService
{
  private readonly IGitCommandRunnerService gitCommandRunnerService;
  public ValidateRepositoryDetailsService(IGitCommandRunnerService gitCommandRunnerService)
  {
    this.gitCommandRunnerService = gitCommandRunnerService;
  }

  /// <inheritdoc/>
  public async Task<bool> ValidateRepositoryDetailsAsync(List<RepositoryDetails> repoDetailsList)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public async Task<bool> ValidateRepoExistsAsync(IRepositoryDetails repoDetails)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public async Task<bool> ValidateRepoAccessAsync(IRepositoryDetails repoDetails)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public async Task<bool> ValidateBranchExistenceAsync(IRepositoryDetails repoDetails, IEnumerable<string> branchNames)
  {
    throw new NotImplementedException();
  }
}