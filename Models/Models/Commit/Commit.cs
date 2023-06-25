using Models.Interfaces.Diffs;

namespace Models.Models.Commit;

public class Commit : ICommit
{
  public string Author { get; set; }
  public string CommitMessage { get; set; }
  public DateTime CommitDate { get; set; }
  public string? EpicLink { get; set; }
  public List<string> References { get; set; }
  public bool FollowsCommitStandards { get; set; }
}