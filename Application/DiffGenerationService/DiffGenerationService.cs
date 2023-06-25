using System.Text;
using System.Text.RegularExpressions;
using Models.Constants;
using Models.Interfaces.Config;
using Models.Interfaces.Services.DiffGenerationService;
using Models.Interfaces.Services.GitCommandRunnerService;
using Models.Models.Commit;

namespace Application.DiffGenerationService;

// TODO: note currently the logic here is a bit messy and not written in the nicest way. this will be updated :)
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
    var saveSucceeded = SaveDiffsToOutputDirectory(config.OutputPath, build, cleanedDiff);
    return saveSucceeded;
  }

  public async Task<List<Commit>> GenerateRawDiffForRepositoryAsync(IRepositoryDetails repoDetail, string fromReference, string toReference)
  {
    // Sets the main branch name for the specified repository
    var configuredFromReference = (fromReference.ToUpper().Equals("DEV") || fromReference.ToUpper().Equals("MAIN")) ? repoDetail.MainBranchName : fromReference;
    var configuredToReference = (toReference.ToUpper().Equals("DEV") || toReference.ToUpper().Equals("MAIN")) ? repoDetail.MainBranchName : toReference;

    // Generates the raw diffs for the repository
    gitCommandRunnerService.SetGitRepoDetail(repoDetail);
    var rawDiffs = await gitCommandRunnerService.GitLogAsync(configuredFromReference, configuredToReference);
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
    // Groups and clean the raw diffs
    var combinedDictionary = new Dictionary<string, List<string>>();
    foreach (var dictionary in diffsList)
    {
      // First iteration
      if (combinedDictionary.Count == 0)
      {
        // Instantiate the combinedDictionary
        combinedDictionary = dictionary;
        continue;
      }

      // Compare the dictionaries
      foreach (var keyValuePair in dictionary)
      {
        // Add the entry to the combinedDictionary if it isn't present yet
        var entryExistsInCombinedDictionary = combinedDictionary.ContainsKey(keyValuePair.Key);
        if (entryExistsInCombinedDictionary == false)
        {
          // Entry doesn't exist in the combinedDictionary, so add it
          combinedDictionary.Add(keyValuePair.Key, keyValuePair.Value);
          continue;
        }

        // Entry does exist, so check the underlying values
        var valueRetrieval = combinedDictionary.TryGetValue(keyValuePair.Key, out var currentListValues);
        var newListValues = new List<string>(currentListValues);
        newListValues.AddRange(keyValuePair.Value);
        newListValues = newListValues.Distinct().ToList();
        combinedDictionary[keyValuePair.Key] = newListValues;
      }
    }

    // Clear all entries that exist as values
    foreach (var entryKvp in combinedDictionary)
    {
      var keyExistsAtListLevel = combinedDictionary.Any(kvp => kvp.Value.Contains(entryKvp.Key));
      var entryShouldBeRemoved = keyExistsAtListLevel && entryKvp.Value.Count == 0;
      if (entryShouldBeRemoved) combinedDictionary.Remove(entryKvp.Key);
    }

    // The grouped diffs with distinct entries
    return combinedDictionary;
  }

  public string ConvertDiffsToString(Dictionary<string, List<string>> groupedDiffs)
  {
    // Group the diffs into the order in which they'll appear in the final string
    var stringbuilder = new StringBuilder();
    var features = groupedDiffs.Where(diff => diff.Key.Contains("FEAT-")).OrderBy(entry => entry.Key).ToDictionary(key => key.Key, value => value.Value);
    var changes = groupedDiffs.Where(diff => diff.Key.Contains("CHANGES-")).OrderBy(entry => entry.Key).ToDictionary(key => key.Key, value => value.Value);
    var defects = groupedDiffs.Where(diff => diff.Key.Contains("DEFECT-")).OrderBy(entry => entry.Key).ToDictionary(key => key.Key, value => value.Value);
    var gips = groupedDiffs.Where(diff => diff.Key.Contains("GIP-")).OrderBy(entry => entry.Key).ToDictionary(key => key.Key, value => value.Value);
    var actions = groupedDiffs.Where(diff => diff.Key.Contains("ACTION-")).OrderBy(entry => entry.Key).ToDictionary(key => key.Key, value => value.Value);
    var improperCommits = groupedDiffs.Where(diff => diff.Key.Contains("DEV-") || diff.Key.Contains(GlobalConstants.diffCommitWithoutReference)).OrderBy(entry => entry.Key).ToDictionary(key => key.Key, value => value.Value);

    // Build the feature section
    var featureSection = ConvertDiffSectionToString(features);
    if (!string.IsNullOrEmpty(featureSection)) stringbuilder.AppendLine(featureSection);

    // Build the changes section
    var changesSection = ConvertDiffSectionToString(changes);
    if (!string.IsNullOrEmpty(changesSection)) stringbuilder.AppendLine(changesSection);

    // Build the defects section
    var defectSection = ConvertDiffSectionToString(defects);
    if (!string.IsNullOrEmpty(defectSection)) stringbuilder.AppendLine(defectSection);

    // Build the gip section
    var gipSection = ConvertDiffSectionToString(gips);
    if (!string.IsNullOrEmpty(gipSection)) stringbuilder.AppendLine(gipSection);

    // Build the action section
    var actionsSection = ConvertDiffSectionToString(actions);
    if (!string.IsNullOrEmpty(actionsSection)) stringbuilder.AppendLine(actionsSection);

    // Build the action section
    var improperCommitsSection = ConvertDiffSectionToString(improperCommits, true);
    if (!string.IsNullOrEmpty(improperCommitsSection)) stringbuilder.AppendLine(improperCommitsSection);

    // Builds the string
    var stringDiff = stringbuilder.ToString().Trim();
    return stringDiff;
  }

  private static string ConvertDiffSectionToString(Dictionary<string, List<string>> diffSection, bool nonStandard = false)
  {
    // Converts the diff section to a string
    var stringbuilder = new StringBuilder();
    if (nonStandard) stringbuilder.AppendLine("Commits without references:");
    foreach (var keyValuePair in diffSection)
    {
      // Adds the epic link
      var referencelessCommit = nonStandard && keyValuePair.Key == GlobalConstants.diffCommitWithoutReference;
      if (!referencelessCommit) stringbuilder.AppendLine(keyValuePair.Key);

      // Adds the tickets linked to the epic (if applicable)
      foreach (var value in keyValuePair.Value)
      {
        // The tickets linked to the epic
        stringbuilder.AppendLine(value);
      }
      // Adds blank line separator
      if (!nonStandard) stringbuilder.AppendLine();
    }

    // Builds the diffSection to string
    var diffSectionString = stringbuilder.ToString().Trim();
    if (string.IsNullOrEmpty(diffSectionString)) return string.Empty;
    diffSectionString += "\n";
    return diffSectionString;
  }

  public bool SaveDiffsToOutputDirectory(string path, string build, string diffs)
  {
    try
    {
      // Create directory if it doesn't exist yet
      var buildFolderPath = $"{path}\\{build}";
      var buildFolderExists = Directory.Exists(buildFolderPath);
      if (!buildFolderExists) Directory.CreateDirectory(buildFolderPath);

      // Save the file
      var textFilePath = $"{buildFolderPath}\\cleaned_diff.txt";
      File.WriteAllText(textFilePath, diffs);
      Console.WriteLine("File saved successfully.");
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine("Failed to save the diffs, please verify that the save path exists");
      return false;
    }
  }
}