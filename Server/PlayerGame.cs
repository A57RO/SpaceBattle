using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameData;

namespace Server
{
    public class PlayerGame
    {
        private readonly GameState gameState;
        private readonly TcpClient client;
        private NetworkStream stream;

        public PlayerGame(TcpClient client)
        {
            this.client = client;
            gameState = new GameState();
            stream = client.GetStream();
        }
    }
}
