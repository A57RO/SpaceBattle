﻿using System;
using GameData.ClientInteraction;

namespace GameData.Entities
{
    [Serializable]
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

        public int WeaponPhysicalDamage { get; private set; }
        public int WeaponEnergyDamage { get; private set; }

        private readonly int thrusterCost;
        private readonly int shieldCost;
        private readonly int fireCost;

        public bool ThrustersRunning { get; private set; }
        public bool ShieldTakingHit { get; private set; }
        public bool WeaponsFiring { get; private set; }
        public bool HullTakingHit { get; private set; }

        public bool DeadInConflictWith(IEntity conflictedEntity)
        {
            if (conflictedEntity is IWeapon weapon)
            {
                if (ShieldStrength > 0)
                {
                    Energy -= weapon.EnergyDamage;
                    var penetratingDamage = weapon.PhysicalDamage - (int)(weapon.PhysicalDamage * ShieldStrength);
                    if (penetratingDamage > Armor)
                        Health -= penetratingDamage - Armor;
                    ShieldTakingHit = true;
                }
                else
                {
                    Health -= weapon.PhysicalDamage - Armor;
                    HullTakingHit = true;
                }
            }

            return Health <= 0;
        }

        public EntityAction Act(GameState state, Location location)
        {
            var action = new EntityAction();
            ThrustersRunning = WeaponsFiring = ShieldTakingHit = HullTakingHit = false;

            var systemCommand = state.CommandsFromClient.Systems;
            if (systemCommand == ClientCommand.ActivateShield && Energy >= shieldCost)
            {
                ShieldStrength = maxShieldStrength;
                Energy -= shieldCost;
            }
            else
            {
                ShieldStrength = 0;
                if (systemCommand == ClientCommand.OpenFire && Energy >= fireCost)
                {
                    var spawnLocation = new Location(location.Y - 1, location.X);
                    if (state.IsInsideGameField(spawnLocation))
                    {
                        action.Spawn.Add(new FriendlyLaserShot(WeaponPhysicalDamage, WeaponEnergyDamage), spawnLocation);
                        Energy -= fireCost;
                        WeaponsFiring = true;
                    }
                }
            }

            if (Energy >= thrusterCost)
            {
                var horizontalMoveCommand = state.CommandsFromClient.HorizontalMove;
                if (horizontalMoveCommand != ClientCommand.Idle)
                {
                    var deltaX = horizontalMoveCommand == ClientCommand.MoveRight ? 1 : -1;
                    if (state.IsInsideGameField(location.Y, location.X + deltaX))
                    {
                        action.DeltaX = deltaX;
                        Energy -= thrusterCost;
                        ThrustersRunning = true;
                    }
                }
                else
                {
                    var verticalMoveCommand = state.CommandsFromClient.VerticalMove;
                    if (verticalMoveCommand != ClientCommand.Idle)
                    {
                        var deltaY = verticalMoveCommand == ClientCommand.MoveDown ? 1 : -1;
                        if (state.IsInsideGameField(location.Y + deltaY, location.X))
                        {
                            action.DeltaY = deltaY;
                            Energy -= thrusterCost;
                            ThrustersRunning = true;
                        }
                    }
                }
            }

            Energy++;
            return action;
        }

        public Player()
        {
            maxHeath = 100;
            Health = maxHeath;
            maxEnergy = 100;
            Energy = maxEnergy;
            Armor = 1;
            maxShieldStrength = 0.8;
            ShieldStrength = 0;

            WeaponPhysicalDamage = 10;
            WeaponEnergyDamage = 25;

            thrusterCost = 1;
            shieldCost = 2;
            fireCost = 5;

            ThrustersRunning = WeaponsFiring = ShieldTakingHit = HullTakingHit = false;
        }
    }
}