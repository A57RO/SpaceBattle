namespace SpaceBattle.Data
{
    public interface IMortal : IEntity
    {
        int Health { get; }

        /// <summary>
        /// Количество поглощаемого PhysicalDamage.
        /// </summary>
        int Armor { get; }

        /// <summary>
        /// Процент поглощения PhysicalDamage щитом =0 - щит отключён, >0 щит активен, =1 - щит поглощает весь урон.
        /// </summary>
        double ShieldStrength { get; }
    }
}