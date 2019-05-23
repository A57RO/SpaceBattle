using System.Drawing;

namespace SpaceBattle.Data
{
    public interface IEntity
    {
        /// <summary>
        /// Higher value - sprite closer to the screen
        /// </summary>
        int DrawingPriority { get; }
        Point Position { get; }
        bool DeadInConflictWith(IEntity conflictedEntity);
        EntityCommand Act(GameState state);
    }
}