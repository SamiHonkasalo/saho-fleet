using MQTTnet.AspNetCore;
using NLog;
using NLog.Web;
using SaHoFleet.FleetManager;
using SaHoFleet.MQTT;

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

    // Allow MQTT connections on TCP port 1883
    builder.WebHost.ConfigureKestrel(opt => { opt.ListenAnyIP(1883, l => l.UseMqtt()); });

    // starts the IHostedService, which creates the ActorSystem and actors
    builder.Services.AddHostedService<AkkaService>();
    builder.Services.AddSingleton<FleetMqttServer>();

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