using System.Drawing;

namespace SpaceBattle.Data
{
    public class Animation
    {
        public IGameObject Creature;
        public Command Command;
        public Point Location;
        public Point TargetLogicalLocation;
    }
}