namespace Models.Interfaces.Diffs;

public interface ICommit
{
  /// <summary>
  /// The name of the developer who pushed the commit
  /// </summary>
  public string Author { get; set; }

  /// <summary>
  /// The commit message that the developer committed
  /// </summary>
  public string CommitMessage { get; set; }

  /// <summary>
  /// The Epic link for the commit (if applicable)
  /// </summary>
  public string? EpicLink { get; set; }

  /// <summary>
  /// The JIRA references for the commit (if provided)
  /// </summary>
  public List<string> References { get; set; }

  /// <summary>
  /// Whether the commit was committed according to project standards
  /// </summary>
  public bool FollowsCommitStandards { get; set; }
}