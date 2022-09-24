using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Seed;
using Proto.DependencyInjection;
using Proto.Remote.GrpcNet;
using Proto.Remote.Healthchecks;

namespace Proto.Utils;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProtoCluster(this IServiceCollection self, string clusterName, string bindToHost = "localhost", int port = 0, Func<ActorSystemConfig, ActorSystemConfig>? systemConfigFactory = null, Func<GrpcNetRemoteConfig, GrpcNetRemoteConfig>? remoteConfigFactory = null, Func<ClusterConfig, ClusterConfig>? clusterConfigFactory = null, IClusterProvider? clusterProvider = null)
    {
        self.AddSingleton(p =>
        {
            var loggerFactory = p.GetRequiredService<ILoggerFactory>();
            Log.SetLoggerFactory(loggerFactory);
            var s = new ActorSystemConfig();
            s = systemConfigFactory?.Invoke(s) ?? s;
            var r = GrpcNetRemoteConfig.BindTo(bindToHost, port);
            r = remoteConfigFactory?.Invoke(r) ?? r;
            clusterProvider ??= new SeedNodeClusterProvider();
            var c = ClusterConfig.Setup(clusterName, clusterProvider, new PartitionIdentityLookup());
            c = clusterConfigFactory?.Invoke(c) ?? c;
            var system = new ActorSystem(s).WithRemote(r).WithCluster(c).WithServiceProvider(p);
            return system;
        });
        self.AddSingleton(p => p.GetRequiredService<ActorSystem>().Cluster());
        self.AddSingleton(p => p.GetRequiredService<ActorSystem>().Root);
        self.AddHostedService<ProtoActorLifecycleHost>();
        self.AddHealthChecks().AddCheck<ActorSystemHealthCheck>("proto");
        return self;
    }
}