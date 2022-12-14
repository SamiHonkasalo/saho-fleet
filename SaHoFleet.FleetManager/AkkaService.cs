using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using SaHoFleet.FleetManager.Actors;

namespace SaHoFleet.FleetManager;

/// <summary>
/// IHostedService implementation for <see cref="ActorSystem"/>
/// </summary>
public class AkkaService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _lifetime;
    private ActorSystem _actorSystem = null!;

    public AkkaService(IHostApplicationLifetime lifetime, IServiceProvider serviceProvider)
    {
        _lifetime = lifetime;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var bootstrap = BootstrapSetup.Create()
            .WithConfig(ConfigurationFactory.ParseString(File.ReadAllText("akka.conf")));
        // enable DI support inside this ActorSystem, if needed
        var diSetup = DependencyResolverSetup.Create(_serviceProvider);
        // merge this setup (and any others) together into ActorSystemSetup
        var actorSystemSetup = bootstrap.And(diSetup);
        // start ActorSystem
        _actorSystem = ActorSystem.Create("DeviceBrokerSystem", actorSystemSetup);
        var brokerProps = DependencyResolver.For(_actorSystem).Props<DeviceBroker>();
        var deviceBroker = _actorSystem.ActorOf(brokerProps, "DeviceBroker");
        deviceBroker.Tell(new DeviceBroker.Start());

        _actorSystem.WhenTerminated.ContinueWith(_ => { _lifetime.StopApplication(); }, cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await CoordinatedShutdown.Get(_actorSystem)
            .Run(CoordinatedShutdown.ClrExitReason.Instance);
    }
}