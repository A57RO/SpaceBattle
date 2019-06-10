using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GameData;
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
        }

        public void AddPlayer(PlayerGame player)
        {
            secondPlayer = player;
            Start();
        }

        private void Start()
        {
            
        }
    }
}