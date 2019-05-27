namespace SpaceBattle.Data.Entities
{
    public class FriendlyLaserShot : Shot
    {
        private static EntityAction act(GameState state, Location location)
        {
            return new EntityAction {DeltaY = -1};
        }
        
        public FriendlyLaserShot() : base(act, 10, 25)
        {
        }
    }
}