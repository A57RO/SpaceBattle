using System;

namespace GameData.ClientInteraction
{
    [Serializable]
    public enum ClientCommand : byte
    {
        Idle = 0,
        OpenFire = 1,
        ActivateShield = 2,
        MoveRight = 3,
        MoveLeft = 4,
        MoveUp = 5,
        MoveDown = 6
    }
}