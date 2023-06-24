namespace Models.Interfaces.Services.PromptUserInputService;

/// <summary>
/// Runs various methods to prompt the user for input
/// </summary>
public interface IPromptUserInputService
{
  /// <summary>
  /// Prompts the user to enter a 'from' branch/tag and 'to' branch/tag.
  /// This will be used to generate the diff between the 'from' and 'to' references.
  /// </summary>
  /// <returns>A Tuple containing the 'from' branch/tag and 'to' branch/tag values respectively</returns>
  Tuple<string, string> PromptBranchOrTagNames();

  /// <summary>
  /// Prompts the user to enter a unique 'build' name for the diffs being generated.
  /// This will be used as the folder name to save the diffs to.
  /// </summary>
  /// <returns>The name of the 'build' entered by the user</returns>
  string PromptBuildName();
}