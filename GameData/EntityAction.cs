using System;
using System.Collections.Generic;
using GameData.Entities;

namespace GameData
{
    [Serializable]
    public class EntityAction
    {
        public int DeltaX;
        public int DeltaY;
        public Dictionary<IEntity, Location> Spawn = new Dictionary<IEntity, Location>();
    }
}