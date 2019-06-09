namespace GameData
{
    public class EntityAnimation
    {
        public readonly IEntity Entity;
        public readonly EntityAction Action;
        public readonly Location BeginActLocation;
        public readonly Location TargetLocation;

        public EntityAnimation(IEntity entity, EntityAction action, Location beginActLocation)
        {
            Entity = entity;
            Action = action;
            BeginActLocation = beginActLocation;
            TargetLocation = new Location(BeginActLocation.Y + Action.DeltaY, BeginActLocation.X + Action.DeltaX);
        }
    }
}