using System;

namespace GameData.Entities
{
    [Serializable]
    public class FriendlyLaserShot : Shot
    {
        private static EntityAction FriendlyLaserShotAct(GameState state, Location location)
        {
            return new EntityAction {DeltaY = -1};
        }
        
        public FriendlyLaserShot(int physicalDamage, int energyDamage) : base(physicalDamage, energyDamage, FriendlyLaserShotAct)
        {
        }
    }
}