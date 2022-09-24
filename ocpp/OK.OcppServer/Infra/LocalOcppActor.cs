using OK.Messages;
using Proto;
using Proto.Cluster;

namespace OK.OcppServer.Infra;

public class LocalOcppActor : IActor
{
    private readonly OcppConnection _con;
    private readonly string _id;

    public LocalOcppActor(string id, OcppConnection con)
    {
        _id = id;
        _con = con;
    }

    public async Task ReceiveAsync(IContext context)
    {
        if (context.Message is FromChargepoint fromChargePoint)
        {
            var client = context.Cluster().GetChargePoint(_id);
            await client.HandleFromChargePoint(fromChargePoint, CancellationToken.None);
        }
        else if (context.Message is ToChargepoint toChargePoint)
        {
            await _con.SendStringAsync(toChargePoint.Payload, CancellationToken.None);
        }
    }
}