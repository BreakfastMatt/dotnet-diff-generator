using Models.Interfaces.Config;
using Models.Models.Commit;

namespace Models.Interfaces.Services.DiffGenerationService;

/// <summary>
/// A service to generate the raw diffs, clean them and group them
/// </summary>
public interface IDiffGenerationService
{
  /// <summary>
  /// Generates the raw diffs, then cleans and groups them.
  /// Once all the diffs have been generated, they will be combined and saved to the user specified output directory.
  /// </summary>
  /// <param name="config">The parsed JSON config details</param>
  /// <param name="build">The unique name for the build as provided by the user</param>
  /// <param name="fromReference">The branch or tag to generate the diffs 'from'</param>
  /// <param name="toReference">The branch or tag to generate the diffs 'to'</param>
  /// <returns>An indicator that denotes whether the diffs generated correctly</returns>
  Task<bool> GenerateRepositoryDiffsAsync(IConfig config, string build, string fromReference, string toReference);

  /// <summary>
  /// Generates the raw diffs from the provided <paramref name="fromReference"/> to the provided <paramref name="toReference"/> for the specified repository.
  /// </summary>
  /// <param name="repoDetail">The details of the repository where the git commands will be run</param>
  /// <param name="fromReference">The branch or tag to generate the diffs 'from'</param>
  /// <param name="toReference">The branch or tag to generate the diffs 'to'</param>
  /// <returns>The List of structured commits</returns>
  Task<List<Commit>> GenerateRawDiffForRepositoryAsync(IRepositoryDetails repoDetail, string fromReference, string toReference);

  /// <summary>
  /// Takes in the raw commits and extracts the JIRA references.
  /// </summary>
  /// <param name="commits">The List of structured commits</param>
  /// <returns>The extracted dictinoary entires of commits</returns>
  Dictionary<string, List<string>> ExtractCommitReferences(List<Commit> commits);

  /// <summary>
  /// Takes in the list of cleaned diff references for the repositories and collates them.
  /// In the process all duplicate entires will be removed to ensure there's a distinct list of commits.
  /// The distinct list will then be formatted and structured into a single string.
  /// </summary>
  /// <param name="diffsList">A list of all extracted commits for the configured repositories</param>
  /// <returns>A formatted string containing the distinct diff entries grouped by feature references</returns>
  string ConvertDiffsToString(List<Dictionary<string, List<string>>> diffsList);

  /// <summary>
  /// Takes in the provided <paramref name="build"/> name, creates the folder if it isn't present
  /// and saves the <paramref name="diffs"/> to a file in this folder.
  /// </summary>
  /// <param name="build">The unique name for the build as provided by the user</param>
  /// <param name="diffs">The extracted commits for the configured repositories</param>
  /// <returns>True if the save was successful, otherwise false</returns>
  bool SaveDiffsToOutputDirectory(string build, string diffs);
}