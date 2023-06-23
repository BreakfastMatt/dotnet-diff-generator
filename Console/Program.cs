using Application.DelegatorService;
using Application.ReadFromConfig;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Interfaces.Services.Delegator;
using Models.Interfaces.Services.JsonSerialiser;
using Models.Interfaces.Services.ReadFromConfig;

namespace ConsoleApp;

public class Program
{
  /// <summary>
  /// The main console application entry point
  /// </summary>
  public static async Task Main(string[] args)
  {
    // Create DI Container
    using var host = Host.CreateDefaultBuilder(args)
      .ConfigureServices(services =>
      {
        // Register Services
        services.AddSingleton<IJsonSerialiser, JsonSerialiser>();
        services.AddScoped<IDelegatorService, DelegatorService>();
        services.AddScoped<IReadFromConfigService, ReadFromConfigService>();
      })
      .Build();

    // Call the delegator service
    var service = host.Services.GetRequiredService<IDelegatorService>();
    await service.GetResponseAsync();

    // Prevent automatic console closure
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
  }
}