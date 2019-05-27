using System;
using System.Collections.Generic;
using System.Linq;
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

        public WebSocketApiController(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var context = ControllerContext.HttpContext;

            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine($"Accepted connection {context.Connection.Id}");
                var webSocketConnection = _connectionFactory.CreateConnection(webSocket);
                await webSocketConnection.KeepReceiving();
                await webSocketConnection.Close();

                return new EmptyResult();
            }
            else
            {
                return new StatusCodeResult((int) HttpStatusCode.BadRequest);
            }
        }
    }
}