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
 
        static void ClientThread(Object stateInfo)
        {
            foreach (var game in Games)
            {
                if (!game.IsReady)
                    game.AddPlayer(new PlayerGame((TcpClient)stateInfo));
                return;
            }
            Games.Add(new Game(new PlayerGame((TcpClient)stateInfo)));
        }

        public void Listen()
        {
            Listener.Start();
            Console.WriteLine("===============");
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            Console.WriteLine("===============");
            while (true)
            {
                ClientThread(Listener.AcceptTcpClient());
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