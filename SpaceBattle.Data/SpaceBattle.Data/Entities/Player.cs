using SpaceBattle.Data.ClientInteraction;

namespace SpaceBattle.Data.Entities
{
    public class Player : IMortal
    {
        private readonly int maxHeath;
        private int health;
        public int Health
        {
            get => health;
            private set => health = value > maxHeath ? maxHeath : value < 0 ? 0 : value;
        }

        private readonly int maxEnergy;
        private int energy;
        public int Energy
        {
            get => energy;
            private set => energy = value > maxEnergy ? maxEnergy : value < 0 ? 0 : value;
        }

        public int Armor { get; private set; }

        private readonly double maxShieldStrength;
        private double shieldStrength;
        public double ShieldStrength
        {
            get => shieldStrength;
            private set => shieldStrength = value > maxShieldStrength ? maxShieldStrength : value < 0 ? 0 : value;
        }
        
        public bool DeadInConflictWith(IEntity conflictedEntity)
        {
            if (conflictedEntity is IWeapon weapon)
            {
                Energy -= weapon.EnergyDamage;
                if (ShieldStrength > 0)
                {
                    var penetratingDamage = weapon.PhysicalDamage - (int)(weapon.PhysicalDamage * (1 - ShieldStrength));
                    if (penetratingDamage > Armor)
                        Health -= penetratingDamage + Armor;
                }
                else
                    Health -= weapon.PhysicalDamage + Armor;
            }

            return Health <= 0;
        }

        public EntityAction Act(GameState state, Location location)
        {
            var action = new EntityAction();

            var systemCommand = state.CommandsFromClient.Systems;
            if (systemCommand == ClientCommand.ActivateShield && Energy >= 2)
            {
                Energy -= 2;
                ShieldStrength = maxShieldStrength;
            }
            else
            {
                ShieldStrength = 0;
                if (systemCommand == ClientCommand.OpenFire && Energy >= 10)
                {
                    Energy -= 10;
                    action.Spawn.Add(new FriendlyLaserShot(), new Location(location.Y - 1, location.X));
                }
            }

            if (Energy >= 5)
            {
                var horizontalMoveCommand = state.CommandsFromClient.HorizontalMove;
                if (horizontalMoveCommand != ClientCommand.Idle)
                {
                    Energy -= 5;
                    var deltaX = horizontalMoveCommand == ClientCommand.MoveRight ? 1 : -1;
                    if (state.IsInsideGameField(location.Y, location.X + deltaX))
                        action.DeltaX = deltaX;
                }
                else
                {
                    var verticalMoveCommand = state.CommandsFromClient.VerticalMove;
                    if (verticalMoveCommand != ClientCommand.Idle)
                    {
                        Energy -= 5;
                        var deltaY = verticalMoveCommand == ClientCommand.MoveDown ? 1 : -1;
                        if (state.IsInsideGameField(location.Y + deltaY, location.X))
                            action.DeltaY = deltaY;
                    }
                }
            }

            Energy++;
            return action;
        }

        public Player()
        {
            //Position = position;

            maxHeath = 100;
            Health = maxHeath;
            maxEnergy = 100;
            Energy = maxEnergy;
            Armor = 1;
            maxShieldStrength = 0.8;
            ShieldStrength = 0;
        }
    }
}