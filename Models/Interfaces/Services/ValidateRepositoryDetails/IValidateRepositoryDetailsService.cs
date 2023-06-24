using Models.Interfaces.Config;
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
  /// <param name="repoDetailsList">The configured list of repositories to be validated</param>
  /// <param name="names">The name of the branches/tags to check</param>
  /// <returns>True if all the validation rules pass for the configured repositories, otherwise false</returns>
  Task<bool> ValidateRepositoryDetailsAsync(List<RepositoryDetails> repoDetailsList, IEnumerable<string> names);

  /// <summary>
  /// Checks whether the specified repo exists
  /// </summary>
  /// <param name="repoDetails">The repo to be validated</param>
  /// <returns>True if the repo exists, otherwise false</returns>
  Task<bool> ValidateRepoExistsAsync(IRepositoryDetails repoDetails);

  /// <summary>
  /// Checks whether the user has access to the configured repo
  /// </summary>
  /// <param name="repoDetails">The repo to be validated</param>
  /// <returns>True if the user has access to the repo, otherwise false</returns>
  Task<bool> ValidateRepoAccessAsync(IRepositoryDetails repoDetails);

  /// <summary>
  /// Checks whether the configured branches/tags exist on the repo
  /// </summary>
  /// <param name="repoDetails">The repo to be validated</param>
  /// <param name="names">The name of the branches/tags to check</param>
  /// <returns>True if the branches/tags exist on the repo, otherwise false</returns>
  Task<bool> ValidateBranchExistenceAsync(IRepositoryDetails repoDetails, IEnumerable<string> names);
}