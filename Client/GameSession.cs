using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Client.Forms;
using GameData;
using GameData.ClientInteraction;
using GameData.Entities;
using GameData.Packets;

namespace Client
{
    public class GameSession
    {
        public volatile GameForm GameForm;
        private volatile GameState topSideState;
        private volatile GameState bottomSideState;
        private readonly ControlSettings controlSettings;
        private TcpClient server;
        private NetworkStream serverConnection;
        private bool playerIsRed;
        private string playerName;
        private string enemyName;
        private bool gameInProcess;
        private int tickCount = 0;
        //private int disconnectedActs = 0;

        public bool Connected => server != null && server.Connected && playerName != null;

        public GameSession(int mapWidth, int bottomMapHeight, int topSideMapHeight, ControlSettings controlSettings)
        {
            this.controlSettings = controlSettings;
            bottomSideState = new GameState(bottomMapHeight, mapWidth);
            topSideState = new GameState(topSideMapHeight, mapWidth);
            GameForm = new GameForm(mapWidth, bottomMapHeight + topSideMapHeight);
            gameInProcess = false;
        }

        public bool ConnectToServer(IPEndPoint serverAddress, bool playerIsRed, string playerName)
        {
            var clientHello = new ClientHello(playerIsRed, playerName);
            server = new TcpClient();
            server.Connect(serverAddress);
            serverConnection = server.GetStream();
            Network.SendPacket(clientHello, serverConnection);
            var serverHello = (ServerHello)Network.ReceivePacket(serverConnection);

            this.playerName = playerName;
            this.playerIsRed = serverHello.ColorIsRed;
            if (serverHello.EnemyName == null)
                return false;
            enemyName = serverHello.EnemyName;
            return true;
        }

        public void WaitForEnemy()
        {
            var serverHello = (ServerHello)Network.ReceivePacket(serverConnection);
            enemyName = serverHello.EnemyName;
        }

        public void Start()
        {
            if (!Connected)
                throw new MethodAccessException("Метод Start() был вызван до подключения к серверу");
            if (enemyName == null)
                throw new MethodAccessException("Метод Start() был вызван до подключения второго игрока");

            GameForm.Text = $@": {playerName} vs {enemyName}";

            var timer = new System.Windows.Forms.Timer {Interval = 10};
            timer.Tick += OnTick;

            gameInProcess = true;
            Task.Run(() => StartListenServer());
            timer.Start();
        }

        public void StartSolo()
        {
            var timer = new System.Windows.Forms.Timer { Interval = 10 };
            timer.Tick += OnTick;

            gameInProcess = true;
            timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (gameInProcess)
            {
                if (tickCount == 0) BeginSessionAct();

                lock (bottomSideState)
                {
                    lock (GameForm.BottomSideHUD)
                    {
                        GameForm.BottomSideHUD.Clear();
                        if (!bottomSideState.GameOver)
                            Visual.UpdatePlayerHUD(GameForm.BottomSideHUD, bottomSideState.PlayerEntity,
                                GameForm.ClientSize, true);
                    }

                    lock (GameForm.BottomSideField)
                    {
                        GameForm.BottomSideField.Clear();
                        Visual.UpdateGameField(GameForm.BottomSideField, bottomSideState, true, playerIsRed, tickCount);
                    }
                }

                lock (topSideState)
                {
                    lock (GameForm.TopSideHUD)
                    {
                        GameForm.TopSideHUD.Clear();
                        if (!topSideState.GameOver)
                            Visual.UpdatePlayerHUD(GameForm.TopSideHUD, topSideState.PlayerEntity, GameForm.ClientSize,
                                false);
                    }

                    lock (GameForm.TopSideField)
                    {
                        GameForm.TopSideField.Clear();
                        Visual.UpdateGameField(GameForm.TopSideField, topSideState, false, !playerIsRed, tickCount);
                    }
                }

                tickCount++;
                if (tickCount == 9) EndSessionAct();
            }

            GameForm.Invalidate();
        }

        private void BeginSessionAct()
        {
            var commands = new GameActCommands(controlSettings, GameForm.PressedKeys);

            lock (bottomSideState)
            {
                lock (commands)
                {
                    bottomSideState.GiveCommandsFromClient(commands);
                }
                Task.Run(() => SendCommandsToServer(commands));
                GameEngine.BeginAct(bottomSideState);
                if (!bottomSideState.GameOver)
                    Sound.PlaySoundsAtBeginAct(bottomSideState.PlayerEntity);
            }

            lock (topSideState)
            {
                GameEngine.BeginAct(topSideState);
            }
        }

        private void EndSessionAct()
        {
            lock (bottomSideState)
            {
                lock (topSideState)
                    GameEngine.EndAct(bottomSideState, topSideState);
                if (bottomSideState.GameOver || topSideState.GameOver)
                {
                    gameInProcess = false;
                }
                if (!bottomSideState.GameOver)
                    Sound.PlaySoundsAtEndAct(bottomSideState.PlayerEntity);
            }
            tickCount = 0;
        }

        private void SendCommandsToServer(GameActCommands commands)
        {
            lock (commands)
            {
                var clientUpdate = new ClientUpdate(commands);
                Network.SendPacket(clientUpdate, serverConnection);
                /*
                try
                {
                    Network.SendPacket(clientUpdate, server.GetStream());
                }
                catch (Exception e)
                {
                    disconnectedActs++;
                }
                */
            }
        }

        private void StartListenServer()
        {
            while (gameInProcess)
            {

                var serverUpdate = (ServerUpdate)Network.ReceivePacket(serverConnection);
                if (serverUpdate == null)
                    throw new Exception();
                var updatedAnimations = serverUpdate.Animations;
                if (updatedAnimations == null)
                    throw new Exception();
                if (updatedAnimations.Count == 0)
                    throw new Exception();
                if (!updatedAnimations.Any(a => a.Entity is Player))
                    throw new Exception();
                if (playerIsRed && serverUpdate.SideColorIsRed || !playerIsRed && !serverUpdate.SideColorIsRed)
                {
                    lock (bottomSideState)
                    {
                        bottomSideState.UpdateStateInAct(updatedAnimations);
                    }
                }
                else
                {
                    lock (topSideState)
                    {
                        topSideState.UpdateStateInAct(updatedAnimations);
                    }
                }
            }
            //server.Close();
            //server.Dispose();
        }
    }
}