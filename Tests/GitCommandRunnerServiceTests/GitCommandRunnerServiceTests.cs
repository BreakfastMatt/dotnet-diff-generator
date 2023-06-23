using Application.GitCommandRunnerService;
using Models.Models.Config;

namespace Tests.ApplicationServiceTests;

[TestFixture]
// TODO: note a mock repo needs to be setup to test against here (will add a config.json file to the test project to allow this to be configured by the user)
public class GitCommandRunnerServiceTests
{
  /// <summary>
  /// Initialises the GitCommandRunnerService Test Fixture on startup.
  /// </summary>
  [OneTimeSetUp]
  public void InitialiseGitCommandRunnerServiceTestFixture()
  {
    // Check if there are any changes in the repo
    // TODO:

    // Stash the changes for the duration of the unit tests
    // TODO:
  }

  /// <summary>
  /// Cleanup function to be run after every GitCommandRunnerService Unit Test.
  /// </summary>
  [TearDown]
  public void GitCommandRunnerServiceTestCleanup()
  {
    // TODO: potentially do something like restore a git stash here. (might not need this)
  }

  /// <summary>
  /// Tests GitCommandRunnerService using the basic "git status" command.
  /// </summary>
  [Test]
  public void ExecuteGitStatusCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = "D:\\Documents\\Programming Projects\\Git-Diff-Generator" };
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
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = "D:\\Documents\\Programming Projects\\Git-Diff-Generator" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var output = gitCommandRunnerService.CheckWorkingTreeForOutstandingChanges();

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
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitStatusBefore = gitCommandRunnerService.ExecuteGitCommand("status");
    var gitStashSave = gitCommandRunnerService.GitStashSave();
    var gitRestore = gitCommandRunnerService.GitStashPop();
    var gitStatusAfter = gitCommandRunnerService.ExecuteGitCommand("status");

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(gitStashSave?.Contains("Saved working directory") ?? false, Is.True);
      Assert.That(gitRestore?.Contains("Dropped stash") ?? false, Is.True);
      Assert.That(gitStatusBefore, Is.EqualTo(gitStatusAfter));
    });
  }

  [Test]
  public void GitFetchCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetch = gitCommandRunnerService.GitFetch("main");

    // Assert
    Assert.That(gitFetch != null && gitFetch.Contains("branch") && gitFetch.Contains("main") && gitFetch.Contains("FETCH_HEAD"), Is.True);
  }
}