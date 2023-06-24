using Models.Interfaces.Services.PromptUserInputService;

namespace Application.PromptUserInputService;

public class PromptUserInputService : IPromptUserInputService
{
  public Tuple<string, string> PromptBranchOrTagNames()
  {
    // Prompts the user to enter a 'from' branch or tag
    Console.Clear();
    var fromReference = string.Empty;
    while (string.IsNullOrEmpty(fromReference))
    {
      Console.WriteLine("Please enter a 'from' branch or tag");
      fromReference = Console.ReadLine() ?? string.Empty;
    }

    // Prompts the user to enter a 'to' branch or tag
    var toReference = string.Empty;
    while (string.IsNullOrEmpty(toReference))
    {
      Console.WriteLine("Please enter a 'to' branch or tag");
      toReference = Console.ReadLine() ?? string.Empty;
    }

    // Returns a Tuple containing the 'from' & 'to' values
    Console.Clear();
    var response = new Tuple<string, string>(fromReference, toReference);
    return response;
  }

  public string PromptBuildName()
  {
    // Prompts the user to enter a 'build' name
    //Console.Clear();
    var buildName = string.Empty;
    while (string.IsNullOrEmpty(buildName))
    {
      Console.WriteLine("Please enter a name for the build you are generating diffs for");
      buildName = Console.ReadLine() ?? string.Empty;
    }
    return buildName;
  }
}