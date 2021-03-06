using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server;

namespace SpaceBattle.Server
{
    class Server
    {
        TcpListener Listener;
        private static List<Game> Games = new List<Game>();

        public Server(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port);
            
        }
 
        static void ClientThread(object stateInfo)
        {
            var client = (TcpClient) stateInfo;
            foreach (var game in Games)
            {
                if (game.IsReady) continue;
                game.AddPlayer(new PlayerGame(client));
                return;
            }
            Games.Add(new Game(new PlayerGame(client)));
        }

        public void Listen()
        {
            Listener.Start();
            Console.WriteLine("===============");
            Console.WriteLine("������ �������. �������� �����������...");
            Console.WriteLine("===============");
            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());
            }
        }

        ~Server()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }
    }
}