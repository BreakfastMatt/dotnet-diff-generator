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
  public void InitialiseGitCommandRunnerServiceTestFixture()
  {
    // Store the working changes
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Store the working changes
    var gitStashCommand = "stash save \"Git-Diff-Startup\"";
    var stashSaveOutput = gitCommandRunnerService.ExecuteGitCommand(gitStashCommand);
    if (stashSaveOutput != null && stashSaveOutput.Contains("No local changes"))
    {
      gitCommandRunnerService.ExecuteGitCommand("stash pop 0");
      gitCommandRunnerService.ExecuteGitCommand(gitStashCommand);
    }
  }

  /// <summary>
  /// Cleanups the GitCommandRunnerService Test Fixture on exit.
  /// </summary>
  [OneTimeTearDown]
  public void CleanupGitCommandRunnerServiceTestFixture()
  {
    // Store the working changes
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);
    gitCommandRunnerService.ExecuteGitCommand("stash pop 0");
  }

  /// <summary>
  /// Tests GitCommandRunnerService using the basic "git status" command.
  /// </summary>
  [Test]
  public void ExecuteGitStatusCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);
    var gitCommand = "status";

    // Act
    var output = gitCommandRunnerService.ExecuteGitCommand(gitCommand);

    // Assert
    Assert.That(output?.Contains("On branch") ?? false, Is.True);
  }

  /// <summary>
  /// Tests if there are any outstanding changes on the repository using the CheckWorkingTreeForOutstandingChanges function.
  /// </summary>
  [Test]
  public void CheckWorkingChangesTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    gitCommandRunnerService.ExecuteGitCommand("stash pop 0"); // Bring Back Outstanding Changes
    var output = gitCommandRunnerService.CheckWorkingTreeForOutstandingChanges(); // Check for Outstanding Changes
    gitCommandRunnerService.ExecuteGitCommand("stash save \"Git-Diff-Startup\""); // Save changes back to stash

    // Assert
    Assert.That(output, Is.True);
  }

  /// <summary>
  /// Tests the git stash save and apply functions
  /// </summary>
  [Test]
  public void GitStashSaveAndRestoreTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    gitCommandRunnerService.ExecuteGitCommand("stash pop 0"); // Bring Back Outstanding Changes
    var gitStatusBefore = gitCommandRunnerService.ExecuteGitCommand("status"); // Check before status
    var gitStashSave = gitCommandRunnerService.GitStashSave(); // Stash current 
    var gitRestore = gitCommandRunnerService.GitStashPop(); // Pop current
    var gitStatusAfter = gitCommandRunnerService.ExecuteGitCommand("status"); // Check after status
    gitCommandRunnerService.ExecuteGitCommand("stash save \"Git-Diff-Startup\""); // Restore Outstanding Changes

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
  public void GitFetchCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetch = gitCommandRunnerService.GitFetch(branch);

    // Assert
    Assert.That(gitFetch != null && gitFetch.Contains("branch") && gitFetch.Contains(branch) && gitFetch.Contains("FETCH_HEAD"), Is.True);
  }

  /// <summary>
  /// Tets the 'git checkout' functionality
  /// </summary>
  [Test]
  public void GitCheckoutCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitCheckout = gitCommandRunnerService.GitCheckout(branch);

    // Assert
    Assert.That(gitCheckout != null && gitCheckout.Contains($"origin/{branch}"), Is.True);
  }

  /// <summary>
  /// Tests the 'git pull' functionality
  /// </summary>
  [Test]
  public void GitPullCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    gitCommandRunnerService.ExecuteGitCommand($"checkout {branch}");
    var gitPull = gitCommandRunnerService.GitPull(branch);

    // Assert
    Assert.That(gitPull != null && (gitPull.Contains("Already up to date") || gitPull.Contains("Updating")));
  }

  /// <summary>
  /// Tests the 'git log' functionality
  /// </summary>
  [Test]
  public async Task GitLogCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetchFrom = gitCommandRunnerService.GitFetch("11.8.1_hotfix_15052023");
    var gitFetchTo = gitCommandRunnerService.GitFetch("11.8.2_hotfix_05062023");
    var gitStatus = gitCommandRunnerService.ExecuteGitCommand("status");
    var gitLogTask = gitCommandRunnerService.GitLogAsync("11.8.1_hotfix_15052023", "11.8.2_hotfix_05062023");
    var gitLog = await gitLogTask.ConfigureAwait(false);

    // Assert
    Assert.Pass(); // TODO: temporary
  }
}