using System.Net.WebSockets;
using System.Text;

namespace OK.OcppServer.Infra;

public static class SocketExtensions
{
    public static async Task<string> ReceiveStringAsync(this WebSocket socket, CancellationToken ct = default)
    {
        var buffer = new byte[1000];
        using var ms = new MemoryStream();
        WebSocketReceiveResult result;

        do
        {
            ct.ThrowIfCancellationRequested();
            result = await socket.ReceiveAsync(buffer, ct);
            ms.Write(buffer, 0, result.Count);
        } while (!result.EndOfMessage);

        ms.Seek(0, SeekOrigin.Begin);

        if (result.MessageType != WebSocketMessageType.Text || result.Count.Equals(0))
        {
            throw new Exception("Unexpected message");
        }

        using var reader = new StreamReader(ms, Encoding.UTF8);

        return await reader.ReadToEndAsync();
    }

    //StackOverflow code: https://stackoverflow.com/questions/46457674/asp-net-core-with-websockets-websocket-handshake-never-occurs
    public static Task SendStringAsync(this WebSocket socket, string data, CancellationToken ct = default)
    {
        var segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));

        return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
    }
}