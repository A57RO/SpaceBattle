using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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


        public static void EndActGood(GameState sideOne, GameState sideTwo)
        {
            //Стадия передвижения и получение анимаций переносимых на другую сторону сущностей
            var animationsToSideOne = MoveStage(sideTwo);
            var animationsToSideTwo = MoveStage(sideOne);
            //Конфликт переносимых сущностей и добавление выживших в список существующих сущностей 
            ConflictTransferringEntities(sideOne, animationsToSideOne, animationsToSideTwo);
            ConflictTransferringEntities(sideTwo, animationsToSideTwo, animationsToSideOne));
            //Стадия конфликта и удаление мёртвых сущностей
            ConflictStage(sideOne);
            ConflictStage(sideTwo);
        }
        
        public static List<EntityAnimation> MoveStage(GameState side)
        {
            var transferringEntitiesAnimations = new List<EntityAnimation>();
            var animations = side.Animations.ToList();
            foreach (var animation in animations)
            {
                var entity = animation.Entity;
                var targetLocation = animation.TargetLocation;

                if (!side.IsInsideGameField(targetLocation))
                {
                    if (targetLocation.Y < 0)
                        transferringEntitiesAnimations.Add(animation);
                    side.ExistingEntities.Remove(entity);
                    side.Animations.Remove(animation);
                    continue;
                }

                entity.Position = targetLocation;
            }

            return transferringEntitiesAnimations;
        }

        private static IEntity ConvertTransferredEntity(IEntity entity, Size mapSize)
        {
            if (entity is FriendlyLaserShot)
                return new EnemyLaserShot(ConvertCoordinatesToAnotherSide(entity.Position, mapSize));
            throw new ArgumentException($"Unknown transferring entity {entity.GetType().Name}");
        }

        public static Point ConvertCoordinatesToAnotherSide(Point position, Size mapSize) =>
            new Point(mapSize.Width - position.X - 1, position.Y);

        private static void ConflictTransferringEntities(
            GameState destinationSide,
            List<EntityAnimation> incomingAnimations, 
            List<EntityAnimation> outcomingAnimations)
        {
            foreach (var animation in incomingAnimations)
            {
                var entity = ConvertTransferredEntity(animation.Entity, destinationSide.MapSize);
                var conflictedEntities = outcomingAnimations
                    .FindAll(a => a.PositionAtBeginAct.Equals(entity.Position))
                    .Select(a => a.Entity);
                var die = false;
                foreach (var conflictedEntity in conflictedEntities)
                    if (entity.DeadInConflictWith(conflictedEntity))
                        die = true;

                if (!die)
                {
                    destinationSide.ExistingEntities.Add(entity);
                    destinationSide.Animations.Add(new EntityAnimation(entity, new EntityAction()));
                }
            }
        }

        public static void ConflictStage(GameState side)
        {
            var allEntities = side.ExistingEntities.ToList();
            foreach (var animation in side.Animations)
            {
                var entity = animation.Entity;
                var action = animation.Action;
                //Стоявшие в соседних клетках сущности которые пересеклись в движении
                var conflictedEntities = side.Animations
                    .FindAll(a => 
                        entity.Position.Equals(a.PositionAtBeginAct) &&
                        entity != a.Entity &&
                        action.DeltaX + a.Action.DeltaX == 0 &&
                        action.DeltaY + a.Action.DeltaY == 0)
                    .Select(a => a.Entity)
                    .ToList();
                //Сущности попавшие в одинаковую позицию
                conflictedEntities.AddRange(
                    allEntities.FindAll(e => 
                        e.Position.Equals(entity.Position) &&
                        entity != e));

                var die = false;
                foreach (var conflictedEntity in conflictedEntities)
                    if (entity.DeadInConflictWith(conflictedEntity))
                        die = true;

                if (die)
                    side.ExistingEntities.Remove(entity);
            }
        }
    }

    public class GameState
    {
        //public IDifficulty Difficulty { get; private set; }
        public readonly Size MapSize;
        public bool IsInsideGameField(int x, int y) => x >= 0 && x < MapSize.Width && y >= 0 && y < MapSize.Height;
        public bool IsInsideGameField(Point position) => IsInsideGameField(position.X, position.Y);

        public List<IEntity> ExistingEntities { get; internal set; }
        public Player PlayerEntity { get; internal set; }

        public List<EntityAnimation> Animations { get; internal set; }
        public GameActCommands Commands { get; internal set; }


        public bool IsStarted { get; internal set; }
        public bool IsOver { get; internal set; }
        public bool IsWin { get; internal set; }

        public GameState(Size mapSize)
        {
            MapSize = mapSize;
            PlayerEntity = new Player(new Point(MapSize.Width / 2, MapSize.Height - 1));
            ExistingEntities = new List<IEntity> {PlayerEntity};
            Animations = new List<EntityAnimation>();
        }
        public GameState(int mapHeight, int mapWidth) : this(new Size(mapWidth, mapHeight))
        {
        }
    }
}