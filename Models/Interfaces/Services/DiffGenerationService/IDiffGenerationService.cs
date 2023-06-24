using Models.Interfaces.Config;
using Models.Models.Commit;

namespace Models.Interfaces.Services.DiffGenerationService;

/// <summary>
/// A service to generate the raw diffs, clean them and group them
/// </summary>
public interface IDiffGenerationService // TODO: add comments / finish functionality
{
  Task<bool> GenerateRepositoryDiffsAsync(IRepositoryDetails repoDetail, string build, string fromReference, string toReference);

  Task<List<Commit>> GenerateRawDiffForRepositoryAsync(IRepositoryDetails repoDetail, string fromReference, string toReference);

  List<Commit> ExtractCommitReferences(List<Commit> commits);

  string GroupDiffsForRepository(List<Commit> commits);
}