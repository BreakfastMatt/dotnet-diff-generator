using System.Diagnostics;
using Models.Constants;
using Models.Interfaces.Config;
using Models.Interfaces.Services.GitCommandRunnerService;

namespace Application.GitCommandRunnerService;

public class GitCommandRunnerService : IGitCommandRunnerService
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
    throw new NotImplementedException();
  }

  public string? GitFetch(string? name = null)
  {
    throw new NotImplementedException();
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
      FileName = "git",
      Arguments = gitCommand,
      WorkingDirectory = repositoryDetail.Path,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true
    };

    // Execute the git command using the ProcessStartInfo
    using var gitProcess = Process.Start(processStartInfo);
    if (gitProcess == null) return null;
    gitProcess.WaitForExit();

    // Check if there was a StandardError result
    string error = gitProcess.StandardError.ReadToEnd();
    // TODO: currently not dealing with the error.

    // Check if there was a StandardOutput result or StandardError result
    string output = gitProcess.StandardOutput.ReadToEnd();
    return output;
  }
}