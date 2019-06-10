using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GameData;
using GameData.Packets;
using Server;


namespace SpaceBattle.Server
{
    public class Game
    {
        private PlayerGame firstPlayer;
        private PlayerGame secondPlayer = null;
        public bool IsReady => secondPlayer != null;

        public Game(PlayerGame player)
        {
            firstPlayer = player;
            Network.SendPacket(new ServerHello(player.IsRed), player.stream);
        }

        public void AddPlayer(PlayerGame player)
        {
            secondPlayer = player;
            Network.SendPacket(new ServerHello(!firstPlayer.IsRed, firstPlayer.Name), player.stream);
            Network.SendPacket(new ServerHello(firstPlayer.IsRed, secondPlayer.Name), firstPlayer.stream);
            Start();
        }

        private void Start()
        {
            
        }
    }
}