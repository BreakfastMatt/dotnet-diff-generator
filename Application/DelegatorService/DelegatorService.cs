using Models.Interfaces.Services.DelegatorService;
using Models.Interfaces.Services.GitCommandRunnerService;
using Models.Interfaces.Services.PromptUserInputService;
using Models.Interfaces.Services.ReadFromConfigService;
using Models.Interfaces.Services.ValidateRepositoryDetailsService;
using Models.Models.Config;

namespace Application.DelegatorService;

public class DelegatorService : IDelegatorService
{
  // Short-Circuiting logic
  private bool shortCircuitValidation = false;
  private bool shortCircuitRepoFetch = false;
  private bool shortCircuitDiffGeneration = false;
  private bool shortCircuitDiffClean = false;

  // Diffs
  private string rawDiffs;
  private string cleanDiffs;

  // Inject relevant services
  private readonly IReadFromConfigService readFromConfigService;
  private readonly IPromptUserInputService promptUserInputService;
  private readonly IValidateRepositoryDetailsService validateRepositoryDetailsService;
  private readonly IGitCommandRunnerService gitCommandRunnerService;
  public DelegatorService(IReadFromConfigService readFromConfigService, IPromptUserInputService promptUserInputService, IValidateRepositoryDetailsService validateRepositoryDetailsService, IGitCommandRunnerService gitCommandRunnerService)
  {
    this.readFromConfigService = readFromConfigService;
    this.promptUserInputService = promptUserInputService;
    this.validateRepositoryDetailsService = validateRepositoryDetailsService;
    this.gitCommandRunnerService = gitCommandRunnerService;
  }

  private void ResetShortCircuitingIndicators(List<string> currentNames, List<string> previousNames)
  {
    // Resets the short-circuit indicators
    var firstIteration = (previousNames?.Count ?? 0) == 0;
    var detailsHaveChanged = !firstIteration && !currentNames.SequenceEqual(previousNames);
    if (firstIteration || detailsHaveChanged)
    {
      shortCircuitValidation = false;
      shortCircuitRepoFetch = false;
      shortCircuitDiffGeneration = false;
      shortCircuitDiffClean = false;
    }
  }

  public async Task DelegateAsync()
  {
    // Arrange & Parse Config.json
    var @continue = true;
    var previousNames = new List<string>();
    var config = readFromConfigService.ReadFromConfig();

    while (@continue)
    {
      // Calls the relevant services required to generate the diffs
      var branchTagNames = promptUserInputService.PromptBranchOrTagNames();
      var names = new List<string> { branchTagNames.Item1, branchTagNames.Item2 };
      ResetShortCircuitingIndicators(names, previousNames);
      var response = await CallServicesAsync(config, names);

      // Prompt the user to restart the process if it failed
      if (response == false)
      {
        // Keep track of the users input
        previousNames.Clear();
        previousNames.AddRange(names);

        // Allow the user to restart the process
        Console.WriteLine("\nPress enter to restart...");
        var userInput = Console.ReadKey();
        @continue = userInput.Key == ConsoleKey.Enter;
        continue;
      }

      // Prompt the user to exit
      Console.WriteLine("\nPress any key to exit...");
      Console.ReadKey();
      break;
    }
  }

  public async Task<bool> CallServicesAsync(IConfig config, List<string> names)
  {
    // 1) Validate repository details
    var validationSucceeded = await RunValidationAsync(config, names);

    // 2) Fetch the latest changes for the specified branches/tags
    var fetchSucceeded = validationSucceeded && await FetchLatestChangesForRepositoriesAsync(config, names);
    var build = fetchSucceeded ? promptUserInputService.PromptBuildName() : null;

    // 3) Generate the raw diffs for the specified branches/tags
    var diffSucceeded = fetchSucceeded && await GenerateRawDiffsForRepositoriesAsync(config, names);

    // 4) Cleanup & Group the diffs
    var diffCleaningSucceeded = diffSucceeded && await CleanAndGroupDiffsAsync(this.rawDiffs);

    // 5) Save the cleaned diff to the configured path
    var diffSaveSucceeded = diffCleaningSucceeded && await SaveDiffsToOutputAsync(build, this.cleanDiffs);
    return diffSaveSucceeded;
  }

  private async Task<bool> RunValidationAsync(IConfig config, List<string> names)
  {
    // Short-Circuting Logic
    if (shortCircuitValidation)
    {
      // The section passed previously so we'll skip it on this iteration
      Console.WriteLine("Skipping repositories validation...");
      return true;
    }

    // Validate the configured repos
    Console.WriteLine("Validating repositories...");
    var validateConfig = await validateRepositoryDetailsService.ValidateRepositoryDetailsAsync(config.RepositoryDetails, names);

    // Check if section ran successfully
    shortCircuitValidation = validateConfig;
    return validateConfig;
  }

  private async Task<bool> FetchLatestChangesForRepositoriesAsync(IConfig config, List<string> names)
  {
    // Short-Circuiting Logic
    if (shortCircuitRepoFetch)
    {
      // The section passed previously so we'll skip it on this iteration
      Console.WriteLine("Skipping repositories latest fetch...");
      return true;
    }

    // Fetch latest changes for repositories
    var fetchLatestChanges = false;
    Console.WriteLine("Fetching latest changes for repositories...");
    foreach (var repoDetail in config.RepositoryDetails)
    {
      gitCommandRunnerService.SetGitRepoDetail(repoDetail);
      var fetchedFrom = gitCommandRunnerService.GitFetchAsync(names.FirstOrDefault());
      var fetchedTo = gitCommandRunnerService.GitFetchAsync(names.LastOrDefault());
      await Task.WhenAll(fetchedFrom, fetchedTo);
      fetchLatestChanges = fetchedFrom != null && fetchedTo != null; // TODO: need a more robust way to handle if the fetch failed.
    }

    // Check if section ran successfully
    shortCircuitRepoFetch = fetchLatestChanges;
    return fetchLatestChanges;
  }

  private async Task<bool> GenerateRawDiffsForRepositoriesAsync(IConfig config, List<string> names)
  {
    // Short-Circuiting Logic
    if (shortCircuitDiffGeneration)
    {
      // The section passed previously so we'll skip it on this iteration
      Console.WriteLine("Skipping raw diff generation...");
      return true;
    }

    // Prompt user for build folder & generate raw diffs for repositories
    Console.WriteLine("Generating diffs for repositories...");
    var diffGeneration = true; // TODO: add service to generate & clean diffs.
    this.rawDiffs = ""; // TODO:

    // Check if section ran successfully
    shortCircuitDiffGeneration = diffGeneration;
    return diffGeneration;
  }

  private async Task<bool> CleanAndGroupDiffsAsync(string rawDiffs)
  {
    // Short-Circuiting Logic
    if (shortCircuitDiffClean)
    {
      // The section passed previously so we'll skip it on this iteration
      Console.WriteLine("Skipping diff cleaning and grouping...");
      return true;
    }

    // Prompt user for build folder & generate raw diffs for repositories
    Console.WriteLine("Cleaning and grouping diffs for repositories...");
    var cleanDiffs = true; // TODO: add service to generate & clean diffs.
    this.cleanDiffs = ""; // TODO:

    // Check if section ran successfully
    shortCircuitDiffClean = cleanDiffs;
    return cleanDiffs;
  }

  private async Task<bool> SaveDiffsToOutputAsync(string build, string cleanedDiffs)
  {
    // TODO: add logic.
    return true;
  }
}