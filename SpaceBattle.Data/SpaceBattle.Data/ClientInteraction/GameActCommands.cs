using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpaceBattle.Data.ClientInteraction
{
    /// <summary>
    /// Клиент передаёт этот класс на сервер в начале акта
    /// </summary>
    public class GameActCommands
    {
        public readonly ClientCommand Systems;
        public readonly ClientCommand HorizontalMove;
        public readonly ClientCommand VerticalMove;

        public static GameActCommands IdleCommands => new GameActCommands(null, null);

        public GameActCommands(ControlSettings controlSettings, HashSet<Keys> pressedKeys)
        {
            Systems = HorizontalMove = VerticalMove = ClientCommand.Idle;
            if (controlSettings == null || pressedKeys == null) return;
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