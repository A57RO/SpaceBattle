using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameData;
using GameData.Entities;

namespace Client
{
    public static class Visual
    {
        public const int ElementSize = 32;
        public const int IconSize = ElementSize / 2;
        public const string FontName = "Eras Bold ITC";

        private static readonly Dictionary<Type, Bitmap> RedSprites = new Dictionary<Type, Bitmap>
        {
            {typeof(Player), Properties.Resources.PlayerRed},
            {typeof(FriendlyLaserShot), Properties.Resources.ShotRed},
            {typeof(EnemyLaserShot), Properties.Resources.ShotBlue}
        };
        private static readonly Dictionary<Type, Bitmap> RedSpritesFlipped =
            RedSprites.ToDictionary(s => s.Key, s => GetFlippedImage(s.Value));

        private static readonly Dictionary<Type, Bitmap> blueSprites = new Dictionary<Type, Bitmap>
        {
            {typeof(Player), Properties.Resources.PlayerBlue},
            {typeof(FriendlyLaserShot), Properties.Resources.ShotBlue},
            {typeof(EnemyLaserShot), Properties.Resources.ShotRed}
        };
        private static readonly Dictionary<Type, Bitmap> BlueSpritesFlipped =
            blueSprites.ToDictionary(s => s.Key, s => GetFlippedImage(s.Value));

        private static readonly Dictionary<Type, Bitmap> CommonSprites = new Dictionary<Type, Bitmap>
        {
        };

        public static Bitmap GetFlippedImage(Bitmap sprite)
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
            GameState state,
            bool isBottom,
            bool isRed,
            int tick,
            out Point playerDrawingPosition)
        {
            drawingElements.Clear();
            playerDrawingPosition = Point.Empty;
            foreach (var animation in state.Animations)
            {
                var sprite = GetSpriteForEntity(animation.Entity, isBottom, isRed);
                var drawingPosition = isBottom ? 
                    GetShiftedCoordinates(
                    new Point(animation.BeginActLocation.X, animation.BeginActLocation.Y + state.MapHeight),
                    animation.Action.DeltaX, animation.Action.DeltaY, tick) :
                GetShiftedCoordinatesForTopSide(
                    animation.BeginActLocation,
                    state.MapWidth,
                    state.MapHeight,
                    animation.Action.DeltaX,
                    animation.Action.DeltaY,
                    tick);
                if (animation.Entity is Player)
                    playerDrawingPosition = drawingPosition;
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
                (coordinates.Y  + 1) * ElementSize + yShift);

        }

        private static Point ConvertCoordinatesToTopSide(Location bottomSideCoordinates, int mapWidth, int mapHeight) => 
            new Point(mapWidth - bottomSideCoordinates.X - 1, mapHeight - bottomSideCoordinates.Y - 1);
    }
}