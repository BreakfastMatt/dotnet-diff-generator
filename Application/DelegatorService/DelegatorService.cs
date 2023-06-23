using Models.Interfaces.Services.Delegator;
using Models.Interfaces.Services.ReadFromConfig;

namespace Application.DelegatorService;

/// <inheritdoc/>
public class DelegatorService : IDelegatorService
{
  private readonly IReadFromConfigService readFromConfigService;
  public DelegatorService(IReadFromConfigService readFromConfigService)
  {
    this.readFromConfigService = readFromConfigService;
  }

  /// <inheritdoc/>
  public Task GetResponseAsync()
  {
    // Fetch Configured Settings from Config.json
    var config = readFromConfigService.ReadFromConfig();

    // Check if the specified branches/tags are present
    // TODO:

    // Fetch the latest changes for the specified branches/tags
    // TODO:

    // Generate the raw diffs for the specified branches/tags
    // TODO:

    // Cleanup & Group the diffs
    // TODO:

    // Save the cleaned diff to the configured path
    // TODO:
    return Task.CompletedTask;
  }
}