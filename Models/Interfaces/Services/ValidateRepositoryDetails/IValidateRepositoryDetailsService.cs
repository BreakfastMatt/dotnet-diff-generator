using Models.Models.Config;

namespace Models.Interfaces.Services.ValidateRepositoryDetails;

/// <summary>
/// Runs various validation rules against each of the configured repositories to ensure git commands can be run successfully
/// </summary>
public interface IValidateRepositoryDetailsService
{
  /// <summary>
  /// Runs the various validation rules for the configured repositories
  /// </summary>
  /// <param name="repoDetails">The configured repo details to be validated</param>
  /// <returns>True if all the validation rules pass for the configured repositories, otherwise false</returns>
  Task<bool> ValidateRepositoryDetailsAsync(List<RepositoryDetails> repoDetails);

  /// <summary>
  /// Checks whether the specified repo exists
  /// </summary>
  /// <param name="repoName">The repo to be validated</param>
  /// <returns>True if the repo exists, otherwise false</returns>
  Task<bool> ValidateRepoExistsAsync(string repoName);

  /// <summary>
  /// Checks whether the user has access to the configured repo
  /// </summary>
  /// <param name="repoName"></param>
  /// <returns>True if the user has access to the repo, otherwise false</returns>
  Task<bool> ValidateRepoAccessAsync(string repoName);

  /// <summary>
  /// Checks whether the configured branches/tags exist on the repo
  /// </summary>
  /// <param name="repoName">The name of the repo to be checked</param>
  /// <param name="branchNames">The name of the branches/tags to check</param>
  /// <returns>True if the branches/tags exist on the repo, otherwise false</returns>
  Task<bool> ValidateBranchExistenceAsync(string repoName, IEnumerable<string> branchNames);
}