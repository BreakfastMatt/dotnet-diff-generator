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
    // Store the working changes
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Store the working changes
    var gitStashCommand = "stash save \"Git-Diff-Startup\"";
    var stashSaveOutput = gitCommandRunnerService.ExecuteGitCommand(gitStashCommand);
    if (stashSaveOutput != null & stashSaveOutput.Contains("No local changes"))
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
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
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
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
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
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
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
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
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
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var gitFetch = gitCommandRunnerService.GitFetch("main");

    // Assert
    Assert.That(gitFetch != null && gitFetch.Contains("branch") && gitFetch.Contains("main") && gitFetch.Contains("FETCH_HEAD"), Is.True);
  }

  /// <summary>
  /// Tests the 'git pull' functionality
  /// </summary>
  [Test]
  public void GitPullCommandTest()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator", Path = "D:\\Documents\\Programming Projects\\React Native\\Learning\\HelloWorldProject" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    gitCommandRunnerService.ExecuteGitCommand("checkout main");
    var gitPull = gitCommandRunnerService.GitPull("main");

    // Assert
    Assert.That(gitPull != null && (gitPull.Contains("Already up to date") || gitPull.Contains("Updating")));
  }
}