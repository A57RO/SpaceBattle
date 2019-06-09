namespace GameData.Packets
{
    public class ServerHello : HelloPacket
    {
        public string EnemyName => Name;

        public ServerHello(string enemyName, bool colorIsRed) : base(enemyName, colorIsRed)
        {
        }
    }
}