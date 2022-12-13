using NLog;
using NLog.Web;
using SaHoFleet.FleetManager;

var logger = LogManager.Setup()
    .LoadConfigurationFromFile("nlog.config", false)
    .GetCurrentClassLogger();
logger.Info("Starting FleetManager!");


try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    
    // starts the IHostedService, which creates the ActorSystem and actors
    builder.Services.AddHostedService<AkkaService>();

    var app = builder.Build();
    

    app.MapGet("/health", Results.NoContent);

    app.Run();
}
catch (Exception e)
{
    // NLog: catch setup errors
    logger.Error(e, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}