using System;
using System.Collections.Generic;

namespace GameData.Packets
{
    [Serializable]
    public class ServerUpdate : IPacket
    {
        public readonly bool SideColorIsRed;
        public readonly List<EntityAnimation> Animations;

        public ServerUpdate(bool sideColorIsRed, List<EntityAnimation> animations)
        {
            SideColorIsRed = sideColorIsRed;
            Animations = animations;
        }
    }
}