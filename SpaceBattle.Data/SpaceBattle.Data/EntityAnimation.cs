using System.Drawing;

namespace SpaceBattle.Data
{
    public class EntityAnimation
    {
        public readonly IEntity Entity;
        public readonly EntityAction Action;
        public readonly Point PositionAtBeginAct;
        public readonly Point TargetLocation;

        public EntityAnimation(IEntity entity, EntityAction action, Point positionAtBeginAct)
        {
            Entity = entity;
            Action = action;
            PositionAtBeginAct = positionAtBeginAct;
            TargetLocation = new Point(PositionAtBeginAct.X + Action.DeltaX, PositionAtBeginAct.Y + Action.DeltaY);
        }
        public EntityAnimation(IEntity entity, EntityAction action) : this(entity, action, entity.Position)
        {
        }
    }
}