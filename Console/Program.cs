using Application.ReadFromConfig;
using Domain;
using Models.Interfaces.Services.ReadFromConfig;

namespace ConsoleApp;

public class Program
{
  //private readonly IReadFromConfigService readFromConfigService;
  //public Program(IReadFromConfigService readFromConfigService)
  //{
  //  this.readFromConfigService = readFromConfigService;
  //}

  /// <summary>
  /// The main console application entry point
  /// </summary>
  public static void Main()
  {
    // TODO: tmep
    TempTest();

    // Prevent automatic console closure
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
  }

  public static void TempTest()
  {
    // TODO: temp logic, need a proper way to do this.
    // TODO: note project references need to be removed after this
    var jsonSerialiser = new JsonSerialiser();
    var readFromConfigService = new ReadFromConfigService(jsonSerialiser);
    var configDetails = readFromConfigService.ReadFromConfig();
  }
}