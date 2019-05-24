namespace SpaceBattle.Data
{
    public interface IMortal : IEntity
    {
        int Health { get; }

        /// <summary>
        /// Amount of PhysicalDamage absorbed by armor
        /// </summary>
        int Armor { get; }

        /// <summary>
        /// Shield damage absorption percentage. =0 - shield deactivated, >0 shield activated, =1 - shield absorbs all damage
        /// </summary>
        double ShieldStrength { get; }
    }
}