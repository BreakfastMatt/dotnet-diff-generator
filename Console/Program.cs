namespace ConsoleApp;
using Application.DelegatorService;
using Application.DiffGenerationService;
using Application.GitCommandRunnerService;
using Application.PromptUserInputService;
using Application.ReadFromConfigService;
using Application.ValidateRepositoryDetailsService;
using Domain.JsonSerialiser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Interfaces.Services.DelegatorService;
using Models.Interfaces.Services.DiffGenerationService;
using Models.Interfaces.Services.GitCommandRunnerService;
using Models.Interfaces.Services.JsonSerialiser;
using Models.Interfaces.Services.PromptUserInputService;
using Models.Interfaces.Services.ReadFromConfigService;
using Models.Interfaces.Services.ValidateRepositoryDetailsService;

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
        // Register Domain Services
        services.AddSingleton<IJsonSerialiser, JsonSerialiser>();

        // Register Application Services
        services.AddTransient<IGitCommandRunnerService, GitCommandRunnerService>();
        services.AddScoped<IDelegatorService, DelegatorService>();
        services.AddScoped<IReadFromConfigService, ReadFromConfigService>();
        services.AddScoped<IPromptUserInputService, PromptUserInputService>();
        services.AddScoped<IValidateRepositoryDetailsService, ValidateRepositoryDetailsService>();
        services.AddScoped<IDiffGenerationService, DiffGenerationService>();
      })
      .Build();

    // Call the delegator service
    var service = host.Services.GetRequiredService<IDelegatorService>();
    await service.DelegateAsync();
  }
}