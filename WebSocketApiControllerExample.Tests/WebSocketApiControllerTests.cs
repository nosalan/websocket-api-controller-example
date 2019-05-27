using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebSocketApiControllerExample.Controllers;
using static TddXt.AnyRoot.Root;

namespace WebSocketApiControllerExample.Tests
{
    public class WebSocketApiControllerTests
    {
        [Test]
        public async Task ShouldReturnBadRequestIfRequestIsNotAWebSocketRequest()
        {
            //GIVEN
            var controller = CreateController(Any.Instance<IConnectionFactory>(),
                Any.Instance<IConnectionManager>());
            controller.ControllerContext.HttpContext.WebSockets.IsWebSocketRequest.Returns(false);

            //WHEN
            var result = await controller.Get();


            //THEN
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(400, ((StatusCodeResult) result).StatusCode);
        }

        [Test]
        public async Task ShouldAcceptConnectionIfRequestIsAWebSocketRequest()
        {
            //GIVEN
            var connectionFactory = Substitute.For<IConnectionFactory>();
            var webSocket = Any.Instance<WebSocket>();
            var connectionManager = Substitute.For<ConnectionManager>();
            var connection = Substitute.For<IConnection>();
            connectionFactory.CreateConnection(webSocket).Returns(connection);

            var controller = CreateController(connectionFactory, connectionManager);
            controller.ControllerContext.HttpContext.WebSockets.IsWebSocketRequest.Returns(true);
            controller.ControllerContext.HttpContext.WebSockets.AcceptWebSocketAsync().Returns(webSocket);
            
            //WHEN
            var result = await controller.Get();
            
            //THEN
            Assert.IsInstanceOf<EmptyResult>(result);
            await connectionManager.Received().HandleConnection(connection);
        }


        private static WebSocketApiController CreateController(IConnectionFactory connectionFactory,
            IConnectionManager connectionManager)
        {
            var controller = new WebSocketApiController(connectionFactory, connectionManager)
            {
                ControllerContext = Substitute.For<ControllerContext>()
            };
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.ControllerContext.HttpContext.WebSockets.Returns(Substitute.For<WebSocketManager>());
            return controller;
        }
    }
}