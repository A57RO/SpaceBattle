using System;
using GameData.ClientInteraction;

namespace GameData.Packets
{
    [Serializable]
    public class ClientUpdate : IPacket
    {
        public readonly GameActCommands Commands;

        public ClientUpdate(GameActCommands commands)
        {
            Commands = commands;
        }
    }
}