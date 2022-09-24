using OK.CentralSystem;
using OK.Messages;
using Proto.Remote;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var chargePoint = ChargePointActor.GetClusterKind((ctx, _) => new ChargepointActor(ctx));

builder.Services.AddProtoCluster("MyCluster",
    port:8090,
    remoteConfigFactory: r => r.WithProtoMessages(MessagesReflection.Descriptor),
    clusterConfigFactory: c => c.WithClusterKind(chargePoint));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();