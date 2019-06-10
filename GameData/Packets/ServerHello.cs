using System;

namespace GameData.Packets
{
    [Serializable]
    public class ServerHello : IPacket
    {
        public readonly bool ColorIsRed;
        public readonly string EnemyName;

        public ServerHello(bool colorIsRed, string enemyName = null)
        {
            ColorIsRed = colorIsRed;
            EnemyName = enemyName;
        }
    }
}