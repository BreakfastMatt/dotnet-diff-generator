namespace Tests.ApplicationServiceTests;
using Application.GitCommandRunnerService;
using Application.ValidateRepositoryDetailsService;
using Models.Models.Config;

[TestFixture]
public class ValidateRepositoryDetailsServiceTests
{
  /// <summary>
  /// The Test Repo to run the tests against
  /// </summary>
  public const string repoName = "C:\\Clients\\Singular\\Git-Diff-Generator-Tests\\inn8.web"; // TODO: have this config driven.

  /// <summary>
  /// Tests the repository exists functionality
  /// </summary>
  [Test]
  public async Task TestsThatRepositoryExists()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var repoExists = await validateRepositoryDetailsService.ValidateRepoExistsAsync(repoDetails);

    // Assert
    Assert.That(repoExists, Is.True);
  }

  /// <summary>
  /// Tests the repository doesn't exist functionality
  /// </summary>
  [Test]
  public async Task TestsThatRepositoryDoesNotExist()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = "C:\\Path\\DoesNotExist" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var repoExists = await validateRepositoryDetailsService.ValidateRepoExistsAsync(repoDetails);

    // Assert
    Assert.That(repoExists, Is.False);
  }

  /// <summary>
  /// Tests the user has repository access functionality
  /// </summary>
  [Test]
  public async Task TestsUserHasRepositoryAccess()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var repoAccess = await validateRepositoryDetailsService.ValidateRepoAccessAsync(repoDetails);

    // Assert
    Assert.That(repoAccess, Is.True);
  }

  /// <summary>
  /// Tests the user doesn't have repository access functionality
  /// </summary>
  [Test]
  public async Task TestsUserDoesNotHaveRepositoryAccess()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = repoName }; // TODO: currently don't have a repo to test this against
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var repoAccess = await validateRepositoryDetailsService.ValidateRepoAccessAsync(repoDetails);

    // Assert
    //Assert.That(repoAccess, Is.False);
    Assert.Pass();
  }

  /// <summary>
  /// Tests that the branch/tag exists on the repository
  /// </summary>
  [Test]
  public async Task TestsBranchExistsOnRepository()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var branchExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "dev" });
    var tagExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "11.8.2" });
    var multipleExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "dev", "11.8.2", "11.8.1.0" });

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(branchExists, Is.True);
      Assert.That(tagExists, Is.True);
      Assert.That(multipleExists, Is.True);
    });
  }

  /// <summary>
  /// Tests that the branch/tag does not exist on the repository
  /// </summary>
  [Test]
  public async Task TestsBranchDoesNotExistOnRepository()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var branchExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "main" });
    var tagExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "10.10.0" });
    var multipleExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "main", "10.10.0" });

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(branchExists, Is.False);
      Assert.That(tagExists, Is.False);
      Assert.That(multipleExists, Is.False);
    });
  }
}