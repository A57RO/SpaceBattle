using System;
using System.Collections.Generic;
using GameData.ClientInteraction;
using GameData.Entities;

namespace GameData
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
        public EntityAnimation PlayerAnimation { get; private set; }

        public List<EntityAnimation> Animations { get; private set; }
        public GameActCommands CommandsFromClient { get; private set; }

        /// <summary>
        /// False = игра ещё в процессе или закончилась победой. True = игра закончилась поражением.
        /// Победу проверять по данному полю состояния противника.
        /// </summary>
        public bool GameOver { get; private set; }
        
        public GameState(int mapHeight = 10, int mapWidth = 11)
        {
            if (mapHeight < 2 || mapWidth < 3)
                throw new ArgumentException("Map is too small");
            MapHeight = mapHeight;
            MapWidth = mapWidth;
            Map = new IEntity[mapHeight, mapWidth];
            var playerPosition = new Location(mapHeight - 1, mapWidth / 2);
            PlayerEntity = new Player();
            PlayerAnimation = new EntityAnimation(PlayerEntity, new EntityAction(), playerPosition);
            Map[playerPosition.Y, playerPosition.X] = PlayerEntity;
            Animations = new List<EntityAnimation>();
            CommandsFromClient = GameActCommands.IdleCommands;
            GameOver = false;
        }

        public void GiveCommandsFromClient(GameActCommands commands)
        {
            if (commands != null)
                CommandsFromClient = commands;
        }

        /// <summary>
        /// Общее обновление состояния в середине или конце акта.
        /// </summary>
        /// <param name="newAnimations">Анимации нового состояния</param>
        public void UpdateStateInAct(List<EntityAnimation> newAnimations)
        {
            Animations = newAnimations ?? throw new ArgumentNullException();
            Map = new IEntity[MapHeight, MapWidth];
            var newPlayerAnimation = newAnimations.Find(a => a.Entity is Player);
            if (newPlayerAnimation != null)
            {
                PlayerAnimation = newPlayerAnimation;
                PlayerEntity = (Player) newPlayerAnimation.Entity;
            }
            else
            {
                PlayerEntity = null;
                GameOver = true;
            }
        }
        /// <summary>
        /// Внутреннее обновление состояния из движка по окончанию акта.
        /// </summary>
        /// <param name="newAnimations">Анимации нового состояния</param>
        internal void UpdateStateAfterAct(List<EntityAnimation> newAnimations)
        {
            UpdateStateInAct(newAnimations);
            CommandsFromClient = GameActCommands.IdleCommands;
            foreach (var animation in newAnimations)
                Map[animation.TargetLocation.Y, animation.TargetLocation.X] = animation.Entity;
        }
    }
}