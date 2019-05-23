namespace SpaceBattle.Data
{
    public interface IWeapon : IEntity
    {
        int PhysicalDamage { get; }
        int EnergyDamage { get; }
        //int DeltaX { get; }
        //int DeltaY { get; }
    }
}