using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameData;
using GameData.Packets;

namespace Server
{
    public class PlayerGame
    {
        public readonly GameState gameState;
        private readonly TcpClient client;
        public NetworkStream stream;
        public bool IsRed;
        public string Name;

        public PlayerGame(TcpClient client)
        {
            this.client = client;
            gameState = new GameState();
            stream = client.GetStream();
            var hello = (ClientHello)Network.ReceivePacket(stream);
            IsRed = hello.ColorIsRed;
            Name = hello.PlayerName;
        }
    }
}
