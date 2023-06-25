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

  /// <summary>
  /// Tests the basics of the diff grouping functionality
  /// </summary>
  [Test]
  public async Task TestsCommitReferenceGroupingLogic()
  {
    // Arrange
    var gitCommandRunnerService = new GitCommandRunnerService();
    var diffGenerationService = new DiffGenerationService(gitCommandRunnerService);
    var diffsRepo1 = new Dictionary<string, List<string>>
    {
      { "Key1", new List<string> { "Ref1", "Ref2" } },
      { "Key2", new List<string> { "Ref3", "Ref4" } },
      { "Key3", new List<string> { "Ref5" } }
    };
    var diffsRepo2 = new Dictionary<string, List<string>>
    {
      { "Key1", new List<string> { "Ref1", "Ref6" } }, // Only Ref6 should be added
      { "Key2", new List<string> { "Ref3", "Ref4", "Ref7" } }, // Only Ref7 should be added
      { "Key3", new List<string>{ "Ref9" } }, // Ref9 should be added
      { "Key4", new List<string> { "Ref8" } }, // New entry shuld be added
      { "Ref1", new List<string>() }, // Entry should be removed (exists at list-level)
    };

    // Act
    var groupedDiffs = diffGenerationService.GroupRawDiffs(new List<Dictionary<string, List<string>>> { diffsRepo1, diffsRepo2 });

    // Assert
    var expected = new Dictionary<string, List<string>>
    {
      { "Key1", new List<string> { "Ref1", "Ref2", "Ref6" } },
      { "Key2", new List<string> { "Ref3", "Ref4", "Ref7" } },
      { "Key3", new List<string> { "Ref9" } },
      { "Key4", new List<string> { "Ref8" } },
    };
    Assert.That(groupedDiffs, Is.EquivalentTo(expected));
  }
}

//var diffsRepo1 = new Dictionary<string, List<string>>()
//{
//  {"CommitsWithoutReferences", new List<string>{"LOCAL" } },
//  {"FEAT-1003", new List<string>{ "DEV-3418" } },
//  {"FEAT-779", new List<string>{ "DEV-3632", "DEFECT-3614", "DEV-3632" } },
//  {"FEAT-702", new List<string>{ "DEFECT-3629", "DEV-3681", "DEFECT-3673" } },
//  {"CHANGES-937", new List<string>{ "DEV-3722" } },
//  {"FEAT-7798", new List<string>{ "DEFECT-3632" } },
//  {"DEFECT-3632", new List<string>() }, // Duplicate entry (exists linked to a feature, so should be removed)
//  {"FEAT-997", new List<string>{ "DEFECT-3637", "DEFECT-3644", "DEFECT-3622" }},
//  {"FEAT-692", new List<string>{ "DEV-3191", "DEFECT-3616" } },
//  {"DEFECT-3636", new List<string>()},
//  {"GIP-12746",new List<string>()},
//  {"DEFECT-3630", new List<string>() },
//  {"DEFECT-3672", new List<string>()},
//  {"DEFECT-3646", new List<string>()},
//  {"DEFECT-3675", new List<string>() },
//  {"DEV-3418", new List<string>() }, // Duplicate entry (exists linked to a feature, so should be removed)
//  {"GIP-12456", new List<string>() },
//  {"DEFECT-3676", new List<string>() },
//  {"DEFECT-3715", new List<string>() },
//  {"DEFECT-3547", new List<string>() }
//};
//var diffsRepo2 = new Dictionary<string, List<string>>()
//{
//  {"CommitsWithoutReferences", new List<string>{"Some other random commit" } }, // Combine with the above entry
//  {"FEAT-1003", new List<string>{ "DEV-3418", "DEV-9999" } }, // Add unique tickets to the existing entry
//  {"FEAT-806", new List<string>{ "DEV-3190" } } // New entry, should be added
//};