using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using OK.OcppServer.Infra;
using Proto;

namespace OK.OcppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class OcppController : ControllerBase
{
    private readonly ILogger<OcppController> _logger;
    private readonly WebSocketConnectionManager _ocppConnectionManager;
    private readonly ActorSystem _system;

    public OcppController(WebSocketConnectionManager ocppConnectionManager, ILogger<OcppController> logger,
        ActorSystem system)
    {
        _ocppConnectionManager = ocppConnectionManager;
        _logger = logger;
        _system = system;
    }

    [HttpGet("/ocpp/{connectionId}")]
    public async Task<IActionResult> GetAsync(string connectionId)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return StatusCode(400);
        }

        Request.Headers.TryGetValue("Authorization", out var authorization);
        Request.Headers.TryGetValue("Sec-WebSocket-Protocol", out var subProtocol);

        //TODO: how to handle auth?
        //TODO: how to handle subProtocol?
        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(subProtocol);
        var connection = new OcppConnection(connectionId, webSocket, _system);
        _ocppConnectionManager.Add(connection);

        try
        {
            await connection.Process();
        }
        catch (WebSocketException x)
        {
            //expected exception on client disconnect etc.
            _logger.LogInformation(x, "...");
        }
        catch (Exception x)
        {
            //unexpected exception
            _logger.LogError(x, "...");
        }
        finally
        {
            _ocppConnectionManager.Remove(connection);
        }

        return new EmptyResult();
    }
}