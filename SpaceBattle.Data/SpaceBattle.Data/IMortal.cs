namespace SpaceBattle.Data
{
    public interface IMortal
    {
        int Health { get; }
        int Armor { get; }
        int Shield { get; }
        double ArmorPenetrationPercentage { get; }
        double ShieldPenetrationPercentage { get; }
    }
}