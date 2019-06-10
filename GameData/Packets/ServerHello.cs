using System;

namespace GameData.Packets
{
    [Serializable]
    public class ServerHello
    {
        public readonly bool ColorIsRed;
        public readonly string EnemyName;

        public ServerHello(bool colorIsRed, string enemyName)
        {
            ColorIsRed = colorIsRed;
            EnemyName = enemyName;
        }
    }
}