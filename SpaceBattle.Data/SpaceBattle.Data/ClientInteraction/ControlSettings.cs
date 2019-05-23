using System.Collections.Generic;
using System.Windows.Forms;

namespace SpaceBattle.Data.ClientInteraction
{
    public class ControlSettings
    {
        public readonly Dictionary<Keys, ClientCommand> ControlMap;

        public ControlSettings(Keys fire, Keys shield, Keys left, Keys right, Keys up, Keys down)
        {
            ControlMap = new Dictionary<Keys, ClientCommand>
            {
                {fire, ClientCommand.OpenFire},
                {shield, ClientCommand.ActivateShield},
                {left, ClientCommand.MoveLeft},
                {right, ClientCommand.MoveRight},
                {up, ClientCommand.MoveUp},
                {down, ClientCommand.MoveDown}
            };
        }
    }
}