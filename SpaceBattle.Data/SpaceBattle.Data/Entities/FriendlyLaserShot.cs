using System.Drawing;

namespace SpaceBattle.Data.Entities
{
    public class FriendlyLaserShot : Shot
    {
        private static EntityAction act(GameState state)
        {
            return new EntityAction {DeltaY = -1};
        }

        //TODO: обработать встречное движение при нахождении в соседних клетках
        public FriendlyLaserShot(Point position) : base(
            position,
            act,
            10,
            25)
        {
        }
    }
}