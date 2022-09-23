using OK.CentralSystem;
using OK.Messages;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Seed;
using Proto.Remote;
using Proto.Remote.GrpcNet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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


var system = new ActorSystem(new ActorSystemConfig().WithDeveloperSupervisionLogging(true)).WithRemote(GrpcNetRemoteConfig.BindToLocalhost(8090).WithProtoMessages(MessagesReflection.Descriptor)).WithCluster(ClusterConfig.Setup("MyCluster", new SeedNodeClusterProvider(), new PartitionIdentityLookup())
    .WithClusterKind(ChargePointActor.GetClusterKind((ctx, _) => new ChargepointActor(ctx))));
    

await system.Cluster().StartMemberAsync();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();