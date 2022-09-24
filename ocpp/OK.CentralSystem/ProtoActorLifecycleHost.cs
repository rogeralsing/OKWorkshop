using Proto.Cluster;

namespace OK.CentralSystem;

public class ProtoActorLifecycleHost : IHostedService
{
    private readonly Cluster _cluster;

    public ProtoActorLifecycleHost(Cluster cluster)
    {
        _cluster = cluster;
    }

    public async Task StartAsync(CancellationToken _)
    {
        await _cluster.StartMemberAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cluster.ShutdownAsync();
    }
}