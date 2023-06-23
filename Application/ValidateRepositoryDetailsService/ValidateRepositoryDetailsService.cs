using Models.Interfaces.Services.ValidateRepositoryDetails;
using Models.Models.Config;

namespace Application.ValidateRepositoryDetailsService;

/// <inheritdoc/>
public class ValidateRepositoryDetailsService : IValidateRepositoryDetailsService
{
  public ValidateRepositoryDetailsService()
  {
  }

  /// <inheritdoc/>
  public Task<bool> ValidateRepositoryDetailsAsync(List<RepositoryDetails> repoDetails)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public Task<bool> ValidateRepoExistsAsync(string repoName)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public Task<bool> ValidateRepoAccessAsync(string repoName)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public Task<bool> ValidateBranchExistenceAsync(string repoName, IEnumerable<string> branchNames)
  {
    throw new NotImplementedException();
  }
}