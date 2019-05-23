namespace SpaceBattle.Data
{
    public class EntityCommand
    {
        public int DeltaX;
        public int DeltaY;
        public IEntity TransformTo;

        public EntityCommand(int deltaX, int deltaY, IEntity transformTo)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
            TransformTo = transformTo;
        }
    }
}