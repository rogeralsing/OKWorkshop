using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Seed;
using Proto.DependencyInjection;
using Proto.Remote.GrpcNet;

namespace OK.CentralSystem;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProtoCluster(this IServiceCollection self,string clusterName, string bindToHost="localhost", int port=0, Func<ActorSystemConfig, ActorSystemConfig>? systemConfigFactory = null, Func<GrpcNetRemoteConfig, GrpcNetRemoteConfig>? remoteConfigFactory = null, Func<ClusterConfig,ClusterConfig>? clusterConfigFactory = null)
    {
        self.AddSingleton(p =>
        {
            var loggerFactory = p.GetRequiredService<ILoggerFactory>();
            Log.SetLoggerFactory(loggerFactory);
            
            var s = new ActorSystemConfig();
            s = systemConfigFactory?.Invoke(s) ?? s;

            var r = GrpcNetRemoteConfig.BindTo(bindToHost, port);
            r = remoteConfigFactory?.Invoke(r) ?? r;

            var c = ClusterConfig.Setup(clusterName, new SeedNodeClusterProvider(), new PartitionIdentityLookup());
            c = clusterConfigFactory?.Invoke(c) ?? c;
            
            var system = new ActorSystem(s)
                .WithRemote(r)
                .WithCluster(c)
                .WithServiceProvider(p);

            return system;
        });

        self.AddSingleton(p => p.GetRequiredService<ActorSystem>().Cluster());
        self.AddSingleton(p => p.GetRequiredService<ActorSystem>().Root);
        self.AddHostedService<ProtoActorLifecycleHost>();

        return self;
    }
}