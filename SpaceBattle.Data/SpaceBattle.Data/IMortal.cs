namespace SpaceBattle.Data
{
    public interface IMortal : IEntity
    {
        int Health { get; }
        double Armor { get; }
        double Shield { get; }
    }
}