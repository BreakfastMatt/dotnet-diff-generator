using Application.DelegatorService;
using Application.GitCommandRunnerService;
using Application.ReadFromConfig;
using Application.ValidateRepositoryDetailsService;
using Domain.JsonSerialiser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Interfaces.Services.Delegator;
using Models.Interfaces.Services.GitCommandRunnerService;
using Models.Interfaces.Services.JsonSerialiser;
using Models.Interfaces.Services.ReadFromConfig;
using Models.Interfaces.Services.ValidateRepositoryDetails;

namespace ConsoleApp;

public class Program
{
  /// <summary>
  /// The main console application entry point
  /// </summary>
  public static async Task Main(string[] args)
  {
    // Set the synchronization context to null to disable context capturing
    SynchronizationContext.SetSynchronizationContext(null);

    // Create DI Container
    using var host = Host.CreateDefaultBuilder(args)
      .ConfigureServices(services =>
      {
        // Register Services
        services.AddSingleton<IJsonSerialiser, JsonSerialiser>();
        services.AddScoped<IDelegatorService, DelegatorService>();
        services.AddScoped<IGitCommandRunnerService, GitCommandRunnerService>();
        services.AddScoped<IReadFromConfigService, ReadFromConfigService>();
        services.AddScoped<IValidateRepositoryDetailsService, ValidateRepositoryDetailsService>();
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