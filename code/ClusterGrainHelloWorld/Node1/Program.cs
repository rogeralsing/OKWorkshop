// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Asynkron AB">
//      Copyright (C) 2015-2022 Asynkron AB All rights reserved
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using ClusterHelloWorld.Messages;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Cluster.Partition;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using static Proto.CancellationTokens;
using ProtosReflection = ClusterHelloWorld.Messages.ProtosReflection;

internal class Program
{
    private static async Task Main()
    {
        Log.SetLoggerFactory(LoggerFactory.Create(l => l.AddConsole().SetMinimumLevel(LogLevel.Information)));

        // Required to allow unencrypted GrpcNet connections
        var system = new ActorSystem().WithRemote(GrpcNetRemoteConfig.BindToLocalhost().WithProtoMessages(ProtosReflection.Descriptor)).WithCluster(ClusterConfig.Setup("MyCluster", new ConsulProvider(new ConsulProviderConfig()), new PartitionIdentityLookup()));

        // system.EventStream.Subscribe<ClusterTopology>(e => {
        //         Console.WriteLine($"{DateTime.Now:O} My members {e.TopologyHash}");
        //     }
        // );
        await system.Cluster().StartMemberAsync();
        Console.WriteLine("Started");
        var helloGrain = system.Cluster().GetHelloGrain("MyGrain");
        while (true)
        {
            try
            {
                var res = await helloGrain.SayHello(new HelloRequest(), FromSeconds(5));
                Console.WriteLine(res.Message);
            }
            catch
            {
            }

            Console.ReadLine();
        }

        // res = await helloGrain.SayHello(new HelloRequest(), FromSeconds(5));
        // Console.WriteLine(res.Message);
        Console.WriteLine("Press enter to exit");
        Console.ReadLine();
        Console.WriteLine("Shutting Down...");
        await system.Cluster().ShutdownAsync();
    }
}