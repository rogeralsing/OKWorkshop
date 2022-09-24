using System.Net.WebSockets;
using OK.Messages;
using Proto;

namespace OK.OcppServer.Infra;

public class OcppConnection
{
    private readonly CancellationTokenSource _cts;
    private readonly ActorSystem _system;
    private readonly WebSocket _webSocket;

    public OcppConnection(string id, WebSocket webSocket, ActorSystem system)
    {
        _webSocket = webSocket;
        Id = id;
        _cts = new CancellationTokenSource();
        _system = system;
    }

    public string Id { get; }

    public async Task Process()
    {
        var props = Props.FromProducer(() => new LocalOcppActor(Id, this));
        var pid = _system.Root.Spawn(props);
        //spawn actor for this ID
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var msg = await _webSocket.ReceiveStringAsync(_cts.Token);
                await OnMessage(msg, pid);
            }
        }
        finally
        {
            await _system.Root.StopAsync(pid);
        }

        //finally/catch
        //despawn
    }

    private Task OnMessage(string msg, PID pid)
    {
        _system.Root.Send(pid, new FromChargepoint
        {
            Payload = msg
        });
        return Task.CompletedTask;
    }

    //StackOverflow code: https://stackoverflow.com/questions/46457674/asp-net-core-with-websockets-websocket-handshake-never-occurs

    public async Task SendStringAsync(string payload, CancellationToken ct)
    {
        await _webSocket.SendStringAsync(payload, ct);
    }
}