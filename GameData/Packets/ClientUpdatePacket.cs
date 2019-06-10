using System;
using GameData.ClientInteraction;

namespace GameData.Packets
{
    [Serializable]
    public class ClientUpdatePacket
    {
        public readonly GameActCommands Commands;

        public ClientUpdatePacket(GameActCommands commands)
        {
            Commands = commands;
        }
    }
}