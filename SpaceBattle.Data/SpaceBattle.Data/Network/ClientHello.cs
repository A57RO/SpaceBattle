namespace SpaceBattle.Data.Network
{
    public class ClientHello : HelloPacket
    {
        public string PlayerName => Name;

        public ClientHello(string playerName, bool colorIsRed) : base(playerName, colorIsRed)
        {
        }
    }
}