using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebSocketApiControllerExample.Controllers
{
    [Route("/api/ws")]
    [ApiController]
    public class WebSocketApiController : ControllerBase
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnectionManager _connectionManager;

        public WebSocketApiController(
            IConnectionFactory connectionFactory, 
            IConnectionManager connectionManager)
        {
            _connectionFactory = connectionFactory;
            _connectionManager = connectionManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var context = ControllerContext.HttpContext;

            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine($"Accepted connection '{context.Connection.Id}'");
                var connection = _connectionFactory.CreateConnection(webSocket);
                await _connectionManager.HandleConnection(connection);

                return new EmptyResult();
            }
            else
            {
                return new StatusCodeResult((int) HttpStatusCode.BadRequest);
            }
        }
    }
}