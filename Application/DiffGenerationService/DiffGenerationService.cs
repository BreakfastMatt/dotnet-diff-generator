using System.Text.RegularExpressions;
using Models.Constants;
using Models.Interfaces.Config;
using Models.Interfaces.Services.DiffGenerationService;
using Models.Interfaces.Services.GitCommandRunnerService;
using Models.Models.Commit;

namespace Application.DiffGenerationService;

public class DiffGenerationService : IDiffGenerationService
{
  // Inject relevant services
  private readonly IGitCommandRunnerService gitCommandRunnerService;
  public DiffGenerationService(IGitCommandRunnerService gitCommandRunnerService)
  {
    this.gitCommandRunnerService = gitCommandRunnerService;
  }

  public async Task<bool> GenerateRepositoryDiffsAsync(IConfig config, string build, string fromReference, string toReference)
  {
    // Generate & clean diffs for repository
    var diffList = new List<Dictionary<string, List<string>>>();
    foreach (var repository in config.RepositoryDetails)
    {
      var rawDiff = await GenerateRawDiffForRepositoryAsync(repository, fromReference, toReference);
      var diffReferences = ExtractCommitReferences(rawDiff);
      diffList.Add(diffReferences);
    }

    // Convert the diffList to a string and save it to file
    var groupedDiffs = GroupRawDiffs(diffList);
    var cleanedDiff = ConvertDiffsToString(groupedDiffs);

    // Save the cleaned diffs to the user specified build path
    var saveSucceeded = SaveDiffsToOutputDirectory(build, cleanedDiff);
    return saveSucceeded;
  }

  public async Task<List<Commit>> GenerateRawDiffForRepositoryAsync(IRepositoryDetails repoDetail, string fromReference, string toReference)
  {
    // Generates the raw diffs for the repository
    gitCommandRunnerService.SetGitRepoDetail(repoDetail);
    var rawDiffs = await gitCommandRunnerService.GitLogAsync(fromReference, toReference);
    var commitDetails = rawDiffs
      ?.Split(GlobalConstants.gitLogDiffSeparator)
      ?.Select(rawDiffLine =>
      {
        // Grabs the detail of the commit
        var diffDetail = rawDiffLine?.Split(GlobalConstants.gitLogDelimiter);
        if (diffDetail == null || diffDetail.Length != 3) return new Commit();
        var hasDateSpecified = DateTime.TryParse(diffDetail?[2], out var commitDate);
        var commitDetail = new Commit
        {
          Author = diffDetail?[0] ?? string.Empty,
          CommitMessage = diffDetail?[1] ?? string.Empty,
          CommitDate = hasDateSpecified ? commitDate : DateTime.MinValue
        };
        return commitDetail;
      })
      ?.ToList();
    return commitDetails;
  }

  public Dictionary<string, List<string>> ExtractCommitReferences(List<Commit> commits)
  {
    // Setup & Order the commits
    var referencesDictionary = new Dictionary<string, List<string>>();
    var allCommits = new List<string>();
    var sortedCommits = commits.Where(commit => commit.CommitDate != DateTime.MinValue).OrderBy(commit => commit.CommitDate).ToList();

    // Standardise the commit references
    string StandardiseCommitReference(string? reference)
    {
      // Short-Circuit
      if (string.IsNullOrEmpty(reference)) return string.Empty;

      // Checks if the commit reference was done to standard
      var standardised = Regex.Match(reference, @"\b(?:FEAT|DEV|DEFECT|CHANGES|GIP|ACTION)-\d+\b");
      if (standardised.Success) return reference;

      // Otherwise standardises the commit
      var standardisedReference = Regex.Replace(reference, @"(?i)\b(FEAT|DEV|DEFECT|CHANGES|GIP|ACTION)-?(\d+)\b", "$1-$2");
      return standardisedReference;
    }

    foreach (var commitDetail in sortedCommits)
    {
      // Identify Matches
      var commitMessage = commitDetail.CommitMessage.ToUpper();
      var epicMatch = Regex.Match(commitMessage, @"\b(?:FEAT|CHANGES|GIP)-?\d+\b")?.Value;
      epicMatch = StandardiseCommitReference(epicMatch);
      var standardMatches = Regex.Matches(commitMessage, @"\b(?:DEV|DEFECT|ACTION)-?\d+\b");
      var matches = standardMatches?.Select(match => StandardiseCommitReference(match.Value))?.ToList() ?? new List<string>();

      // Identify PRs that were reverted and remove the references from the referencesDictionary
      var pullRequestWasReverted = (commitMessage.Contains("REVERT") && commitMessage.Contains("PULL REQUEST"));
      if (pullRequestWasReverted)
      {
        // Checks if the commit that was reverted was in the list
        var commitIsInList = allCommits.RemoveAll(commitRef => commitMessage.Contains(commitRef)) > 0;
        if (commitIsInList == false) continue;

        // Remove the reverted commit references from the dictionary
        if (referencesDictionary.ContainsKey(epicMatch) && referencesDictionary[epicMatch].Count == 0) referencesDictionary.Remove(epicMatch);
        foreach (var match in matches)
        {
          // Remove underlying references
          foreach (var entry in referencesDictionary)
          {
            var entireEntryShouldBeRemoved = entry.Key == match && entry.Value.Contains(match);
            if (entireEntryShouldBeRemoved) referencesDictionary.Remove(entry.Key);
            else if (entry.Value.Contains(match)) entry.Value.RemoveAll(dictionaryValue => dictionaryValue?.Equals(match) ?? false);
          }
        }
        if (referencesDictionary.ContainsKey(epicMatch) && referencesDictionary[epicMatch].Count == 0) referencesDictionary.Remove(epicMatch);
        continue;
      }

      // Epic Match
      if (!string.IsNullOrEmpty(epicMatch))
      {
        // Add the commits to the dictionary / allCommits list
        referencesDictionary.TryAdd(epicMatch, new List<string>());
        allCommits.Add(epicMatch);
      }

      // Add matches to the Epic Dictionary
      if (!string.IsNullOrEmpty(epicMatch) && referencesDictionary.TryGetValue(epicMatch, out var existingValues))
      {
        // Matches linked to a Feature
        var filteredMatches = matches?.Where(match => !(existingValues?.Contains(match) ?? true))?.ToList();
        referencesDictionary[epicMatch].AddRange(filteredMatches);
        allCommits.AddRange(matches);
        continue;
      }

      // Matches not linked to a feature
      if (standardMatches.Count > 0)
      {
        foreach (Match test in standardMatches)
        {
          referencesDictionary.TryAdd(test.Value, new List<string>());
          allCommits.AddRange(matches);
        }
        continue;
      }

      // Add commits without refences
      if (referencesDictionary.TryAdd(GlobalConstants.diffCommitWithoutReference, new List<string> { commitMessage }))
      {
        referencesDictionary.TryGetValue(GlobalConstants.diffCommitWithoutReference, out var currentValue);
        if (!currentValue.Contains(commitMessage)) currentValue.Add(commitMessage);
      }
      continue;
    }
    return referencesDictionary;
  }

  public Dictionary<string, List<string>> GroupRawDiffs(List<Dictionary<string, List<string>>> diffsList)
  {
    throw new NotImplementedException();
  }

  public string ConvertDiffsToString(Dictionary<string, List<string>> groupedDiffs)
  {
    throw new NotImplementedException();
  }

  public bool SaveDiffsToOutputDirectory(string build, string diffs) // TODO: consider .csv logic (can add this later)
  {
    // TODO: validate build driectory
    // TODO: create the build directory if not present
    // TODO: save the diffs to the build
    throw new NotImplementedException();
  }
}