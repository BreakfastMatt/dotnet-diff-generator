using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Models.Constants;
using Models.Interfaces.Config;
using Models.Interfaces.Services.GitCommandRunnerService;

namespace Application.GitCommandRunnerService;

// TODO: error handling logic & unit tests need to be added still
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

  public async Task<bool> CheckWorkingTreeForOutstandingChangesAsync()
  {
    // Execute the 'git status' command
    var gitStatus = await ExecuteGitCommandAsync("status");
    if (string.IsNullOrEmpty(gitStatus))
    {
      return false;
    }

    // Check if there are any outstanding changes on the working tree
    var hasUncommittedChanges = gitStatus.Contains(GlobalConstants.gitUncommittedChanges);
    var hasUnstagedChanges = gitStatus.Contains(GlobalConstants.gitUnstagedChanges);
    var hasUntrackedFiles = gitStatus.Contains(GlobalConstants.gitUntrackedFiles);
    return hasUncommittedChanges || hasUnstagedChanges || hasUntrackedFiles;
  }

  public async Task<string?> GitStashSaveAsync()
  {
    // Execute the 'git stash save stashName' command
    var gitStashCommand = $"stash save \"{GlobalConstants.gitTempStashName}\"";
    var stashSaveOutput = await ExecuteGitCommandAsync(gitStashCommand);
    return stashSaveOutput;
  }

  public async Task<string?> GitStashPopAsync(string stashName = GlobalConstants.gitTempStashName)
  {
    // Execute the 'git stash list' command & extract the stash index using a Regex
    var stashListOutput = await ExecuteGitCommandAsync("stash list");
    var stashDetails = stashListOutput?.Split('\n')?.FirstOrDefault(stashDetail => stashDetail.Contains(stashName)) ?? string.Empty;
    var regexMatch = Regex.Matches(stashDetails, @"stash@{(\d+)}").FirstOrDefault();
    if (regexMatch == null || string.IsNullOrEmpty(regexMatch.Value))
    {
      return null;
    }

    // Execute the 'git stash pop <regexMatch.Value> command
    var gitStashCommand = $"stash pop \"{regexMatch.Value}\"";
    var stashPopOutput = await ExecuteGitCommandAsync(gitStashCommand);
    return stashPopOutput;
  }

  public async Task<string?> GitFetchAsync(string name)
  {
    // Execute the 'git fetch <remote> <name>' command
    var gitFetchCommand = $"fetch {this.remote} {name}";
    var fetchOutput = await ExecuteGitCommandAsync(gitFetchCommand);
    return fetchOutput;
  }

  public async Task<string?> GitCheckout(string name)
  {
    // Executes the 'git checkout <name>' command
    var gitCheckoutCommand = $"checkout {name}";
    var checkoutOutput = await ExecuteGitCommandAsync(gitCheckoutCommand);
    return checkoutOutput;
  }

  public async Task<string?> GitPullAsync(string name)
  {
    // Execute the 'git pull <remote> <name>' command
    var gitPullCommand = $"pull {this.remote} {name}";
    var pullOutput = await ExecuteGitCommandAsync(gitPullCommand);
    return pullOutput;
  }

  public async Task<string?> GitLogAsync(string from, string to)
  {
    // Executes the 'git log' command to pull the diff for the repository
    var fromDiff = Regex.Match(from, @"^\d{1,2}\.\d{1,2}\.\d{1,2}(\.\d{1,2})?$").Success ? $"'{from}'" : $"{this.remote}/{from}";
    var toDiff = Regex.Match(to, @"^\d{1,2}\.\d{1,2}\.\d{1,2}(\.\d{1,2})?$").Success ? $"'{to}'" : $"{this.remote}/{to}";
    var gitLogCommand = $"log {fromDiff}..{toDiff} --pretty=format:\"%an{GlobalConstants.gitLogDelimiter}%s\" --no-merges";
    var logOutput = await ExecuteGitCommandAsync(gitLogCommand);
    return logOutput;
  }

  public async Task<string?> ExecuteGitCommandAsync(string gitCommand)
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
    using var gitProcess = new Process { StartInfo = processStartInfo };

    // Event handler for capturing standard output
    var standardOutputLock = new object();
    var standardOutputStringBuilder = new StringBuilder();
    var standardOutputTaskCompletionSource = new TaskCompletionSource<string>();
    gitProcess.OutputDataReceived += (sender, outputEvent) =>
    {
      if (!string.IsNullOrEmpty(outputEvent?.Data))
      {
        lock (standardOutputLock)
        {
          standardOutputStringBuilder.AppendLine(outputEvent.Data);
        }
      }
    };

    // Event handler for capturing standard error
    var standardErrorLock = new object();
    var standardErrorStringBuilder = new StringBuilder();
    var standardErrorTaskCompletionSource = new TaskCompletionSource<string>();
    gitProcess.ErrorDataReceived += (sender, errorEvent) =>
    {
      if (!string.IsNullOrEmpty(errorEvent?.Data))
      {
        lock (standardErrorLock)
        {
          standardErrorStringBuilder.AppendLine(errorEvent.Data);
        }
      }
    };

    // Start the git process and begin reading output asynchronously
    gitProcess.Start();
    gitProcess.BeginOutputReadLine();
    gitProcess.BeginErrorReadLine();
    await gitProcess.WaitForExitAsync();

    // Set the completion result for standard output and standard error
    standardOutputTaskCompletionSource.SetResult(standardOutputStringBuilder.ToString());
    standardErrorTaskCompletionSource.SetResult(standardErrorStringBuilder.ToString());

    // Get the final standard output and standard error values
    var standardOutput = await standardOutputTaskCompletionSource.Task;
    var standardError = await standardErrorTaskCompletionSource.Task;

    // Return the appropriate output based on whether standard output is empty
    return string.IsNullOrEmpty(standardOutput) ? standardError : standardOutput;
  }
}