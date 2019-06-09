using System.Collections.Generic;

namespace GameData
{
    public class EntityAction
    {
        public int DeltaX;
        public int DeltaY;
        public Dictionary<IEntity, Location> Spawn = new Dictionary<IEntity, Location>();
    }
}