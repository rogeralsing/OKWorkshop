using OK.CentralSystem;
using OK.Messages;
using Proto.Remote;
using Proto.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var chargePoint = ChargePointActor.GetClusterKind((ctx, _) => new ChargepointActor(ctx));
builder.Services.AddProtoCluster("MyCluster", port: 8090,
    remoteConfigFactory: r => r.WithProtoMessages(MessagesReflection.Descriptor),
    clusterConfigFactory: c => c.WithClusterKind(chargePoint));
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();