namespace UserInterface;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
  /// <summary>
  /// The maui application entry point
  /// </summary>
  public static MauiApp CreateMauiApp()
  {
    // Set the synchronization context to null to disable context capturing
    SynchronizationContext.SetSynchronizationContext(null);

    // Create DI Container
    var builder = MauiApp
      .CreateBuilder()
      .UseMauiApp<App>()
      .ConfigureDebugMode()
      .ConfigureFonts(fonts =>
      {
        // Add applicable fonts
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
      })
      .Build();

    return builder;
  }

  private static MauiAppBuilder ConfigureDebugMode(this MauiAppBuilder builder)
  {
    // Configure debug-specific details
    if (Debugger.IsAttached)
    {
      builder.Logging.AddDebug();
    }

    return builder;
  }
}