using System;
using System.Collections.Generic;
using System.Linq;
using SpaceBattle.Data.Entities;

namespace SpaceBattle.Data
{
    public static class GameEngine
    {
        public static void BeginAct(GameState state)
        {
            state.Animations.Clear();
            for (int y = 0; y < state.MapHeight; y++)
            for (int x = 0; x < state.MapWidth; x++)
            {
                var entity = state.Map[y, x];
                if (entity == null) continue;
                var location = new Location(y, x);
                //Act for main entity
                var action = entity.Act(state, location);
                state.Animations.Add(new EntityAnimation(entity, action, location));
                //Act for whole chain of entities from Spawn
                Spawn(action.Spawn, state);
            }
        }

        private static void Spawn(Dictionary<IEntity, Location> spawn, GameState state)
        {
            foreach (var entityWithPosition in spawn)
            {
                var entity = entityWithPosition.Key;
                var position = entityWithPosition.Value;
                var action = entity.Act(state, position);
                var animation = new EntityAnimation(entity, action, position);
                state.Animations.Add(animation);
                Spawn(action.Spawn, state);
            }
        }

        public static void EndAct(GameState sideOne, GameState sideTwo)
        {
            //Получение списка анимаций сущностей прыгающих на другую сторону и удаление их с текущей стороны
            var animationsWarpingToSideOne = WarpAnimations(sideTwo);
            var animationsWarpingToSideTwo = WarpAnimations(sideOne);
            //Конфликт анимаций прыгающих сущностей и добавление выживших в список анимаций стороны
            WarpConflict(sideOne, animationsWarpingToSideOne, animationsWarpingToSideTwo);
            WarpConflict(sideTwo, animationsWarpingToSideTwo, animationsWarpingToSideOne);
            //Конфликт анимаций в пределах одной стороны включая только что прыгнувших на эту сторону
            SpaceConflict(sideOne);
            SpaceConflict(sideTwo);
        }

        private static List<EntityAnimation> WarpAnimations(GameState side)
        {
            var warpingEntitiesAnimations = new List<EntityAnimation>();
            var animations = side.Animations.ToList();
            foreach (var animation in animations)
            {
                var targetLocation = animation.TargetLocation;

                if (side.IsInsideGameField(targetLocation)) continue;
                if (targetLocation.Y < 0)
                    warpingEntitiesAnimations.Add(animation);
                side.Animations.Remove(animation);
            }

            return warpingEntitiesAnimations;
        }

        private static void WarpConflict(
            GameState destinationSide,
            List<EntityAnimation> incomingAnimations,
            List<EntityAnimation> outgoingAnimations)
        {
            var incomingAnimationsInDestination =
                incomingAnimations.Select(a => ConvertWarpedEntityAnimation(destinationSide, a)).ToList();
            foreach (var animation in incomingAnimationsInDestination)
            {
                if (SurvivedInConflictWithParticipants(animation, incomingAnimationsInDestination) &&
                    SurvivedInConflictWithParticipants(animation, outgoingAnimations))
                    destinationSide.Animations.Add(animation);
            }
        }

        private static void SpaceConflict(GameState state)
        {
            var survivedEntitiesAnimations = new List<EntityAnimation>();
            foreach (var animation in state.Animations)
            {
                if (SurvivedInConflictWithParticipants(animation, state.Animations))
                    survivedEntitiesAnimations.Add(animation);
            }
            state.UpdateStateAfterAct(survivedEntitiesAnimations);
        }

        private static bool SurvivedInConflictWithParticipants(EntityAnimation animation, List<EntityAnimation> conflictParticipants)
        {
            var animationIsAlive = true;
            var conflictedEntitiesAnimations = conflictParticipants.FindAll(a => InConflict(animation, a));
            foreach (var conflictedEntityAnimation in conflictedEntitiesAnimations)
            {
                if (animation.Entity.DeadInConflictWith(conflictedEntityAnimation.Entity))
                    animationIsAlive = false;
            }

            return animationIsAlive;
        }

        private static bool InConflict(EntityAnimation first, EntityAnimation second)
        {
            return Confronted() || Crossed();

            //Попали в одинаковую позицию
            bool Confronted() =>
                first.TargetLocation.Equals(second.TargetLocation) &&
                first.Entity != second.Entity;

            //Поменялись местами (пересеклись в движении)
            bool Crossed() =>
                first.TargetLocation.Equals(second.BeginActLocation) &&
                second.TargetLocation.Equals(first.BeginActLocation) &&
                first.Action.DeltaX + second.Action.DeltaX == 0 &&
                first.Action.DeltaY + second.Action.DeltaY == 0;
        }

        private static EntityAnimation ConvertWarpedEntityAnimation(
            GameState destinationSide,
            EntityAnimation animation)
        {
            var oldEntity = animation.Entity;
            var newLocation = ConvertCoordinatesToAnotherSide(animation.TargetLocation, destinationSide.MapWidth);
            IEntity newEntity = null;
            switch (oldEntity)
            {
                case FriendlyLaserShot friendlyLaserShot:
                    newEntity = new EnemyLaserShot(friendlyLaserShot);
                    break;
                default:
                    throw new ArgumentException($"Unknown transferring entity {oldEntity.GetType().Name} at location {animation.BeginActLocation}");
            }

            var newAction = newEntity.Act(destinationSide, newLocation);
            return new EntityAnimation(newEntity, newAction, newLocation);
        }

        private static Location ConvertCoordinatesToAnotherSide(Location location, int mapWidth) =>
            new Location(location.Y, mapWidth - location.X - 1);
    }
}