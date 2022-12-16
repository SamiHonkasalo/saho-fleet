using Akka.Actor;
using Akka.Event;
using JetBrains.Annotations;
using MQTTnet;
using MQTTnet.Server;

namespace SaHoFleet.FleetManager.Actors;

[UsedImplicitly]
public class PublishedMessageHandler : ReceiveActor
{
    private readonly ILoggingAdapter _logger = Context.GetLogger();

    public PublishedMessageHandler()
    {
        Receive<InterceptingPublishEventArgs>((msg) =>
        {
            _logger.Debug("Received a message with topic {topic}", msg.ApplicationMessage.Topic);
            _logger.Debug("Message payload: {payload}", msg.ApplicationMessage.ConvertPayloadToString());
        });
    }

    public static Props Props() => Akka.Actor.Props.Create<PublishedMessageHandler>();
}