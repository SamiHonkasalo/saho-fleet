using System.Text.RegularExpressions;
using Akka.Actor;
using Akka.Event;
using JetBrains.Annotations;
using MQTTnet.Protocol;
using MQTTnet.Server;
using SaHoFleet.Models.Vehicle;

namespace SaHoFleet.FleetManager.Actors;

[UsedImplicitly]
public partial class PublishedMessageHandler : ReceiveActor
{
    private readonly ILoggingAdapter _logger = Context.GetLogger();
    private readonly Dictionary<Guid, IActorRef> _vehicleActors = new();

    public PublishedMessageHandler()
    {
        Receive<InterceptingPublishEventArgs>((msg) =>
        {
            // TODO: Divide the vehicle into groups, and create handlers for groups
            // For now, handle each different vehicle on a single actor
            var (success, vehicleId) = ParseMqttTopic(msg.ApplicationMessage.Topic);
            if (!success)
            {
                _logger.Warning("Received an invalid topic {topic} from client ${client}", msg.ApplicationMessage.Topic,
                    msg.ClientId);
            }
            // TODO: Create new actor for vehicle, or use the existing one from the dictionary
        });
    }

    private static (bool, Guid?) ParseMqttTopic(string topic)
    {
        var topicCheck = TopicRegex();
        var match = topicCheck.Match(topic);
        if (!match.Success) return (false, null);
        if (!Guid.TryParse(match.Groups[0].Value, out var vehicleId)) return (false, null);
        return (true, vehicleId);
    }

    public static Props Props() => Akka.Actor.Props.Create<PublishedMessageHandler>();

    [GeneratedRegex("^vehicle\\/(.*)\\/telemetry$")]
    private static partial Regex TopicRegex();
}