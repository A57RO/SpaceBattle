using System;
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
            while (true)
            {
                try
                {
                    Task.Run(() => ListenClient(firstPlayer));
                    Task.Run(() => ListenClient(secondPlayer));
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        private void ListenClient(PlayerGame player)
        {
            while (true)
            {
                var clientUpdate = (ClientUpdate)Network.ReceivePacket(player.stream);
                player.gameState.GiveCommandsFromClient(clientUpdate.Commands);
                GameEngine.BeginAct(player.gameState);
                Network.SendPacket(new ServerUpdate(player.IsRed, player.gameState.Animations), firstPlayer.stream);
                Network.SendPacket(new ServerUpdate(player.IsRed, player.gameState.Animations), secondPlayer.stream);
                GameEngine.EndAct(firstPlayer.gameState, secondPlayer.gameState);
            }
        }


    }
}