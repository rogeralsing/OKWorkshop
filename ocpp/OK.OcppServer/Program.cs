using OK.OcppServer.Infra;
using Microsoft.AspNetCore.WebSockets;
using OK.Messages;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Seed;
using Proto.Remote;
using Proto.Remote.GrpcNet;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebSockets(x =>
{
    
});
var system = new ActorSystem(new ActorSystemConfig()
        .WithDeveloperSupervisionLogging(true))
    .WithRemote(GrpcNetRemoteConfig.BindToLocalhost().WithProtoMessages(MessagesReflection.Descriptor))
    .WithCluster(ClusterConfig.Setup("MyCluster", new SeedNodeClusterProvider(new SeedNodeClusterProviderOptions(("127.0.0.1",8090))), new PartitionIdentityLookup()));

builder.Services.AddSingleton(system);
builder.Services.AddLogging();
builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
Log.SetLoggerFactory(LoggerFactory.Create(l => l.AddConsole().SetMinimumLevel(LogLevel.Information)));


await system.Cluster().StartMemberAsync();
app.UseWebSockets(new WebSocketOptions()
{
    
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();