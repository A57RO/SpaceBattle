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
            Network.SendPacket(new ServerHello(player.IsRed), player.Stream);
        }

        public void AddPlayer(PlayerGame player)
        {
            player.IsRed = !firstPlayer.IsRed;
            secondPlayer = player;
            Task.Run(() => Network.SendPacket(new ServerHello(!firstPlayer.IsRed, firstPlayer.Name), player.Stream));
            Task.Run(() => Network.SendPacket(new ServerHello(firstPlayer.IsRed, secondPlayer.Name), firstPlayer.Stream));
            Start();
        }

        private void Start()
        {
            Task.Run(() => ListenClient(firstPlayer));
            Task.Run(() => ListenClient(secondPlayer));
        }

        private void ListenClient(PlayerGame player)
        {
            while (true)
            {
                var clientUpdate = (ClientUpdate)Network.ReceivePacket(player.Stream);
                if (clientUpdate == null || player.StateInActUpdated) continue;
                lock (firstPlayer.State)
                {
                    lock (secondPlayer.State)
                    {
                        if (firstPlayer.State.GameOver || secondPlayer.State.GameOver)
                            throw new Exception();
                        if (firstPlayer.State.Animations == null || firstPlayer.State.Animations.Count == 0)
                            throw new Exception();
                        player.State.GiveCommandsFromClient(clientUpdate.Commands);
                        GameEngine.BeginAct(player.State);
                        if (firstPlayer.State.GameOver || secondPlayer.State.GameOver)
                            throw new Exception();
                        if (firstPlayer.State.Animations == null || firstPlayer.State.Animations.Count == 0)
                            throw new Exception();

                        player.StateInActUpdated = true;
                        Task.Run(() => Network.SendPacket(new ServerUpdate(player.IsRed, player.State.Animations), firstPlayer.Stream));
                        Network.SendPacket(new ServerUpdate(player.IsRed, player.State.Animations), secondPlayer.Stream);

                        if (firstPlayer.StateInActUpdated && secondPlayer.StateInActUpdated)
                        {
                            GameEngine.EndAct(firstPlayer.State, secondPlayer.State);
                            if (firstPlayer.State.GameOver || secondPlayer.State.GameOver)
                                throw new Exception();
                            if (firstPlayer.State.Animations == null || firstPlayer.State.Animations.Count == 0)
                                throw new Exception();
                            firstPlayer.StateInActUpdated = secondPlayer.StateInActUpdated = false;
                        }
                    }
                }
            }
        }
    }
}