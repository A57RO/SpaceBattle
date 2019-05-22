namespace SpaceBattle.Data
{
    public interface IGameObject
    {
        string ImageFileName { get; }
        int DrawingPriority { get; }
        bool DeadInConflict(IGameObject conflictedObject);
        Command Act(int x, int y);
    }
}