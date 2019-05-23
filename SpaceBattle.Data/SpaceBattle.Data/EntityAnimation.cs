using System.Drawing;

namespace SpaceBattle.Data
{
    public class EntityAnimation
    {
        public IEntity Entity;
        public EntityCommand EntityCommand;
        //public Point Location;
        public Point TargetLocation => new Point(
            Entity.Position.X + EntityCommand.DeltaX, 
            Entity.Position.Y + EntityCommand.DeltaY);

        public EntityAnimation(IEntity entity, EntityCommand entityCommand)
        {
            Entity = entity;
            EntityCommand = entityCommand;
        }
    }
}