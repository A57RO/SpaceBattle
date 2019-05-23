using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceBattle.Data.ClientInteraction;

namespace SpaceBattle.Data.Entities
{
    public class Player : IMortal
    {
        public int DrawingPriority => 1;

        public Point Position { get; private set; }

        public int Health { get; private set; }
        public double Armor { get; private set; }
        public double Shield { get; private set; }

        public bool DeadInConflictWith(IEntity conflictedEntity)
        {
            if (conflictedEntity is IWeapon weapon)
            {
                
            }

            return Health <= 0;
        }

        public EntityCommand Act(GameState state)
        {
            var command = new EntityCommand();
            //TODO
            switch (state.Commands.Systems)
            {
                case ClientCommand.ActivateShield:
                    Shield = 0.9;
                    break;
            }

            return command;
        }

        public Player(Point position)
        {
            Position = position;

            Health = 100;
            Armor = 0.1;
            Shield = 0;
        }
    }
}