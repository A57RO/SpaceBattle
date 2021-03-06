﻿using System;
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
        public static readonly Font HUDNumbersFont = new Font(FontName, 10);
        public static readonly Font ButtonsFont = new Font(FontName, 16);

        private static readonly Dictionary<Type, int> DrawingPriorities = new Dictionary<Type, int>
        {
            {typeof(Player), 0},
            {typeof(FriendlyLaserShot), 2},
            {typeof(EnemyLaserShot), 1}
        };

        public static IOrderedEnumerable<EntityAnimation> OrderAnimations(List<EntityAnimation> animations) =>
            animations.OrderBy(a => DrawingPriorities[a.Entity.GetType()]);

        private static Bitmap GetFlippedImage(Bitmap sprite)
        {
            var clone = sprite.Clone() as Bitmap;
            clone.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return clone;
        }

        private static IEnumerable<Bitmap> GetSpritesForEntity(IEntity entity, bool isBottom, bool isRed)
        {
            var sprites = new List<Bitmap>();
            switch (entity)
            {
                case Player p:
                    sprites.Add(isRed ? Properties.Resources.PlayerRed : Properties.Resources.PlayerBlue);
                    if (p.ShieldStrength > 0)
                        sprites.Add(Properties.Resources.Shield);
                    break;
                case FriendlyLaserShot s:
                    sprites.Add(isRed ? Properties.Resources.ShotRed : Properties.Resources.ShotBlue);
                    break;
                case EnemyLaserShot s:
                    sprites.Add(isRed ? Properties.Resources.ShotBlue : Properties.Resources.ShotRed);
                    break;
                default:
                    throw new ArgumentException($"Unknown entity {entity.GetType().Name}");
            }

            if (!isBottom)
                for (int i = 0; i < sprites.Count; i++)
                    sprites[i] = GetFlippedImage(sprites[i]);
            return sprites;
        }

        public static void UpdateGameField(
            DrawingElements elements,
            IOrderedEnumerable<EntityAnimation> orderedAnimations,
            bool isBottom,
            bool isRed,
            int mapWidth,
            int mapHeight,
            int tick)
        {
            foreach (var animation in orderedAnimations)
            {
                var sprites = GetSpritesForEntity(animation.Entity, isBottom, isRed);
                var drawingLocation = isBottom
                    ? GetShiftedCoordinates(
                        new Point(animation.BeginActLocation.X, animation.BeginActLocation.Y + mapHeight),
                        animation.Action.DeltaX, animation.Action.DeltaY, tick)
                    : GetShiftedCoordinatesForTopSide(
                        animation.BeginActLocation,
                        mapWidth,
                        mapHeight,
                        animation.Action.DeltaX,
                        animation.Action.DeltaY,
                        tick);
                foreach (var sprite in sprites)
                {
                    if (elements.Images.TryGetValue(sprite, out var points))
                        points.Add(drawingLocation);
                    else
                        elements.Images.Add(sprite, new HashSet<Point> { drawingLocation });
                }
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
                (coordinates.Y + 1) * ElementSize + yShift);

        }

        private static Point ConvertCoordinatesToTopSide(Location bottomSideCoordinates, int mapWidth, int mapHeight) =>
            new Point(mapWidth - bottomSideCoordinates.X - 1, mapHeight - bottomSideCoordinates.Y - 1);

        public static void UpdatePlayerHUD(DrawingElements hud, Player player, Size clientSize, bool isBottom)
        {
            var middleX = clientSize.Width / 2;
            int numericY;
            int scaledY;
            if (isBottom)
            {
                numericY = clientSize.Height - ElementSize;
                scaledY = numericY + IconSize;
            }
            else
            {
                numericY = IconSize;
                scaledY = 0;
            }

            UpdateHUDIcons(hud, clientSize, middleX, numericY, scaledY);

            UpdateHUDNumber(hud, player.Armor.ToString(), IconSize, numericY, true);
            UpdateHUDNumber(hud, $"{(int)(player.ShieldStrength * 100)}%", clientSize.Width - ElementSize, numericY, false);

            var rectangleSize = new Size(middleX / 20 - 1, IconSize);

            var leftX = middleX - IconSize - rectangleSize.Width;
            UpdateHUDNumber(hud, player.WeaponPhysicalDamage.ToString(), leftX, numericY, false);
            UpdateHUDBar(hud, player.Health, leftX, scaledY, rectangleSize, Brushes.DarkRed, false);
            UpdateHUDNumber(hud, player.Health.ToString(), leftX, scaledY, false);

            var rightX = middleX + IconSize;
            UpdateHUDNumber(hud, player.WeaponEnergyDamage.ToString(), rightX, numericY, true);
            UpdateHUDBar(hud, player.Energy, rightX, scaledY, rectangleSize, Brushes.Indigo, true);
            UpdateHUDNumber(hud, player.Energy.ToString(), rightX, scaledY, true);
        }

        private static void UpdateHUDIcons(DrawingElements hud, Size clientSize, int middleX, int numericY, int scaledY)
        {
            hud.Images.Add(Properties.Resources.HUDArmor, new HashSet<Point> { new Point(0, numericY) });
            hud.Images.Add(Properties.Resources.HUDShield, new HashSet<Point> { new Point(clientSize.Width - IconSize, numericY) });
            hud.Images.Add(Properties.Resources.HUDHealthDamage, new HashSet<Point> { new Point(middleX - IconSize, numericY) });
            hud.Images.Add(Properties.Resources.HUDEnergyDamage, new HashSet<Point> { new Point(middleX, numericY) });
            hud.Images.Add(Properties.Resources.HUDHealth, new HashSet<Point> { new Point(middleX - IconSize, scaledY) });
            hud.Images.Add(Properties.Resources.HUDEnergy, new HashSet<Point> { new Point(middleX, scaledY) });
        }

        private static void UpdateHUDBar(DrawingElements hud, int value, int beginX, int beginY, Size rectangleSize, Brush brush, bool toRight)
        {
            var deltaX = rectangleSize.Width + 1;
            if (!toRight)
                deltaX = -deltaX;
            for (int i = 0; i < value / 5; i++)
            {
                var rectangle = new Rectangle(beginX + i * deltaX, beginY, rectangleSize.Width, rectangleSize.Height);
                if (hud.Rectangles.TryGetValue(rectangle, out var localBrush))
                {
                    if (localBrush.Equals(brush)) continue;
                    hud.Rectangles[rectangle] = brush;
                }
                else
                    hud.Rectangles.Add(rectangle, brush);
            }
        }

        private static void UpdateHUDNumber(DrawingElements hud, string stringValue, int beginX, int beginY, bool toRight)
        {
            var numberX = beginX;
            if (!toRight)
                numberX -= stringValue.Length * 9 - 2;
            var location = new Point(numberX, beginY);
            if (hud.Strings.TryGetValue(stringValue, out var value))
                value.Add(location);
            else
                hud.Strings.Add(stringValue, new HashSet<Point> { location });
        }
    }
}