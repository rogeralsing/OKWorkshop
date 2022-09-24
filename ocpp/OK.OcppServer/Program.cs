using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.WebSockets;
using OK.Messages;
using OK.OcppServer.Infra;
using Proto.Cluster.Seed;
using Proto.Remote;
using Proto.Remote.Healthchecks;
using Proto.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebSockets(_ => { });
var clusterProvider = new SeedNodeClusterProvider(new SeedNodeClusterProviderOptions(("localhost", 8090)));

builder.Services.AddProtoCluster("MyCluster",
    configureRemote: r => r.WithProtoMessages(MessagesReflection.Descriptor),
    clusterProvider: clusterProvider);

builder.Services.AddLogging();
builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddCheck<ActorSystemHealthCheck>("proto", null, new[] { "ready", "live" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/live", new HealthCheckOptions
{
    Predicate = x => x.Tags.Contains("live")
});

app.MapHealthChecks("/ready", new HealthCheckOptions
{
    Predicate = x => x.Tags.Contains("ready")
});

app.UseWebSockets(new WebSocketOptions());
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();