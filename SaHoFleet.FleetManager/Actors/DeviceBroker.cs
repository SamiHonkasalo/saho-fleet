using Akka.Actor;
using Akka.Event;
using JetBrains.Annotations;
using static Akka.Actor.Props;

namespace SaHoFleet.FleetManager.Actors;

[UsedImplicitly]
public class DeviceBroker : UntypedActor
{
    private readonly ILoggingAdapter _logger = Context.GetLogger();

    protected override void PreStart() => _logger.Info("DeviceBroker started");
    protected override void PostStop() => _logger.Info("DeviceBroker stopped");

    protected override void OnReceive(object message)
    {
        _logger.Info("Got a message: {msg}", message);
    }

    public static Props Prop() => Create<DeviceBroker>();
}

#region Messages

public class StartBroker
{
}

#endregion