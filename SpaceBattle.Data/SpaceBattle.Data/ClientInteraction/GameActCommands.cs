using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpaceBattle.Data.ClientInteraction
{
    /// <summary>
    /// Client transmit instance of this class to server at beginning of game act
    /// </summary>
    public class GameActCommands
    {
        public readonly ClientCommand Systems;
        public readonly ClientCommand HorizontalMove;
        public readonly ClientCommand VerticalMove;

        public GameActCommands(ControlSettings controlSettings, HashSet<Keys> pressedKeys)
        {
            foreach (var key in pressedKeys)
            {
                if (controlSettings.ControlMap.TryGetValue(key, out var command))
                {
                    switch (command)
                    {
                        case ClientCommand.OpenFire:
                        case ClientCommand.ActivateShield:
                            Systems = command;
                            break;
                        case ClientCommand.MoveRight:
                        case ClientCommand.MoveLeft:
                            HorizontalMove = command;
                            break;
                        case ClientCommand.MoveUp:
                        case ClientCommand.MoveDown:
                            VerticalMove = command;
                            break;
                        default:
                            throw new ArgumentException($"Unknown command {command}");
                    }
                }
            }
        }
    }
}