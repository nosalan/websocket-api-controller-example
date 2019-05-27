using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebSocketApiControllerExample
{
    public interface IConnectionManager
    {
        Task HandleConnection(IConnection connection);
    }

    public class ConnectionManager : IConnectionManager
    {
        public async Task HandleConnection(IConnection connection)
        {
            await connection.KeepReceiving();
            await connection.Close();
        }
    }
}
