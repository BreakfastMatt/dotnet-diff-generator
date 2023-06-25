using Application.DiffGenerationService;
using Application.GitCommandRunnerService;
using Models.Constants;
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
  public void TestsCommitReferenceGroupingLogic()
  {
    // Arrange
    var diffGenerationService = new DiffGenerationService(new GitCommandRunnerService());
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
      { "Key3", new List<string> { "Ref5", "Ref9" } },
      { "Key4", new List<string> { "Ref8" } },
    };
    Assert.That(groupedDiffs, Is.EquivalentTo(expected));
  }

  /// <summary>
  /// Tests the string conversion logic for the diffs.
  /// </summary>
  [Test]
  public void TestsConvertDiffsToStringLogic()
  {
    // Arrange
    var diffGenerationService = new DiffGenerationService(new GitCommandRunnerService());
    var diffsInput = new Dictionary<string, List<string>>
    {
      { "FEAT-1", new List<string> { "DEV-1", "DEV-2", "DEV-6" } },
      { "CHANGES-1", new List<string>() },
      { "FEAT-2", new List<string> { "DEV-3", "DEV-4", "DEV-7" } },
      { "DEFECT-1", new List<string>() },
      { "FEAT-3", new List<string> { "DEV-5", "DEV-9", "DEFECT-3" } },
      { "GIP-2", new List<string> { "DEV-12" } },
      { "DEFECT-2", new List<string> { "DEV-11" } },
      { "ACTION-1", new List<string> { "DEV-13" } },
      { "CHANGES-2", new List<string>{ "DEV-10" } },
      { "FEAT-4", new List<string> { "DEV-8" } },
      { "ACTION-2", new List<string>() },
      { GlobalConstants.diffCommitWithoutReference, new List<string>{"random commit without reference", "another random commit"} },
      { "GIP-1", new List<string> () },
      { "FEAT-5", new List<string> { "ACTION-3" } },
      { "DEV-14", new List<string>() },
    };

    // Act
    var diffString = diffGenerationService.ConvertDiffsToString(diffsInput);

    // Assert
    var expected = "FEAT-1\r\nDEV-1\r\nDEV-2\r\nDEV-6\r\n\r\nFEAT-2\r\nDEV-3\r\nDEV-4\r\nDEV-7\r\n\r\nFEAT-3\r\nDEV-5\r\nDEV-9\r\nDEFECT-3\r\n\r\nFEAT-4\r\nDEV-8\r\n\r\nFEAT-5\r\nACTION-3\n\r\nCHANGES-1\r\n\r\nCHANGES-2\r\nDEV-10\n\r\nDEFECT-1\r\n\r\nDEFECT-2\r\nDEV-11\n\r\nGIP-1\r\n\r\nGIP-2\r\nDEV-12\n\r\nACTION-1\r\nDEV-13\r\n\r\nACTION-2\n\r\nCommits without references:\r\nrandom commit without reference\r\nanother random commit\r\nDEV-14";
    Assert.That(diffString, Is.EqualTo(expected));
  }
}