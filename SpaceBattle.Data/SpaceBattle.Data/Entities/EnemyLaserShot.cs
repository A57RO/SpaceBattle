namespace SpaceBattle.Data.Entities
{
    public class EnemyLaserShot : Shot
    {
        private static EntityAction act(GameState state, Location location)
        {
            return new EntityAction { DeltaY = 1 };
        }

        public EnemyLaserShot() : base(act, 10, 25)
        {
        }

        public EnemyLaserShot(FriendlyLaserShot shot) : base(act, shot.PhysicalDamage, shot.EnergyDamage)
        {
        }
    }
}