using System.Collections.Concurrent;

namespace OK.OcppServer.Infra;

public class WebSocketConnectionManager
{
    private readonly ConcurrentDictionary<string, OcppConnection?> _connections = new();

    public void Add(OcppConnection connection)
    {
        OcppConnection? old = null;
        _connections.AddOrUpdate(connection.Id, connection, (_, o) =>
        {
            old = o;
            return connection;
        });
        if (old != null)
        {
            OnRemove(old);
        }
    }

    public void Remove(OcppConnection connection)
    {
        // only remove if both Id and the connection instance match. otherwise this connection has already been replaced
        if (_connections.TryRemove(new KeyValuePair<string, OcppConnection>(connection.Id, connection)!))
        {
            OnRemove(connection);
        }
    }

    private void OnRemove(OcppConnection? connection)
    {
    }
}