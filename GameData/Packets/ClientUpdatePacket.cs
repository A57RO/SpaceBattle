using GameData.ClientInteraction;

namespace GameData.Packets
{
    public class ClientUpdatePacket
    {
        public readonly GameActCommands commands;

        public ClientUpdatePacket(GameActCommands commands)
        {
            this.commands = commands;
        }
    }
}