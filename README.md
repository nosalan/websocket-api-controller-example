# websocket-api-controller-example

This is an example WebSocket server API with ASP.NET Core MVC approach. The WebSocket server is implemented in a Controller class, in the same way as a standard REST controller is usually implemented.    

Separate controller is thin, unit testable and more readable than middleware. If the request is not a WebSocket request (is missing web socket upgrade headers) then 400 is returned.    

This is an excerpt from the controller class:



```c#
    [Route("/api/ws")]
    [ApiController]
    public class WebSocketApiController : ControllerBase
    {
        ...

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
```

The server listens for messages from clients and responds to each message with the same message (echo)\
The server listens for messages on `ws://localhost:5000/api/ws` and `wss://localhost:5001/api/ws`\    

The `WebSocket` instance is wrapped by the `WebSocketConnection` class that exposes standard `WebSocket` methods like Receive, Send, Close. The class is separately testable and keeps the websocket logic away from Controller.

The solution is fully asynchronous and nonblocking.

Additionally the solution features the `AddControllersAsServices` approach which means the Controller classes are created in code and not by framework.
