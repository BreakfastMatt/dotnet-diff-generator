using Models.Interfaces.Config;
using Models.Interfaces.Services.GitCommandRunnerService;

namespace Application.GitCommandRunnerService;

/// <inheritdoc/>
public class GitCommandRunnerService : IGitCommandRunnerService
{
  private IRepositoryDetails repositoryDetail;
  private string remote;

  /// <inheritdoc/>
  public void SetGitRepoDetail(IRepositoryDetails repoDetail, string remote = "origin")
  {
    this.repositoryDetail = repoDetail;
    this.remote = remote;
  }

  /// <inheritdoc/>
  public bool GitCheckWorkingTree()
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public void GitStashSave()
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>

  public void GitStashPop(string stashName = "diff_generator")
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public void GitFetch(string? name = null)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public void GitPull(string? name = null)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public string GitLog(string from, string to)
  {
    throw new NotImplementedException();
  }
}