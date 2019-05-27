using System;

namespace SpaceBattle.Data.Entities
{
    public abstract class Shot : IWeapon
    {
        public int PhysicalDamage { get; protected set; }
        public int EnergyDamage { get; protected set; }

        public bool DeadInConflictWith(IEntity conflictedEntity) => true;
        
        private readonly Func<GameState, Location, EntityAction> act;
        public EntityAction Act(GameState state, Location location) => act(state, location);

        protected Shot(Func<GameState, Location, EntityAction> act, int physicalDamage, int energyDamage)
        {
            PhysicalDamage = physicalDamage;
            EnergyDamage = energyDamage;

            this.act = act;
        }
    }
}