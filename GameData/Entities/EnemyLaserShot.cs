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

        public EnemyLaserShot() : base(EnemyLaserShotAct, 10, 25)
        {
        }

        public EnemyLaserShot(FriendlyLaserShot shot) : base(EnemyLaserShotAct, shot.PhysicalDamage, shot.EnergyDamage)
        {
        }
    }
}