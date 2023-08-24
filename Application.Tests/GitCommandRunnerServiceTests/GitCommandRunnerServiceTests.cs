namespace Application.Tests.GitCommandRunnerServiceTests;
using Application.GitCommandRunnerService;
using Models.Models.Config;

public class GitCommandRunnerServiceTests
{
  /// <summary>
  /// Initialises the GitCommandRunnerServiceTests on Startup
  /// </summary>
  public GitCommandRunnerServiceTests()
  {
    // Store the working changes
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Store the working changes
    var gitStashCommand = "stash save \"Git-Diff-Startup\"";
    var stashSaveOutput = gitCommandRunnerService.ExecuteGitCommandAsync(gitStashCommand).Result;
    if (stashSaveOutput != null && stashSaveOutput.Contains("No local changes"))
    {
      gitCommandRunnerService.ExecuteGitCommandAsync("stash pop 0").Wait();
      gitCommandRunnerService.ExecuteGitCommandAsync(gitStashCommand).Wait();
    }
  }

  /// <summary>
  /// Cleanups the GitCommandRunnerServiceTests on exit.
  /// </summary>
  private static void Dispose()
  {
    // Store the working changes
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);
    gitCommandRunnerService.ExecuteGitCommandAsync("stash pop 0").Wait();
  }

  /// <summary>
  /// Tests GitCommandRunnerService using the basic "git status" command.
  /// </summary>
  [Fact]
  public async Task ExecuteGitCommandAsync_ForGitStatus_ShouldReturnOnBranchStatus()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var output = await gitCommandRunnerService.ExecuteGitCommandAsync("status");

    // Assert
    Assert.Multiple(() =>
    {
      Assert.NotNull(output);
      Assert.Contains("On branch", output);
    });
  }

  /// <summary>
  /// Tests if there are any outstanding changes on the repository using the CheckWorkingTreeForOutstandingChanges function.
  /// </summary>
  [Fact]
  public async Task CheckWorkingTreeForOutstandingChanges_ForRepoWithChanges_ShouldReturnTrue()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    await gitCommandRunnerService.ExecuteGitCommandAsync("stash pop 0"); // Bring Back Outstanding Changes
    var workingTreeHasChanges = await gitCommandRunnerService.CheckWorkingTreeForOutstandingChangesAsync(); // Check for Outstanding Changes
    await gitCommandRunnerService.ExecuteGitCommandAsync("stash save \"Git-Diff-Startup\""); // Save changes back to stash

    // Assert
    Assert.True(workingTreeHasChanges);
  }

  /// <summary>
  /// Tests the git stash save and apply functions
  /// </summary>
  [Fact]
  public async Task GitStashSaveAsync_ForRepoWithChanges_ShouldStashAndRestore()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    await gitCommandRunnerService.ExecuteGitCommandAsync("stash pop 0"); // Bring Back Outstanding Changes
    var gitStatusBefore = await gitCommandRunnerService.ExecuteGitCommandAsync("status"); // Check before status
    var gitStashSave = await gitCommandRunnerService.GitStashSaveAsync(); // Stash current 
    var gitRestore = await gitCommandRunnerService.GitStashPopAsync(); // Pop current
    var gitStatusAfter = await gitCommandRunnerService.ExecuteGitCommandAsync("status"); // Check after status
    await gitCommandRunnerService.ExecuteGitCommandAsync("stash save \"Git-Diff-Startup\""); // Restore Outstanding Changes

    // Assert
    Assert.Multiple(() =>
    {
      Assert.Contains("Saved working directory", gitStashSave);
      Assert.Contains("Dropped stash", gitRestore);
      Assert.Equal(gitStatusBefore, gitStatusAfter);
    });
  }

  /// <summary>
  /// Tests the 'git fetch' functionality
  /// </summary>
  [Fact]
  public async Task GitFetchAsync_ForTestRepoBranch_ShouldReturnFetchSuccessStatus()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetch = await gitCommandRunnerService.GitFetchAsync(Constants.TestRepositoryBranchName);

    // Assert
    //Assert.That(gitFetch != null && gitFetch.Contains("branch") && gitFetch.Contains(Constants.TestRepositoryBranchName) && gitFetch.Contains("FETCH_HEAD"), Is.True);

    Assert.Multiple(() =>
    {
      Assert.NotNull(gitFetch);
      Assert.Contains("branch", gitFetch);
      Assert.Contains(Constants.TestRepositoryName, gitFetch);
      Assert.Contains("FETCH_HEAD", gitFetch);
    });
  }

  /// <summary>
  /// Tets the 'git checkout' functionality
  /// </summary>
  [Fact]
  public async Task GitCheckout_ForTestRepositoryBranch_ShouldReturnCheckoutSuccessStatus()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitCheckout = await gitCommandRunnerService.GitCheckout(Constants.TestRepositoryBranchName);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.NotNull(gitCheckout);
      Assert.Contains("origin/{Constants.TestRepositoryBranchName}", gitCheckout);
    });
  }

  /// <summary>
  /// Tests the 'git pull' functionality
  /// </summary>
  [Fact]
  public async Task GitPullAsync_ForTestRepositoryBranch_ShouldReturnPullSuccessStatus()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    await gitCommandRunnerService.ExecuteGitCommandAsync($"checkout {Constants.TestRepositoryBranchName}");
    var gitPull = await gitCommandRunnerService.GitPullAsync(Constants.TestRepositoryBranchName);

    // Assert
    //Assert.That(gitPull != null && ));
    Assert.Multiple(() =>
    {
      Assert.NotNull(gitPull);
      var gitPullSuccess = (gitPull.Contains("Already up to date") || gitPull.Contains("Updating"));
      Assert.True(gitPullSuccess, "The Git Pull was not successful");
    });
  }

  /// <summary>
  /// Tests the 'git log' functionality using branches
  /// </summary>
  [Fact]
  public async Task GitLogAsync_ForTestRepositoryBranches_ShouldReturnLogSuccessStatus()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetchFrom = await gitCommandRunnerService.GitFetchAsync("11.8.1_hotfix_15052023");
    var gitFetchTo = await gitCommandRunnerService.GitFetchAsync("11.8.2_hotfix_05062023");
    var gitStatus = await gitCommandRunnerService.ExecuteGitCommandAsync("status");
    var gitLog = await gitCommandRunnerService.GitLogAsync("11.8.1_hotfix_15052023", "11.8.2_hotfix_05062023");

    // Assert
    var gitLogFailed = gitLog?.Contains("unknown revision") ?? true;
    Assert.False(gitLogFailed);
  }

  /// <summary>
  /// Tests the 'git log' functionality using tags
  /// </summary>
  [Fact]
  public async Task GitLogAsync_ForTestRepositoryTags_ShouldReturnLogSuccessStatus()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetchFrom = await gitCommandRunnerService.GitFetchAsync("11.8.1.0");
    var gitFetchTo = await gitCommandRunnerService.GitFetchAsync("11.8.2");
    var gitStatus = await gitCommandRunnerService.ExecuteGitCommandAsync("status");
    var gitLog = await gitCommandRunnerService.GitLogAsync("11.8.1.0", "11.8.2");

    // Assert
    var gitLogFailed = gitLog?.Contains("unknown revision") ?? true;
    Assert.False(gitLogFailed);
  }
}