﻿namespace SpaceBattle.Data.Entities
{
    public class FriendlyLaserShot : Shot
    {
        private static EntityAction FriendlyLaserShotAct(GameState state, Location location)
        {
            return new EntityAction {DeltaY = -1};
        }
        
        public FriendlyLaserShot() : base(FriendlyLaserShotAct, 10, 25)
        {
        }
    }
}