using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameData;


namespace SpaceBattle.Server
{
    public class GameClient
    {
        public readonly IPEndPoint Address;
        public readonly GameState State;

        public GameClient(TcpClient client)
        { 
            Console.WriteLine(client.Connected);
            // получаем сетевой поток для чтения и записи
            NetworkStream stream = client.GetStream();
            // Буфер для хранения принятых от клиента данных
            byte[] Buffer = new byte[1024];
            
           
        }
    }
}