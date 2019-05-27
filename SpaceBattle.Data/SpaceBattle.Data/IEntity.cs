namespace SpaceBattle.Data
{
    public interface IEntity
    {
        bool DeadInConflictWith(IEntity conflictedEntity);
        EntityAction Act(GameState state, Location location);
    }
}