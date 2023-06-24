using Application.DiffGenerationService;
using Application.GitCommandRunnerService;
using Models.Models.Config;

namespace Tests.ApplicationServiceTests;

[TestFixture]
public class DiffGenerationServiceTests
{
  /// <summary>
  /// The Test Repo to run the tests against
  /// </summary>
  public const string repoName = "C:\\Clients\\Singular\\Git-Diff-Generator-Tests\\inn8.web"; // TODO: have this config driven.

  /// <summary>
  /// Tests the raw diff generation logic.
  /// </summary>
  [Test]
  public async Task TestsRawDiffGeneration()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var diffGenerationService = new DiffGenerationService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    await gitCommandRunnerService.GitFetchAsync("11.8.1.0");
    await gitCommandRunnerService.GitFetchAsync("11.8.2");
    var diffOutput = await diffGenerationService.GenerateRawDiffForRepositoryAsync(repoDetails, "11.8.1.0", "11.8.2");

    // Assert
    Assert.That(diffOutput, Is.Not.EqualTo(null));
  }

  /// <summary>
  /// Tests the raw diff generation logic JRIA reference extraction logic
  /// </summary>
  [Test]
  public async Task TestsCommitReferenceExtraction()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var diffGenerationService = new DiffGenerationService(gitCommandRunnerService);
    var repoDetails = new RepositoryDetails { Name = "Git-Diff-Generator-Test", Path = repoName };
    gitCommandRunnerService.SetGitRepoDetail(repoDetails);

    // Act
    await gitCommandRunnerService.GitFetchAsync("11.8.1.0");
    await gitCommandRunnerService.GitFetchAsync("11.8.2");
    var diffOutput = await diffGenerationService.GenerateRawDiffForRepositoryAsync(repoDetails, "11.8.1.0", "11.8.2");
    var references = diffGenerationService.ExtractCommitReferences(diffOutput);

    // Assert
    Assert.That(references, Is.Not.EqualTo(null));
  }
}