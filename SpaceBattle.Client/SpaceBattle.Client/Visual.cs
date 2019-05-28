using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SpaceBattle.Data;

namespace SpaceBattle.Client
{
    public static class Visual
    {
        public const int ElementSize = 32;

        private static readonly Dictionary<Type, Bitmap> RedSprites = new Dictionary<Type, Bitmap>
        {
            {typeof(Data.Entities.Player), Properties.Resources.PlayerRed},
            {typeof(Data.Entities.FriendlyLaserShot), Properties.Resources.ShotRed},
            {typeof(Data.Entities.EnemyLaserShot), Properties.Resources.ShotBlue}
        };
        private static readonly Dictionary<Type, Bitmap> RedSpritesFlipped =
            RedSprites.ToDictionary(s => s.Key, s => GetFlippedImage(s.Value));

        private static readonly Dictionary<Type, Bitmap> blueSprites = new Dictionary<Type, Bitmap>
        {
            {typeof(Data.Entities.Player), Properties.Resources.PlayerBlue},
            {typeof(Data.Entities.FriendlyLaserShot), Properties.Resources.ShotBlue},
            {typeof(Data.Entities.EnemyLaserShot), Properties.Resources.ShotRed}
        };
        private static readonly Dictionary<Type, Bitmap> BlueSpritesFlipped =
            blueSprites.ToDictionary(s => s.Key, s => GetFlippedImage(s.Value));

        private static readonly Dictionary<Type, Bitmap> CommonSprites = new Dictionary<Type, Bitmap>
        {
        };

        private static Bitmap GetFlippedImage(Bitmap sprite)
        {
            var clone = sprite.Clone() as Bitmap;
            clone.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return clone;
        }

        public static Bitmap GetSpriteForEntity(IEntity entity, bool isBottom, bool isRed)
        {
            var entityType = entity.GetType();
            if (isBottom)
            {
                if (isRed && RedSprites.TryGetValue(entityType, out var redSprite))
                    return redSprite;
                if (blueSprites.TryGetValue(entityType, out var blueSprite))
                    return blueSprite;
            }
            else
            {
                if (isRed && RedSpritesFlipped.TryGetValue(entityType, out var redSpriteFlipped))
                    return redSpriteFlipped; 
                if (BlueSpritesFlipped.TryGetValue(entityType, out var blueSpriteFlipped))
                    return blueSpriteFlipped;
            }
            if (CommonSprites.TryGetValue(entityType, out var commonSprite))
                return commonSprite;

            throw new ArgumentException($"Unknown entity {entityType}");
        }

        public static void UpdateDrawingElements(
            Dictionary<Bitmap, HashSet<Point>> drawingElements,
            List<EntityAnimation> animations,
            bool isBottom,
            bool isRed,
            int mapWidth,
            int mapHeight,
            int tick)
        {
            drawingElements.Clear();
            foreach (var animation in animations)
            {
                var sprite = GetSpriteForEntity(animation.Entity, isBottom, isRed);
                var drawingPosition = isBottom ? 
                    GetShiftedCoordinates(
                    new Point(animation.BeginActLocation.X, animation.BeginActLocation.Y + mapHeight),
                    animation.Action.DeltaX, animation.Action.DeltaY, tick) :
                GetShiftedCoordinatesForTopSide(
                    animation.BeginActLocation,
                    mapWidth,
                    mapHeight,
                    animation.Action.DeltaX,
                    animation.Action.DeltaY,
                    tick);
                if (drawingElements.TryGetValue(sprite, out var points))
                    points.Add(drawingPosition);
                else
                    drawingElements.Add(sprite, new HashSet<Point> { drawingPosition });
            }
        }

        private static Point GetShiftedCoordinatesForTopSide(
            Location localCoordinates,
            int topSideMapWidth,
            int topSideMapHeight,
            int localDeltaX,
            int localDeltaY,
            int tick) 
            =>
            GetShiftedCoordinates(
                ConvertCoordinatesToTopSide(localCoordinates, topSideMapWidth, topSideMapHeight),
                -localDeltaX,
                -localDeltaY,
                tick);

        private static Point GetShiftedCoordinates(Point coordinates, int deltaX, int deltaY, int tick)
        {
            var xShift = tick * 4 * deltaX;
            var yShift = tick * 4 * deltaY;
            return new Point(
                coordinates.X * ElementSize + xShift,
                coordinates.Y * ElementSize + yShift);

        }

        private static Point ConvertCoordinatesToTopSide(Location bottomSideCoordinates, int mapWidth, int mapHeight) => 
            new Point(mapWidth - bottomSideCoordinates.X - 1, mapHeight - bottomSideCoordinates.Y - 1);
    }
}