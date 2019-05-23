using System.Collections.Generic;
using System.Windows.Forms;

namespace SpaceBattle.Data.ClientInteraction
{
    public class ControlSettings
    {
        public readonly Dictionary<Keys, ClientCommand> ControlMap;

        public ControlSettings(Keys fire, Keys shield, Keys right, Keys left, Keys up, Keys down)
        {
            ControlMap = new Dictionary<Keys, ClientCommand>
            {
                {fire, ClientCommand.OpenFire},
                {shield, ClientCommand.ActivateShield},
                {right, ClientCommand.MoveRight},
                {left, ClientCommand.MoveLeft},
                {up, ClientCommand.MoveUp},
                {down, ClientCommand.MoveDown}
            };
        }
    }
}