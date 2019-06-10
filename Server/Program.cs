using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Server";
            var server = new SpaceBattle.Server.Server(1997);
            server.Listen();
        }
    }
}