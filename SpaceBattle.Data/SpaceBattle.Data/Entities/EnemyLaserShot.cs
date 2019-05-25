using System.Drawing;

namespace SpaceBattle.Data.Entities
{
    public class EnemyLaserShot : Shot
    {
        private static EntityAction act(GameState state)
        {
            return new EntityAction { DeltaY = 1 };
        }

        public EnemyLaserShot(Point position) : base(
            position,
            act,
            10,
            25)
        {
        }

        public EnemyLaserShot(FriendlyLaserShot shot) : base(
            shot.Position,
            act,
            shot.PhysicalDamage,
            shot.EnergyDamage)
        {
        }
    }
}