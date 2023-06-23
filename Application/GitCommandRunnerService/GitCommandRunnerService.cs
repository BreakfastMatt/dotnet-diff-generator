using System.Diagnostics;
using System.Text.RegularExpressions;
using Models.Constants;
using Models.Interfaces.Config;
using Models.Interfaces.Services.GitCommandRunnerService;

namespace Application.GitCommandRunnerService;

public class GitCommandRunnerService : IGitCommandRunnerService // TODO: error handling logic & unit tests need to be added still
{
  // Repository & Remote details for git commands
  private IRepositoryDetails repositoryDetail;
  private string remote;

  public void SetGitRepoDetail(IRepositoryDetails repoDetail, string remote = GlobalConstants.gitDefaultRemote)
  {
    // Sets the global git command details for the repository
    this.repositoryDetail = repoDetail;
    this.remote = remote;
  }

  public bool CheckWorkingTreeForOutstandingChanges()
  {
    // Execute the 'git status' command to check if there are any outstanding changes
    var gitStatus = ExecuteGitCommand("status");
    if (string.IsNullOrEmpty(gitStatus)) return false;

    // Check if there are 
    var hasUncommittedChanges = gitStatus.Contains(GlobalConstants.gitUncommittedChanges);
    var hasUnstagedChanges = gitStatus.Contains(GlobalConstants.gitUnstagedChanges);
    var hasUntrackedFiles = gitStatus.Contains(GlobalConstants.gitUntrackedFiles);
    return hasUncommittedChanges || hasUnstagedChanges || hasUntrackedFiles;
  }

  public string? GitStashSave()
  {
    // Execute the 'git stash save stashName' command
    var gitStashCommand = $"stash save \"{GlobalConstants.gitTempStashName}\"";
    var output = ExecuteGitCommand(gitStashCommand);
    return output;
  }

  public string? GitStashPop(string stashName = GlobalConstants.gitTempStashName)
  {
    // Execute the 'git stash list' command & extract the stash index using a Regex
    var gitStashList = ExecuteGitCommand("stash list");
    var stashDetails = gitStashList?.Split('\n')?.FirstOrDefault(stashDetail => stashDetail.Contains(stashName)) ?? string.Empty;
    var regexMatch = Regex.Matches(stashDetails, @"stash@{(\d+)}").FirstOrDefault();
    if (regexMatch == null || string.IsNullOrEmpty(regexMatch.Value)) return null;

    // Execute the 'git stash pop <regexMatch.Value> command
    var gitStashCommand = $"stash pop \"{regexMatch.Value}\"";
    var stashOutput = ExecuteGitCommand(gitStashCommand);
    return stashOutput;
  }

  public string? GitFetch(string? name = null)
  {
    // Execute the 'git fetch <remote> <name>' command
    var gitFetchCommand = $"fetch {this.remote} {name}";
    var fetchOutput = ExecuteGitCommand(gitFetchCommand);
    return fetchOutput;
  }

  public string? GitPull(string? name = null)
  {
    throw new NotImplementedException();
  }

  public string? GitLog(string from, string to)
  {
    throw new NotImplementedException();
  }

  public string? ExecuteGitCommand(string gitCommand)
  {
    // Configures the ProcessStartInfo needed to execute the provided git command
    var processStartInfo = new ProcessStartInfo
    {
      // The specific git command to run
      FileName = "git",
      Arguments = gitCommand,

      // The Repository To run against
      WorkingDirectory = repositoryDetail.Path,

      // Terminal Details
      CreateNoWindow = true,
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true
    };

    // Execute the git command using the ProcessStartInfo
    using var gitProcess = Process.Start(processStartInfo);
    if (gitProcess == null) return null;
    gitProcess.WaitForExit();

    // Check if there was a StandardOutput or StandardError result
    string error = gitProcess.StandardError.ReadToEnd();
    string output = gitProcess.StandardOutput.ReadToEnd();
    return string.IsNullOrEmpty(output) ? error : output;
  }
}