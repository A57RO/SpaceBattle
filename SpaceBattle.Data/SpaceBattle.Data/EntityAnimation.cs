using System.Drawing;

namespace SpaceBattle.Data
{
    public class EntityAnimation
    {
        public IEntity Entity;
        public EntityAction Action;
        //public Point Location;
        public Point TargetLocation => new Point(
            Entity.Position.X + Action.DeltaX, 
            Entity.Position.Y + Action.DeltaY);

        public EntityAnimation(IEntity entity, EntityAction action)
        {
            Entity = entity;
            Action = action;
        }
    }
}