using Akka.Actor;
using Akka.Event;
using JetBrains.Annotations;
using MQTTnet;
using MQTTnet.Server;
using SaHoFleet.MQTT;

namespace SaHoFleet.FleetManager.Actors;

[UsedImplicitly]
public class DeviceBroker : ReceiveActor
{
    private readonly ILoggingAdapter _logger = Context.GetLogger();
    private readonly IServiceScope _scope;
    private readonly IActorRef _self;
    private readonly FleetMqttServer _mqttServer;

    protected override void PreStart() => _logger.Info("DeviceBroker started");

    protected override void PostStop()
    {
        _scope.Dispose();
        _logger.Info("DeviceBroker stopped");
    }

    public DeviceBroker(IServiceProvider serviceProvider)
    {
        // Need to save a reference to self so the message event handler does not try to use the context  
        _self = Self;
        _scope = serviceProvider.CreateScope();
        _mqttServer = _scope.ServiceProvider.GetRequiredService<FleetMqttServer>();
        ReceiveAsync<Start>(async _ =>
        {
            _logger.Info("Broker received the start command");
            _mqttServer.Server.InterceptingPublishAsync += HandleMessage;
            try
            {
                await _mqttServer.StartAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Could not start the MQTT server, shutting down!");
                await Context.System.Terminate();
            }
        });
    }

    private Task HandleMessage(InterceptingPublishEventArgs msg)
    {
        // TODO: Forward the message to a MessageHandler actor
        _self.Tell(msg.ApplicationMessage);
        _logger.Info(msg.ApplicationMessage.ConvertPayloadToString());
        return Task.CompletedTask;
    }

    #region Messages

    public class Start
    {
    }

    #endregion
}