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
        var commitDetail = new Commit
        {
          Author = diffDetail?.FirstOrDefault() ?? string.Empty,
          CommitMessage = diffDetail?.LastOrDefault() ?? string.Empty,
        };
        return commitDetail;
      })
      ?.ToList();
    return commitDetails;
  }

  //public List<Commit> ExtractCommitReferences(List<Commit> commits)
  //{
  //  // Initialises the epic dictionary
  //  var epicDictionary = new Dictionary<string, List<string>>();
  //  epicDictionary.Add(GlobalConstants.diffCommitWithoutReference, new List<string>());

  //  void AddReferencesToDictionary(string epicMatch, MatchCollection referenceMatches, bool standard = true)
  //  {
  //    var hasValues = epicDictionary.TryGetValue(epicMatch, out var existingValues);
  //    var matches = referenceMatches?.Select(match => match.Value)?.Where(match => existingValues?.Contains(match) ?? true)?.ToList() ?? new List<string>();
  //    if (hasValues) epicDictionary[epicMatch].AddRange(matches);
  //  };

  //  var mappedCommits = commits
  //    .Select(commitDetail =>
  //    {
  //      // Grabs the epicMatch if present
  //      var commitMessage = commitDetail.CommitMessage.ToUpper();
  //      var epicMatch = Regex.Match(commitMessage, @"\b(?:FEAT)-\d+\b")?.Value;
  //      epicMatch = string.IsNullOrEmpty(epicMatch) ? GlobalConstants.diffEpicNotCaptured : epicMatch;
  //      epicDictionary.TryAdd(epicMatch, new List<string>());
  //      var hasEpic = !epicMatch.Equals(GlobalConstants.diffEpicNotCaptured);

  //      // Checks the Standard Matches
  //      var standardMatches = Regex.Matches(commitMessage, @"\b(?:DEV|DEFECT|CHANGES|GIP|ACTION)-\d+\b");
  //      AddReferencesToDictionary(epicMatch, standardMatches);

  //      // Checks the Non-standard Matches
  //      var nonStandardMatches = Regex.Matches(commitMessage, @"(?i)\b(?:feat|dev|defect|changes|gip|action)\d+\b");
  //      AddReferencesToDictionary(GlobalConstants.diffCommitWithoutReference, nonStandardMatches, false);

  //      // Adds commits that have no references to list
  //      var hasNoReferences = standardMatches.Count == 0 && nonStandardMatches.Count == 0;
  //      if (hasNoReferences) epicDictionary[GlobalConstants.diffCommitWithoutReference].Add($"{commitDetail.Author} - {commitMessage}");
  //      return commitDetail;
  //    })
  //    .ToList();
  //  return mappedCommits;
  //}


  public List<Commit> ExtractCommitReferences(List<Commit> commits)
  {
    // TODO: neaten this function uo a bit (has a bit too much spaghet :P)
    // TODO: add logic to ensure the hyphen is present in the reference (standardise the references) 
    // TODO: can create a helper functino for this (needs to run for both the standardmatchesresults and epicMatch

    var referencesDictionary = new Dictionary<string, List<string>>();
    foreach (var commitDetail in commits)
    {
      // Epic Match
      var commitMessage = commitDetail.CommitMessage.ToUpper();
      var epicMatch = Regex.Match(commitMessage, @"\b(?:FEAT)-?\d+\b")?.Value;
      if (!string.IsNullOrEmpty(epicMatch)) referencesDictionary.TryAdd(epicMatch, new List<string>());

      // Iterate through matches
      var standardMatches = Regex.Matches(commitMessage, @"\b(?:DEV|DEFECT|CHANGES|GIP|ACTION)-?\d+\b");
      var matches = standardMatches?.Select(match => match.Value)?.ToList() ?? new List<string>();

      // Add matches to the Epic Dictionary
      if (!string.IsNullOrEmpty(epicMatch) && referencesDictionary.TryGetValue(epicMatch, out var existingValues))
      {
        // Matches linked to a Feature
        var filteredMatches = matches?.Where(match => !(existingValues?.Contains(match) ?? true))?.ToList();
        referencesDictionary[epicMatch].AddRange(filteredMatches);
        continue;
      }

      // Matches not linked to a feature
      if (standardMatches.Count > 0)
      {
        foreach (Match test in standardMatches)
        {
          referencesDictionary.TryAdd(test.Value, new List<string>());
        }
        continue;
      }
      else
      {
        // Add commits without refences
        if (referencesDictionary.TryAdd(GlobalConstants.diffCommitWithoutReference, new List<string> { commitMessage }))
        {
          referencesDictionary.TryGetValue(GlobalConstants.diffCommitWithoutReference, out var currentValue);
          currentValue.Add(commitMessage);
        }
        continue;
      }
    }
    return commits;
  }

  public async Task<bool> GenerateRepositoryDiffsAsync(IRepositoryDetails repoDetail, string build, string fromReference, string toReference)
  {
    throw new NotImplementedException();
  }

  public string GroupDiffsForRepository(List<Commit> commits)
  {
    throw new NotImplementedException();
  }
}