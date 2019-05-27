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
            var controller = new WebSocketApiController(Any.Instance<IConnectionFactory>());
            controller.ControllerContext = Substitute.For<ControllerContext>();
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.ControllerContext.HttpContext.WebSockets.Returns(Substitute.For<WebSocketManager>());
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
            var controller = new WebSocketApiController(Any.Instance<IConnectionFactory>());
            controller.ControllerContext = Substitute.For<ControllerContext>();
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.ControllerContext.HttpContext.WebSockets.Returns(Substitute.For<WebSocketManager>());
            controller.ControllerContext.HttpContext.WebSockets.IsWebSocketRequest.Returns(true);
            var anyWebSocket = Any.Instance<WebSocket>();
            controller.ControllerContext.HttpContext.WebSockets.AcceptWebSocketAsync().Returns(anyWebSocket);

            //WHEN
            var result = await controller.Get();


            //THEN
            Assert.IsInstanceOf<EmptyResult>(result);
            Assert.AreEqual(400, ((StatusCodeResult)result).StatusCode);
        }
    }
}