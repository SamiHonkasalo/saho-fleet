using Akka.Actor;
using Akka.Event;
using JetBrains.Annotations;
using SaHoFleet.MQTT;

namespace SaHoFleet.FleetManager.Actors;

[UsedImplicitly]
public class DeviceBroker : ReceiveActor
{
    private readonly ILoggingAdapter _logger = Context.GetLogger();
    private readonly IServiceScope _scope;
    private readonly FleetMqttServer _mqttServer;

    protected override void PreStart()
    {
        _logger.Info("DeviceBroker started");
        base.PreStart();
    }

    protected override void PostStop()
    {
        _scope.Dispose();
        _logger.Info("DeviceBroker stopped");
        base.PostStop();
    }

    public DeviceBroker(IServiceProvider serviceProvider)
    {
        _scope = serviceProvider.CreateScope();
        _mqttServer = _scope.ServiceProvider.GetRequiredService<FleetMqttServer>();
        Receive<Start>(_ =>
        {
            _logger.Info("Broker received the start command");
            var msgHandler = Context.ActorOf(PublishedMessageHandler.Props());
            _mqttServer.Server.InterceptingPublishAsync += (msg) =>
            {
                msgHandler.Tell(msg);
                return Task.CompletedTask;
            };
            try
            {
                _mqttServer.StartAsync().Wait();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Could not start the MQTT server, shutting down!");
                Context.System.Terminate().Wait();
            }
        });
    }

    #region Messages

    public class Start
    {
    }

    #endregion
}