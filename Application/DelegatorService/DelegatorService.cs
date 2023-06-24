using Models.Interfaces.Services.Delegator;
using Models.Interfaces.Services.ReadFromConfig;
using Models.Interfaces.Services.ValidateRepositoryDetails;

namespace Application.DelegatorService;

public class DelegatorService : IDelegatorService
{
  // Inject relevant services
  private readonly IReadFromConfigService readFromConfigService;
  private readonly IValidateRepositoryDetailsService validateRepositoryDetailsService;
  public DelegatorService(IReadFromConfigService readFromConfigService, IValidateRepositoryDetailsService validateRepositoryDetailsService)
  {
    this.readFromConfigService = readFromConfigService;
    this.validateRepositoryDetailsService = validateRepositoryDetailsService;
  }

  public Task GetResponseAsync()
  {
    // Fetch Configured Settings from Config.json
    var config = readFromConfigService.ReadFromConfig();

    // Validate the configured repos
    var validateConfig = validateRepositoryDetailsService.ValidateRepositoryDetailsAsync(config.RepositoryDetails);

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