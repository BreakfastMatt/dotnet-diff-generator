using Application.GitCommandRunnerService;
using Models.Models.Config;

namespace Tests.ApplicationServiceTests;

[TestFixture]
// TODO: note a mock repo needs to be setup to test against here (will add a config.json file to the test project to allow this to be configured by the user)
public class GitCommandRunnerServiceTests
{
  /// <summary>
  /// The Test Repo to run the tests against
  /// </summary>
  public const string repoName = "C:\\Clients\\Singular\\Git-Diff-Generator-Tests\\inn8.web"; // TODO: have this config driven.
  public const string branch = "dev";

  /// <summary>
  /// Initialises the GitCommandRunnerService Test Fixture on startup.
  /// </summary>
  [OneTimeSetUp]
  public async Task InitialiseGitCommandRunnerServiceTestFixture()
  {
    // Store the working changes
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Store the working changes
    var gitStashCommand = "stash save \"Git-Diff-Startup\"";
    var stashSaveOutput = await gitCommandRunnerService.ExecuteGitCommandAsync(gitStashCommand);
    if (stashSaveOutput != null && stashSaveOutput.Contains("No local changes"))
    {
      await gitCommandRunnerService.ExecuteGitCommandAsync("stash pop 0");
      await gitCommandRunnerService.ExecuteGitCommandAsync(gitStashCommand);
    }
  }

  /// <summary>
  /// Cleanups the GitCommandRunnerService Test Fixture on exit.
  /// </summary>
  [OneTimeTearDown]
  public async Task CleanupGitCommandRunnerServiceTestFixture()
  {
    // Store the working changes
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);
    await gitCommandRunnerService.ExecuteGitCommandAsync("stash pop 0");
  }

  /// <summary>
  /// Tests GitCommandRunnerService using the basic "git status" command.
  /// </summary>
  [Test]
  public async Task ExecuteGitStatusCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var output = await gitCommandRunnerService.ExecuteGitCommandAsync("status");

    // Assert
    Assert.That(output?.Contains("On branch") ?? false, Is.True);
  }

  /// <summary>
  /// Tests if there are any outstanding changes on the repository using the CheckWorkingTreeForOutstandingChanges function.
  /// </summary>
  [Test]
  public async Task CheckWorkingChangesTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    await gitCommandRunnerService.ExecuteGitCommandAsync("stash pop 0"); // Bring Back Outstanding Changes
    var output = await gitCommandRunnerService.CheckWorkingTreeForOutstandingChangesAsync(); // Check for Outstanding Changes
    await gitCommandRunnerService.ExecuteGitCommandAsync("stash save \"Git-Diff-Startup\""); // Save changes back to stash

    // Assert
    Assert.That(output, Is.True);
  }

  /// <summary>
  /// Tests the git stash save and apply functions
  /// </summary>
  [Test]
  public async Task GitStashSaveAndRestoreTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
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
      Assert.That(gitStashSave?.Contains("Saved working directory") ?? false, Is.True);
      Assert.That(gitRestore?.Contains("Dropped stash") ?? false, Is.True);
      Assert.That(gitStatusBefore, Is.EqualTo(gitStatusAfter));
    });
  }

  /// <summary>
  /// Tests the 'git fetch' functionality
  /// </summary>
  [Test]
  public async Task GitFetchCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetch = await gitCommandRunnerService.GitFetchAsync(branch);

    // Assert
    Assert.That(gitFetch != null && gitFetch.Contains("branch") && gitFetch.Contains(branch) && gitFetch.Contains("FETCH_HEAD"), Is.True);
  }

  /// <summary>
  /// Tets the 'git checkout' functionality
  /// </summary>
  [Test]
  public async Task GitCheckoutCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitCheckout = await gitCommandRunnerService.GitCheckout(branch);

    // Assert
    Assert.That(gitCheckout != null && gitCheckout.Contains($"origin/{branch}"), Is.True);
  }

  /// <summary>
  /// Tests the 'git pull' functionality
  /// </summary>
  [Test]
  public async Task GitPullCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    await gitCommandRunnerService.ExecuteGitCommandAsync($"checkout {branch}");
    var gitPull = await gitCommandRunnerService.GitPullAsync(branch);

    // Assert
    Assert.That(gitPull != null && (gitPull.Contains("Already up to date") || gitPull.Contains("Updating")));
  }

  /// <summary>
  /// Tests the 'git log' functionality using branches
  /// </summary>
  [Test]
  public async Task GitLogCommandTestBranches()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetchFrom = await gitCommandRunnerService.GitFetchAsync("11.8.1_hotfix_15052023");
    var gitFetchTo = await gitCommandRunnerService.GitFetchAsync("11.8.2_hotfix_05062023");
    var gitStatus = await gitCommandRunnerService.ExecuteGitCommandAsync("status");
    var gitLog = await gitCommandRunnerService.GitLogAsync("11.8.1_hotfix_15052023", "11.8.2_hotfix_05062023");

    // Assert
    var gitLogFailed = gitLog?.Contains("unknown revision") ?? true;
    Assert.That(gitLogFailed, Is.False);
  }

  /// <summary>
  /// Tests the 'git log' functionality using tags
  /// </summary>
  [Test]
  public async Task GitLogCommandTestTags()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetchFrom = await gitCommandRunnerService.GitFetchAsync("11.8.1.0");
    var gitFetchTo = await gitCommandRunnerService.GitFetchAsync("11.8.2");
    var gitStatus = await gitCommandRunnerService.ExecuteGitCommandAsync("status");
    var gitLog = await gitCommandRunnerService.GitLogAsync("11.8.1.0", "11.8.2");

    // Assert
    var gitLogFailed = gitLog?.Contains("unknown revision") ?? true;
    Assert.That(gitLogFailed, Is.False);
  }
}