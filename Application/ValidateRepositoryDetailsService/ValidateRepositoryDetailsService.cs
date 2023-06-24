using System.Text.RegularExpressions;
using Models.Interfaces.Config;
using Models.Interfaces.Services.GitCommandRunnerService;
using Models.Interfaces.Services.ValidateRepositoryDetails;
using Models.Models.Config;

namespace Application.ValidateRepositoryDetailsService;

// TODO: implement this service :)
public class ValidateRepositoryDetailsService : IValidateRepositoryDetailsService
{
  private readonly IGitCommandRunnerService gitCommandRunnerService;
  public ValidateRepositoryDetailsService(IGitCommandRunnerService gitCommandRunnerService)
  {
    this.gitCommandRunnerService = gitCommandRunnerService;
  }

  public async Task<bool> ValidateRepositoryDetailsAsync(List<RepositoryDetails> repoDetailsList)
  {
    //repoDetailsList.ForEach(repoDetails =>
    //{
    //  gitCommandRunnerService.SetGitRepoDetail(repoDetails);
    //});

    // Validate each of the repositories
    // - ValidateRepoExistsAsync
    // - ValidateRepoAccessAsync
    // - ValidateBranchExistenceAsync
    throw new NotImplementedException();
  }

  public Task<bool> ValidateRepoExistsAsync(IRepositoryDetails repoDetails)
  {
    // Checks if a Repository exists
    var repoPath = repoDetails.Path;
    var repoExists = Directory.Exists(repoPath);

    // Deal with scenario where there path doesn't exist
    if (repoExists == false)
    {
      var response = $"{repoDetails.Name} does not exist at: {repoPath}";
      Console.WriteLine(response);
    }
    return Task.FromResult(repoExists);
  }

  public async Task<bool> ValidateRepoAccessAsync(IRepositoryDetails repoDetails)
  {
    // Checks if the user has access to run git commands against the specified repository by running a simple list command
    var gitListRemoteBranchesCommand = "ls-remote";
    var remoteBranches = await gitCommandRunnerService.ExecuteGitCommandAsync(gitListRemoteBranchesCommand);

    // Check if the command executed 
    var hasRepoAccess = string.IsNullOrEmpty(remoteBranches) == false;
    if (hasRepoAccess == false)
    {
      var response = $"You do not have access to the {repoDetails.Name} repository";
      Console.WriteLine(response);
    }
    return hasRepoAccess;
  }

  public async Task<bool> ValidateBranchExistenceAsync(IRepositoryDetails repoDetails, IEnumerable<string> names)
  {
    // Fetch list of tags and branches on the remote
    var gitListRemoteBranchesCommand = "ls-remote";
    var listRemoteOutput = await gitCommandRunnerService.ExecuteGitCommandAsync(gitListRemoteBranchesCommand) ?? string.Empty;
    //var remoteBranches = Regex.Matches(listRemoteOutput, @"refs/heads/(.+)");
    var remoteBranches = Regex.Matches(listRemoteOutput, @"refs/heads/(.+)").Select(branch => branch.Groups?[1]?.Value?.Trim()).ToList();
    //var remoteTags = Regex.Matches(listRemoteOutput, @"refs/tags/(.+)");
    var remoteTags = Regex.Matches(listRemoteOutput, @"refs/tags/(.+)").Select(branch => branch.Groups?[1]?.Value?.Trim()).ToList();

    // Checks if the specified branches or tags exist
    var isValid = names.Any();
    foreach (var name in names)
    {
      // Checks the branch or tag
      var existsOnRemote = (remoteBranches?.Contains(name) ?? false) || (remoteTags?.Contains(name) ?? false);
      if (existsOnRemote == false)
      {
        var response = $"The reference {name} does not exist on the {repoDetails.Name} repository.";
        Console.WriteLine(response);
        isValid = false;
      }
    }
    return isValid;
  }
}