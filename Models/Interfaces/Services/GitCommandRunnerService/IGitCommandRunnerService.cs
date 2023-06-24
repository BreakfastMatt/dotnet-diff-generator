using Models.Interfaces.Config;

namespace Models.Interfaces.Services.GitCommandRunnerService;

/// <summary>
/// A service to execute various git commands asynchronously
/// </summary>
public interface IGitCommandRunnerService
{
  /// <summary>
  /// Configures the working directory & remote for the specified repo
  /// </summary>
  /// <param name="repoDetail">The details of the repository where the git commands will be run</param>
  /// <param name="remote">The remote server to run git commands against</param>
  void SetGitRepoDetail(IRepositoryDetails repoDetail, string remote = "origin");

  /// <summary>
  /// Utilises the git.exe application installed on the user's machine to execute git commands against a specified repository
  /// </summary>
  /// <param name="gitCommand">The actual git command to be run</param>
  /// <returns>The output of the provided git command</returns>
  Task<string?> ExecuteGitCommandAsync(string gitCommand);

  /// <summary>
  /// Checks if there are any outstanding changes on the specified repository
  /// </summary>
  /// <returns>True if there are outstanding changes in your working tree, otherwise false</returns>
  Task<bool> CheckWorkingTreeForOutstandingChangesAsync();

  /// <summary>
  /// Runs the 'git stash save "diff_generator" --include-untracked' command against the specified repo.
  /// This will stash any outstanding changes currently in the working tree
  /// </summary>
  /// <returns>The output of the 'git stash' command</returns>
  Task<string?> GitStashSaveAsync();

  /// <summary>
  /// Runs the 'git pop "<paramref name="stashName"/>"' command against the specified repo.
  /// This will restore the working tree to its original status.
  /// </summary>
  /// <param name="stashName">Name of the stash to pop</param>
  /// <returns>The output of the 'git pop' command</returns>
  Task<string?> GitStashPopAsync(string stashName);

  /// <summary>
  /// Or Runs the the 'git fetch origin <paramref name="name"/>' command.
  /// This will fetch the exact changes for the specified branch or tag.
  /// </summary>
  /// <param name="name">The name of the branch or tag</param>
  /// <returns>The output of the 'git fetch' command</returns>
  Task<string?> GitFetchAsync(string name);

  /// <summary>
  /// Runs the 'git checkout <paramref name="name"/>' command against the specified repo.
  /// This will checkout the <paramref name="name"/> branch.
  /// </summary>
  /// <param name="name">The name of the branch or tag</param>
  /// <returns>The output of the 'git checkout' command</returns>
  Task<string?> GitCheckout(string name);

  /// <summary>
  /// Runs the 'git pull origin <paramref name="name"/>' command.
  /// This will pull the specific changes for the specified branch or tag.
  /// </summary>
  /// <param name="name">The name of the branch or tag</param>
  /// <returns>The output of the 'git pull' command</returns>
  Task<string?> GitPullAsync(string name);

  /// <summary>
  /// Runs the 'git log <paramref name="from"/>..<paramref name = "to" /> --pretty=format:"%an=====%s --no-merges" command.
  /// This will generate the diffs between the specified branches/tags
  /// </summary>
  /// <param name="from">The name of the branch or tag to go from</param>
  /// <param name="to">The name of the branch or tag to go to</param>
  /// <returns>The generated diff output from the 'git log' command</returns>
  Task<string?> GitLogAsync(string from, string to);
}