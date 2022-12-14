using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;

namespace SaHoFleet.MQTT;

[UsedImplicitly]
public class FleetMqttServer : IAsyncDisposable
{
    private readonly ILogger<FleetMqttServer> _logger;
    public readonly MqttServer Server;

    public FleetMqttServer(ILogger<FleetMqttServer> logger)
    {
        _logger = logger;
        var opt = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .Build();
        Server = new MqttFactory().CreateMqttServer(opt);
    }

    public async Task StartAsync()
    {
        try
        {
            Server.ValidatingConnectionAsync += ValidateConnectionAsync;
            _logger.LogInformation("Starting the MQTT server");
            await Server.StartAsync();
            _logger.LogInformation("MQTT server started");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to start the MQTT server!");
            throw;
        }
    }

    private Task ValidateConnectionAsync(ValidatingConnectionEventArgs args)
    {
        _logger.LogDebug("Validating connection for client ID {id}", args.ClientId);
        // TODO: Check the validity of the connection
        _logger.LogDebug("Validated connection for client ID {id}", args.ClientId);
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _logger.LogInformation("Stopping MQTT server");
        Server.StopAsync();
        Server.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}