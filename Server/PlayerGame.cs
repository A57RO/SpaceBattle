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
        public bool IsRed;
        public readonly string Name;
        public readonly GameState State;
        public readonly TcpClient Client;
        public readonly NetworkStream Stream;
        public bool StateInActUpdated;

        public PlayerGame(TcpClient client)
        {
            this.Client = client;
            State = new GameState();
            Stream = client.GetStream();
            var hello = (ClientHello)Network.ReceivePacket(Stream);
            IsRed = hello.ColorIsRed;
            Name = hello.PlayerName;
            StateInActUpdated = false;
        }
    }
}
