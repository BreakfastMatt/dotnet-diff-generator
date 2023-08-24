namespace Application.Tests.ValidateRepositoryDetailsServiceTests;
using Application.GitCommandRunnerService;
using Application.ValidateRepositoryDetailsService;
using Models.Models.Config;

public class ValidateRepositoryDetailsServiceTests
{
  /// <summary>
  /// Tests the repository exists functionality
  /// </summary>
  [Fact]
  public async Task ValidateRepoExistsAsync_ForExistingTestRepository_ShouldReturnTrue()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var repoExists = await validateRepositoryDetailsService.ValidateRepoExistsAsync(repoDetails);

    // Assert
    Assert.True(repoExists);
  }

  /// <summary>
  /// Tests the repository doesn't exist functionality
  /// </summary>
  [Fact]
  public async Task ValidateRepoExistsAsync_ForNonExistingTestRepository_ShouldReturnFalse()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = "C:\\Path\\DoesNotExist" };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var repoExists = await validateRepositoryDetailsService.ValidateRepoExistsAsync(repoDetails);

    // Assert
    Assert.False(repoExists);
  }

  /// <summary>
  /// Tests the user has repository access functionality
  /// </summary>
  [Fact]
  public async Task ValidateRepoAccessAsync_ForTestRepositoryWithValidAccess_ShouldReturnTrue()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var repoAccess = await validateRepositoryDetailsService.ValidateRepoAccessAsync(repoDetails);

    // Assert
    Assert.True(repoAccess);
  }

  /// <summary>
  /// Tests the user doesn't have repository access functionality
  /// </summary>
  [Fact]
  public async Task ValidateRepoAccessAsync_ForTestRepositoryWithInvalidAccess_ShouldReturnFalse()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = Constants.TestRepositoryName }; // TODO: test repo that people won't have access to
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var repoAccess = await validateRepositoryDetailsService.ValidateRepoAccessAsync(repoDetails);

    // Assert
    Assert.False(repoAccess);
  }

  /// <summary>
  /// Tests that the branch/tag exists on the repository
  /// </summary>
  [Fact]
  public async Task ValidateBranchExistenceAsync_ForBranchesAndTagsThatExistOnTestRepository_ShouldReturnTrue()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var branchExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "dev" });
    var tagExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "11.8.2" });
    var multipleExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "dev", "11.8.2", "11.8.1.0" });

    // Assert
    Assert.Multiple(() =>
    {
      Assert.True(branchExists);
      Assert.True(tagExists);
      Assert.True(multipleExists);
    });
  }

  /// <summary>
  /// Tests that the branch/tag does not exist on the repository
  /// </summary>
  [Fact]
  public async Task ValidateBranchExistenceAsync_ForBranchesAndTagsThatDoNotExistOnTestRepository_ShouldReturnFalse()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var validateRepositoryDetailsService = new ValidateRepositoryDetailsService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "RepoTest", Path = Constants.TestRepositoryName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    var branchExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "main" });
    var tagExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "10.10.0" });
    var multipleExists = await validateRepositoryDetailsService.ValidateBranchExistenceAsync(repoDetails, new List<string> { "main", "10.10.0" });

    // Assert
    Assert.Multiple(() =>
    {
      Assert.False(branchExists);
      Assert.False(tagExists);
      Assert.False(multipleExists);
    });
  }
}