using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SpaceBattle.Data;

namespace SpaceBattle.Server
{
    public class GameClient
    {
        public readonly IPEndPoint Address;
        public readonly GameState State;
    }
}