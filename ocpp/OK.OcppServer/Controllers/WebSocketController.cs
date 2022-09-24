using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using OK.OcppServer.Infra;
using Proto;

namespace OK.OcppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class WebSocketController : ControllerBase
{
    private readonly ILogger<WebSocketController> _logger;
    private readonly ActorSystem _system;
    private readonly WebSocketConnectionManager _webSocketConnectionManager;

    public WebSocketController(WebSocketConnectionManager webSocketConnectionManager, ILogger<WebSocketController> logger, ActorSystem system)
    {
        _webSocketConnectionManager = webSocketConnectionManager;
        _logger                     = logger;
        _system                     = system;
    }

    [HttpGet("/api/v1/ws/{connectionId}")]
    public async Task<IActionResult> GetAsync(string connectionId)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest) return StatusCode(400);
        Request.Headers.TryGetValue("Authorization", out var authorization);
        Request.Headers.TryGetValue("Sec-WebSocket-Protocol", out var subProtocol);

        //TODO: how to handle auth?
        //TODO: how to handle subProtocol?
        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(subProtocol);
        var connection = new WebSocketConnection(connectionId, webSocket, _system);
        _webSocketConnectionManager.Add(connection);
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
            _webSocketConnectionManager.Remove(connection);
        }

        return new EmptyResult();
    }
}