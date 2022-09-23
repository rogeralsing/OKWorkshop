using System.Net.WebSockets;
using System.Text;
using OK.Messages;
using Proto;

namespace OK.OcppServer.Infra;

public class WebSocketConnection
{
    private readonly WebSocket _webSocket;
    private readonly CancellationTokenSource _cts;
    private ActorSystem _system;

    public WebSocketConnection(string id, WebSocket webSocket, ActorSystem system)
    {
        _webSocket = webSocket;
        Id         = id;
        _cts       = new CancellationTokenSource();
        _system    = system;
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
                var msg = await ReceiveStringAsync(_webSocket, _cts.Token);
                await OnMessage(msg, pid);
            }
        }
        finally
        {
            _system.Root.Stop(pid);
        }

        //finally/catch
        //despawn
    }

    private Task OnMessage(string msg, PID pid)
    {
        _system.Root.Send(pid, new FromChargepoint()
        {
            Payload = msg
        });
        return Task.CompletedTask;
    }

    //StackOverflow code: https://stackoverflow.com/questions/46457674/asp-net-core-with-websockets-websocket-handshake-never-occurs
    private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default)
    {
        var buffer = new byte[1000];
        using var ms = new MemoryStream();
        WebSocketReceiveResult result;
        do
        {
            ct.ThrowIfCancellationRequested();

            result = await socket.ReceiveAsync(buffer, ct);
            ms.Write(buffer,0, result.Count);
        }
        while (!result.EndOfMessage);

        ms.Seek(0, SeekOrigin.Begin);
        if (result.MessageType != WebSocketMessageType.Text || result.Count.Equals(0))
        {
            throw new Exception("Unexpected message");
        }

        using var reader = new StreamReader(ms, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
 
    //StackOverflow code: https://stackoverflow.com/questions/46457674/asp-net-core-with-websockets-websocket-handshake-never-occurs
    public  static Task SendStringAsync( WebSocketConnection socket, string data, CancellationToken ct = default)
    {
        var segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
        return socket._webSocket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
    }
}