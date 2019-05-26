namespace SpaceBattle.Data
{
    public interface IWeapon : IEntity
    {
        /// <summary>
        /// Если у цели ShieldStrength > 0, то Health -= PhysicalDamage*(1-ShieldStrength), иначе Health -= PhysicalDamage.
        /// Часть урона по Health поглощается Armor.
        /// </summary>
        int PhysicalDamage { get; }
        /// <summary>
        /// Если у цели ShieldStrength > 0, то Energy -= EnergyDamage, иначе не наносит никакого урона.
        /// </summary>
        int EnergyDamage { get; }
    }
}