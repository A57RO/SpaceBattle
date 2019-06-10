using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SpaceBattle.Server
{
    class Server
    {
        TcpListener Listener;

        public Server(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port);
            
        }
 
        static void ClientThread(Object stateInfo)
        {
            new GameClient((TcpClient)stateInfo);
        }

        public void Listen()
        {
            Listener.Start();
            Console.WriteLine("===============");
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
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