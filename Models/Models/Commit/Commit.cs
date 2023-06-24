using Models.Interfaces.Diffs;

namespace Models.Models.Commit;

public class Commit : ICommit
{
  public string Author { get; set; }
  public string CommitMessage { get; set; }
  public string EpicLink { get; set; }
  public string Reference { get; set; }
  public bool FollowsCommitStandards { get; set; }
}