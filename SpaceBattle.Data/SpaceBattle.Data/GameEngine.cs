using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceBattle.Data.ClientInteraction;
using SpaceBattle.Data.Entities;

namespace SpaceBattle.Data
{
    public static class GameEngine
    {
        public static void BeginAct(GameState state)
        {
            state.Animations.Clear();
            var entitiesToSpawn = new List<IEntity>();
            foreach (var entity in state.ExistingEntities)
            {
                //Act for main entity
                var action = entity.Act(state);
                state.Animations.Add(new EntityAnimation(entity, action));
                //Act for whole chain of SpawnEntities
                var entityToSpawn = action.SpawnEntity;
                while (entityToSpawn != null)
                {
                    action = entityToSpawn.Act(state);
                    state.Animations.Add(new EntityAnimation(entityToSpawn, action));
                    entitiesToSpawn.Add(entityToSpawn);
                    entityToSpawn = action.SpawnEntity;
                }
            }
            state.ExistingEntities.AddRange(entitiesToSpawn);
        }

        public static void EndAct(GameState downSide, GameState upperSide)
        {
            var deadEntities = new List<IEntity>();
            foreach (var downSideAnimation in downSide.Animations)
            {
                var entity = downSideAnimation.Entity;
                var targetLocation = downSideAnimation.TargetLocation;

                if (!downSide.IsInsideGameField(targetLocation))
                {
                    if (targetLocation.Y < 0)
                    {
                        //TODO: Перенос на другую сторону
                    }
                    //downSide.ExistingEntities.Remove(entity);
                    deadEntities.Add(entity);
                }

                var targetEntity = downSide.ExistingEntities.Find(e => e.Position.Equals(targetLocation));
                if (targetEntity != null && entity.DeadInConflictWith(targetEntity))
                    deadEntities.Add(entity);
                    
                //TODO: пиздос

                var conflictedEntities = downSide.Animations.FindAll(a => a.TargetLocation.Equals(targetLocation));


            }

        }
    }

    public class GameState
    {
        //public IDifficulty Difficulty { get; private set; }
        //public static IGameObject[,] Map { get; private set; }
        public readonly int MapWidth;// => Map.GetLength(0);
        public readonly int MapHeight;// => Map.GetLength(1);

        public List<IEntity> ExistingEntities { get; internal set; }
        public Player PlayerEntity { get; internal set; }

        public List<EntityAnimation> Animations { get; internal set; }
        public GameActCommands Commands { get; internal set; }

        public bool IsInsideGameField(int x, int y) => x >= 0 && x < MapWidth && y >= 0 && y < MapHeight;
        public bool IsInsideGameField(Point position) => IsInsideGameField(position.X, position.Y);
        
        public bool IsStarted { get; internal set; }
        public bool IsOver { get; internal set; }
        public bool IsWin { get; internal set; }
        
        public GameState(int mapWidth, int mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            PlayerEntity = new Player(new Point(MapWidth / 2, MapHeight - 1));
            ExistingEntities = new List<IEntity> { PlayerEntity };
            Animations = new List<EntityAnimation>();
        }
    }
}