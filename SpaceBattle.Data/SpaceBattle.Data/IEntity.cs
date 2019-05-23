﻿using System.Drawing;

namespace SpaceBattle.Data
{
    public interface IEntity
    {
        Point Position { get; }
        string ImageFileName { get; }
        /// <summary>
        /// Higher value - sprite closer to the screen
        /// </summary>
        int DrawingPriority { get; }
        bool DeadInConflictWith(IEntity conflictedEntity);
        EntityCommand Act(int x, int y);
    }
}