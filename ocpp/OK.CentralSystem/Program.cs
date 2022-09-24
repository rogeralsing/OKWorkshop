using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OK.CentralSystem;
using OK.Messages;
using Proto.Remote;
using Proto.Remote.Healthchecks;
using Proto.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var chargePoint = ChargePointActor.GetClusterKind((ctx, _) => new ChargepointActor(ctx));

builder.Services.AddProtoCluster("MyCluster", port: 8090,
    configureRemote: r => r.WithProtoMessages(MessagesReflection.Descriptor),
    configureCluster: c => c.WithClusterKind(chargePoint));

builder.Services.AddHealthChecks().AddCheck<ActorSystemHealthCheck>("proto", null, new[] { "ready", "live" });

var app = builder.Build();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();