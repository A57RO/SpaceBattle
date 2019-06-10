namespace GameData.Packets
{
    public abstract class HelloPacket
    {
        protected readonly string Name;

        public readonly bool ColorIsRed;

        protected HelloPacket(string name, bool colorIsRed)
        {
            Name = name;
            ColorIsRed = colorIsRed;
        }
    }
}