using System;

namespace GameData.Packets
{
    [Serializable]
    public sealed class ClientHello : IPacket
    {
        public readonly bool ColorIsRed;
        public readonly string PlayerName;

        public ClientHello(bool colorIsRed, string playerName)
        {
            ColorIsRed = colorIsRed;
            PlayerName = playerName;
        }
    }
}