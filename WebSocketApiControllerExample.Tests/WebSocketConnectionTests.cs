using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace WebSocketApiControllerExample.Tests
{
    class WebSocketConnectionTests
    {
        [Test]
        public async Task ShouldResendReceivedMessages()
        {
            //GIVEN
            var webSocket = Substitute.For<WebSocket>();
            var connection = new WebSocketConnection(webSocket);

            webSocket.ReceiveAsync(Arg.Any<ArraySegment<byte>>(), Arg.Any<CancellationToken>())
                .Returns(args =>
                    {
                        new ArraySegment<byte>(new byte[] { 1, 2, 3 }).CopyTo((ArraySegment<byte>)args[0]);
                        return Task.FromResult(new WebSocketReceiveResult(3, WebSocketMessageType.Binary, false));
                    },
                    args =>
                    {
                        new ArraySegment<byte>(new byte[] { 4, 5, 6 }).CopyTo((ArraySegment<byte>)args[0]);
                        return Task.FromResult(new WebSocketReceiveResult(3, WebSocketMessageType.Close, true));
                    });


       
            //WHEN
            await connection.KeepReceiving();

            //THEN
            await webSocket.Received()
                .SendAsync(Arg.Is<ArraySegment<byte>>(segment => segment.Array.SequenceEqual(new byte[] {1, 2, 3, 4, 5, 6})),
                    WebSocketMessageType.Text, true, CancellationToken.None);
        }

        [Test]
        public async Task ShouldCloseUnderlyingWebSocketOnClose()
        {
            // GIVEN
            WebSocket webSocket = Substitute.For<WebSocket>();
            var connection = new WebSocketConnection(webSocket);

            // WHEN
            await connection.Close();

            // THEN
            await webSocket.Received().CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}
