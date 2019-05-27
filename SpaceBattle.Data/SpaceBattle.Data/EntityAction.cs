using System.Collections.Generic;

namespace SpaceBattle.Data
{
    public class EntityAction
    {
        public int DeltaX;
        public int DeltaY;
        public Dictionary<IEntity, Location> Spawn = new Dictionary<IEntity, Location>();
    }
}