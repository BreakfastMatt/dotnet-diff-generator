namespace Models.Constants;

/// <summary>
/// Global constants to be used throughout the application
/// </summary>
public class GlobalConstants
{
  // Git Command Runner Service Constants
  public const string gitDefaultRemote = "origin";
  public const string gitLogDelimiter = "=====";
  public const string gitLogDiffSeparator = "\r\n";
  public const string gitTempStashName = "Git-Diff-Generator-Stash";
  public const string gitUncommittedChanges = "Changes to be committed";
  public const string gitUnstagedChanges = "Changes not staged";
  public const string gitUntrackedFiles = "Untracked files";

  // Diff Generation Service Constants
  public const string diffEpicNotCaptured = "EpicNotCaptured";
  public const string diffCommitWithoutReference = "CommitsWithoutReferences";
}