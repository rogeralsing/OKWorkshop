using OK.Messages;
using Proto;
using Proto.Cluster;

namespace OK.OcppServer.Infra;

public class LocalOcppActor : IActor
{
    private readonly string _id;
    private readonly WebSocketConnection _con;

    public LocalOcppActor(string id, WebSocketConnection con)
    {
        _id  = id;
        _con = con;
    }

    public async Task ReceiveAsync(IContext context)
    {
        if (context.Message is FromChargepoint fromChargepoint)
        {
            var client = context.Cluster().GetChargePoint(_id);
            await client.HandleFromChargePoint(fromChargepoint, CancellationToken.None);
        }

        else if (context.Message is ToChargepoint toChargepoint)
        {
            await WebSocketConnection.SendStringAsync(_con, toChargepoint.Payload, CancellationToken.None);
        }
        
    }
}