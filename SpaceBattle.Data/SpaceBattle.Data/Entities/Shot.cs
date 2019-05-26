using System;
using System.Drawing;

namespace SpaceBattle.Data.Entities
{
    public abstract class Shot : IWeapon
    {
        public Point Position { get; set; }

        public int PhysicalDamage { get; private set; }
        public int EnergyDamage { get; private set; }

        public bool DeadInConflictWith(IEntity conflictedEntity) => true;
        
        private readonly Func<GameState, EntityAction> act;
        public EntityAction Act(GameState state) => act(state);

        protected Shot(Point position, Func<GameState, EntityAction> act, int physicalDamage, int energyDamage)
        {
            Position = position;

            PhysicalDamage = physicalDamage;
            EnergyDamage = energyDamage;

            this.act = act;
        }
    }
}