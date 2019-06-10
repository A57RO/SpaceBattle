using System;
using System.Collections.Generic;

namespace GameData.Packets
{
    [Serializable]
    public class ServerUpdatePacket
    {
        public readonly bool SideColorIsRed;
        public readonly List<EntityAnimation> Animations;

        public ServerUpdatePacket(bool sideColorIsRed, List<EntityAnimation> animations)
        {
            SideColorIsRed = sideColorIsRed;
            Animations = animations;
        }
    }
}