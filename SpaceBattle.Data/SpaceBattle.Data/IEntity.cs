using System.Drawing;

namespace SpaceBattle.Data
{
    public interface IEntity
    {
        Point Position { get; set; }
        bool DeadInConflictWith(IEntity conflictedEntity);
        EntityAction Act(GameState state);
    }
}