namespace SpaceBattle.Data
{
    public interface IWeapon : IEntity
    {
        /// <summary>
        /// If target ShieldStrength > 0, it deals full damage to Energy and damage to Health = damage*(1-ShieldStrength), otherwise only to Health.
        /// Part of damage to Health is absorbed by Armor.
        /// </summary>
        int PhysicalDamage { get; }
        /// <summary>
        /// If target ShieldStrength > 0, it deals no damage, otherwise full damage to Energy.
        /// </summary>
        int EnergyDamage { get; }
    }
}