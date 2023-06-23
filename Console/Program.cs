using Application.ReadFromConfig;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Interfaces.Services.JsonSerialiser;
using Models.Interfaces.Services.ReadFromConfig;

namespace ConsoleApp;

public class Program
{
  /// <summary>
  /// The main console application entry point
  /// </summary>
  public static void Main(string[] args)
  {
    // Create DI Container
    using var host = Host.CreateDefaultBuilder(args)
      .ConfigureServices(services =>
      {
        // Register Services
        services.AddSingleton<IJsonSerialiser, JsonSerialiser>();
        services.AddScoped<IReadFromConfigService, ReadFromConfigService>();
      })
      .Build();

    // Resolve the dependencies
    var service = host.Services.GetRequiredService<IReadFromConfigService>(); // TODO: calling this service for now (proof of concept)

    // Use the service
    service.ReadFromConfig();

    // Prevent automatic console closure
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
  }
}