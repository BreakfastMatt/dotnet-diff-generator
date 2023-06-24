using Models.Interfaces.Services.DelegatorService;
using Models.Interfaces.Services.ReadFromConfigService;
using Models.Interfaces.Services.ValidateRepositoryDetailsService;

namespace Application.DelegatorService;

public class DelegatorService : IDelegatorService
{
  // Inject relevant services
  private readonly IReadFromConfigService readFromConfigService;
  //private readonly IPromptUserInputService promptUserInputService;
  private readonly IValidateRepositoryDetailsService validateRepositoryDetailsService;
  public DelegatorService(IReadFromConfigService readFromConfigService, IValidateRepositoryDetailsService validateRepositoryDetailsService)
  {
    this.readFromConfigService = readFromConfigService;
    this.validateRepositoryDetailsService = validateRepositoryDetailsService;
  }

  public async Task GetResponseAsync()
  {
    // Fetch Configured Settings from Config.json
    var config = readFromConfigService.ReadFromConfig();

    // Prompt the user for input
    var names = new List<string> { }; // TODO:

    // Validate the configured repos
    var validateConfig = await validateRepositoryDetailsService.ValidateRepositoryDetailsAsync(config.RepositoryDetails, names);

    // Fetch the latest changes for the specified branches/tags
    // TODO:

    // Generate the raw diffs for the specified branches/tags
    // TODO:

    // Cleanup & Group the diffs
    // TODO:

    // Save the cleaned diff to the configured path
    // TODO:
  }
}