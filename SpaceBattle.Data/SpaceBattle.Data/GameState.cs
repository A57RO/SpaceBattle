using System;
using System.Collections.Generic;
using SpaceBattle.Data.ClientInteraction;
using SpaceBattle.Data.Entities;

namespace SpaceBattle.Data
{
    public class GameState
    {
        //public IDifficulty Difficulty { get; private set; }
        public readonly int MapHeight;
        public readonly int MapWidth;
        public bool IsInsideGameField(int y, int x) => y >= 0 && y < MapHeight && x >= 0 && x < MapWidth;
        public bool IsInsideGameField(Location location) => IsInsideGameField(location.Y, location.X);

        public IEntity[,] Map { get; private set; } //[y,x]
        public Player PlayerEntity { get; private set; }

        public List<EntityAnimation> Animations { get; private set; }
        public GameActCommands Commands { get; private set; }
        
        public bool IsOver { get; private set; }
        public bool IsWin { get; private set; }
        
        public GameState(int mapHeight, int mapWidth)
        {
            if (mapHeight < 2 || mapWidth < 3)
                throw new ArgumentException("Map is too small");
            MapHeight = mapHeight;
            MapWidth = mapWidth;
            Map = new IEntity[mapHeight, mapWidth];
            var playerPosition = new Location(mapHeight - 1, mapWidth / 2);
            PlayerEntity = new Player();
            Map[playerPosition.Y, playerPosition.X] = PlayerEntity;
            Animations = new List<EntityAnimation>();
            Commands = GameActCommands.IdleCommands;
            IsOver = IsWin = false;
        }

        public void GiveClientCommands(GameActCommands commands)
        {
            Commands = commands;
        }

        public void UpdateStateInAct(List<EntityAnimation> newAnimations)
        {
            Animations = newAnimations;
            Map = new IEntity[MapHeight, MapWidth];
            var newPlayerAnimation = newAnimations.Find(a => a.Entity is Player);
            if (newPlayerAnimation != null)
                PlayerEntity = (Player)newPlayerAnimation.Entity;
            else
            {
                IsOver = true;
                IsWin = false;
            }
        }

        internal void UpdateStateAfterAct(List<EntityAnimation> newAnimations)
        {
            UpdateStateInAct(newAnimations);
            Commands = GameActCommands.IdleCommands;
            foreach (var animation in newAnimations)
                Map[animation.TargetLocation.Y, animation.TargetLocation.X] = animation.Entity;
        }
    }
}