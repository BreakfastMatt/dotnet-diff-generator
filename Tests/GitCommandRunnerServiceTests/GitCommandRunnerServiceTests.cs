using Application.GitCommandRunnerService;
using Models.Models.Config;

namespace Tests.ServiceTests;

[TestFixture]
public class GitCommandRunnerServiceTests
{
  /// <summary>
  /// Initialises the GitCommandRunnerService Test Fixture on startup.
  /// </summary>
  [OneTimeSetUp]
  public void InitialiseGitCommandRunnerServiceTestFixture()
  {
    // Check if there are any changes in the repo
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = "D:\\Documents\\Programming Projects\\Git-Diff-Generator" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Stash the changes for the duration of the unit tests
    // TODO:
  }

  /// <summary>
  /// Cleanup function to be run after every GitCommandRunnerService Unit Test.
  /// </summary>
  [TearDown]
  public void GitCommandRunnerServiceTestCleanup()
  {
    // Restore the git stash 
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
  /// Tests if there are any outstanding changes on the repository using the GitCheckWorkingTree function.
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
}