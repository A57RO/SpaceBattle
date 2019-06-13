using System;

namespace GameData.Entities
{
    [Serializable]
    public class EnemyLaserShot : Shot
    {
        private static EntityAction EnemyLaserShotAct(GameState state, Location location)
        {
            return new EntityAction { DeltaY = 1 };
        }

        public EnemyLaserShot(int physicalDamage, int energyDamage) : base(physicalDamage, energyDamage, EnemyLaserShotAct)
        {
        }

        public EnemyLaserShot(FriendlyLaserShot shot) : base(shot.PhysicalDamage, shot.EnergyDamage, EnemyLaserShotAct)
        {
        }
    }
}