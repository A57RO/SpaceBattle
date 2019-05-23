using System.Drawing;

namespace SpaceBattle.Data
{
    public class Animation
    {
        public IEntity Entity;
        public EntityCommand EntityCommand;
        public Point Location;
        public Point TargetLogicalLocation;
    }
}