using Microsoft.Extensions.Hosting;

namespace Proto.Utils;

public class ProtoActorLifecycleHost : IHostedService
{
    private readonly Cluster.Cluster _cluster;

    public ProtoActorLifecycleHost(Cluster.Cluster cluster)
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