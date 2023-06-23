using System.Diagnostics;
using Models.Interfaces.Config;

namespace Models.Interfaces.Services.GitCommandRunnerService;

/// <summary>
/// A service to execute various git commands
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
  string? ExecuteGitCommand(string gitCommand);

  /// <summary>
  /// Checks if there are any outstanding changes on the specified repository
  /// </summary>
  /// <returns>True if there are outstanding changes in your working tree, otherwise false</returns>
  bool CheckWorkingTreeForOutstandingChanges();

  /// <summary>
  /// Runs the 'git stash save "diff_generator" --include-untracked' command against the specified repo.
  /// This will stash any outstanding changes currently in the working tree
  /// </summary>
  void GitStashSave();

  /// <summary>
  /// Runs the 'git pop "<paramref name="stashName"/>"' command against the specified repo.
  /// This will restore the working tree to its original status.
  /// </summary>
  /// <param name="stashName"></param>
  void GitStashPop(string stashName = "diff_generator");

  /// <summary>
  /// TODO: confirm the commands below
  /// Runs the 'git fetch origin refs/tags/<paramref name="name"/>:refs/tags/<paramref name="name"/>' command for tags.
  /// Or Runs the the 'git fetch origin <paramref name="name"/>' command for branches.
  /// This will fetch the exact changes for the specified branch or tag.
  /// </summary>
  /// <param name="name">The name of the branch or tag</param>
  void GitFetch(string? name = null);

  /// <summary>
  /// Runs the 'git pull origin <paramref name="name"/>' command.
  /// This will pull the specific changes for the specified branch or tag.
  /// </summary>
  /// <param name="name">The name of the branch or tag</param>
  void GitPull(string? name = null);

  /// <summary>
  /// TODO: confirm the command below
  /// Runs the 'git log <paramref name="from"/>..<paramref name="to"/> --pretty=format: %an`t%s --no-merges command' command.
  /// This will generate the diffs between the specified branches/tags
  /// </summary>
  /// <param name="from">The name of the branch or tag to go from</param>
  /// <param name="to">The name of the branch or tag to go to</param>
  /// <returns>The generated diff</returns>
  string GitLog(string from, string to);
}