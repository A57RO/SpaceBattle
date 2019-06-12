using System;
using System.Collections.Generic;
using System.Linq;
using GameData.ClientInteraction;
using GameData.Entities;

namespace GameData
{
    public static class GameEngine
    {
        /// <summary>
        /// Начало акта для текущего состояния.
        /// </summary>
        /// <param name="state">Текущее состояние</param>
        public static void BeginAct(GameState state)
        {
            state.Animations.Clear();
            for (int y = 0; y < state.MapHeight; y++)
            for (int x = 0; x < state.MapWidth; x++)
            {
                //Cущность в текущей позиции
                var entity = state.Map[y, x];
                if (entity == null) continue;
                var location = new Location(y, x);
                //Начало акта для данной сущности
                var action = entity.Act(state, location);
                state.Animations.Add(new EntityAnimation(entity, action, location));
                //Обработка сущностей, которых хочет создать данная сущность
                Spawn(action.Spawn, state);
            }
        }

        /// <summary>
        /// Обработка создаваемых сущностей в текущем состоянии.
        /// </summary>
        /// <param name="spawn">Отображение создаваемой сущности в позицию её создания</param>
        /// <param name="state">Текущее состояние</param>
        private static void Spawn(Dictionary<IEntity, Location> spawn, GameState state)
        {
            foreach (var entityWithPosition in spawn)
            {
                var entity = entityWithPosition.Key;
                var position = entityWithPosition.Value;
                if (!state.IsInsideGameField(position)) continue;
                //Начало акта для создаваемой сущности
                var action = entity.Act(state, position);
                state.Animations.Add(new EntityAnimation(entity, action, position));
                //Рекурсивное создание сущностей, которых хочет создать только что созданная сущность
                Spawn(action.Spawn, state);
            }
        }

        /// <summary>
        /// Окончание акта для состояний из одной игровой сессии.
        /// Порядок аргументов значения не имеет.
        /// </summary>
        /// <param name="stateOne">Одно состояние</param>
        /// <param name="stateTwo">Другое состояние</param>
        public static void EndAct(GameState stateOne, GameState stateTwo)
        {
            //Стадия перемещения
            //Получение сущностей, прыгающих в другое состояние, и удаление их из родного состояния
            var animationsWarpingToStateOne = WarpAnimations(stateTwo);
            var animationsWarpingToStateTwo = WarpAnimations(stateOne);
            //Стадия конфликта
            //Конфликт сущностей, прыгающих в одно состояние, с сущностями, прыгающими в другое состояние
            WarpConflict(stateOne, animationsWarpingToStateOne, animationsWarpingToStateTwo);
            WarpConflict(stateTwo, animationsWarpingToStateTwo, animationsWarpingToStateOne);
            //Конфликт всех сущностей в обоих состояниях и обновление состояния
            SpaceConflict(stateOne);
            SpaceConflict(stateTwo);
        }

        /// <summary>
        /// Обработка перемещения сущностей за пределы карты.
        /// </summary>
        /// <param name="state">Текущее состояние</param>
        /// <returns>Cписок анимаций сущностей, прыгающих в другое состояние</returns>
        private static List<EntityAnimation> WarpAnimations(GameState state)
        {
            var warpingEntitiesAnimations = new List<EntityAnimation>();
            var animations = state.Animations.ToList();
            foreach (var animation in animations)
            {
                var targetLocation = animation.TargetLocation;
                if (state.IsInsideGameField(targetLocation)) continue;
                //Перемещаем в другое состояние только сущности, вылетевшие за верхнюю границу карты
                if (targetLocation.Y < 0)
                    warpingEntitiesAnimations.Add(animation);
                //Удаление всех сущностей, перемещающихся за пределы карты, из текущего состояния включая прыгающих в другое состояние
                state.Animations.Remove(animation);
            }

            return warpingEntitiesAnimations;
        }

        /// <summary>
        /// Обработка конфликта анимаций сущностей, прыгающих в состояние назначения.
        /// Добавление выживших из них в список анимаций состояния назначения.
        /// </summary>
        /// <param name="destinationState">Состояние назначения</param>
        /// <param name="incomingAnimations">Анимации сущностей, прыгающих в состояние назначения, в системе координат родного состояния</param>
        /// <param name="outgoingAnimations">Анимации сущностей, прыгающих из состояния назначения</param>
        private static void WarpConflict(
            GameState destinationState,
            List<EntityAnimation> incomingAnimations,
            List<EntityAnimation> outgoingAnimations)
        {
            //Анимации сущностей, прыгающих в состояние назначения, в системе координат состояния назначения
            var incomingAnimationsInDestination =
                incomingAnimations.Select(a => ConvertWarpedEntityAnimation(a, destinationState)).ToList();
            foreach (var animation in incomingAnimationsInDestination)
            {
                if (SurvivedInConflictWithParticipants(animation, incomingAnimationsInDestination) &&
                    SurvivedInConflictWithParticipants(animation, outgoingAnimations))
                    destinationState.Animations.Add(animation);
            }
        }

        /// <summary>
        /// Обработка конфликта анимаций в пределах одного состояния, включая только что прыгнувших в это состояние.
        /// Обновление состояния по завершению конфликта.
        /// </summary>
        /// <param name="state">Текущее состояние</param>
        private static void SpaceConflict(GameState state)
        {
            var survivedEntitiesAnimations = new List<EntityAnimation>();
            foreach (var animation in state.Animations)
            {
                if (SurvivedInConflictWithParticipants(animation, state.Animations))
                    survivedEntitiesAnimations.Add(animation);
            }
            //Обновление текущего состояния
            state.UpdateStateAfterAct(survivedEntitiesAnimations);
        }

        /// <summary>
        /// Обработка конфликта сущности со списком возможных участников конфликта.
        /// </summary>
        /// <param name="animation">Анимация рассматриваемой сущности затянутой в конфликт</param>
        /// <param name="conflictParticipants">Список анимаций возможных участников конфликта</param>
        /// <returns>Выжила ли рассматриваемая сущность в конфликте со списком возможных участников</returns>
        private static bool SurvivedInConflictWithParticipants(EntityAnimation animation,
            List<EntityAnimation> conflictParticipants)
        {
            var animationIsAlive = true;
            //Список анимаций удостоверенных участников конфликта не включающий рассматриваемую сущность
            var conflictedEntitiesAnimations = conflictParticipants.FindAll(a => InConflict(animation, a));
            foreach (var conflictedEntityAnimation in conflictedEntitiesAnimations)
            {
                if (animation.Entity.DeadInConflictWith(conflictedEntityAnimation.Entity))
                    animationIsAlive = false;
            }

            return animationIsAlive;
        }

        /// <summary>
        /// Проверка двух сущностей на конфликт между ними.
        /// </summary>
        /// <param name="first">Анимация первой сущности</param>
        /// <param name="second">Анимация второй сущности</param>
        /// <returns>Конфликтуют ли между собой первая и вторая сущность</returns>
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

        /// <summary>
        /// Перевод анимации прыгающей сущности в систему координат состояния назначения.
        /// </summary>
        /// <param name="animation">Анимация прыгающей сущности</param>
        /// <param name="destinationState">Состояние назначения</param>
        /// <returns>Анимация прыгающей сущности в системе координат состояния назначения</returns>
        private static EntityAnimation ConvertWarpedEntityAnimation(
            EntityAnimation animation,
            GameState destinationState)
        {
            var oldEntity = animation.Entity;
            //Перевод координат позиции в начале акта прыгающей сущности в систему координат состояния назначения
            //Полученная позиция будет находится за верхней границей карты состояния назначения
            //Она будет являться позицией в начале акта состояния назначения прыгающей сущости
            var newLocation = ConvertCoordinatesToAnotherState(animation.TargetLocation, destinationState.MapWidth);
            IEntity newEntity;
            //Отображение переносимой сущности из сущности родного состояния в эквивалентную сущность в состоянии назначения
            switch (oldEntity)
            {
                case FriendlyLaserShot friendlyLaserShot:
                    newEntity = new EnemyLaserShot(friendlyLaserShot);
                    break;
                default:
                    throw new ArgumentException(
                        $"Unknown transferring entity {oldEntity.GetType().Name} at location {animation.BeginActLocation}");
            }
            //Получение действия переносимой сущности в состоянии назначения эквивалентного действию в родном состоянии
            var newAction = newEntity.Act(destinationState, newLocation);
            return new EntityAnimation(newEntity, newAction, newLocation);
        }

        /// <summary>
        /// Перевод координат позиции из системы координат родного состояния в систему координат состояния назначения.
        /// </summary>
        /// <param name="location">Координаты позиции в системе координат родного состояния</param>
        /// <param name="destinationStateMapWidth">Ширина карты состояния назначения</param>
        /// <returns>Координаты позиции в системе координат состояния назначения</returns>
        private static Location ConvertCoordinatesToAnotherState(Location location, int destinationStateMapWidth) =>
            new Location(location.Y, destinationStateMapWidth - location.X - 1);
    }
}