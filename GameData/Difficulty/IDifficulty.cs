namespace GameData.Difficulty
{
    public interface IDifficulty
    {
        int CurrentDifficulty { get; }
        int DifficultyIncreaseRate { get; }
        int MaxDifficulty { get; }
    }
}