using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameData;
using GameData.ClientInteraction;
using GameData.Entities;

namespace Client
{
    public class GameWindow : Form
    {
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private readonly ControlSettings controlSettings;
        private readonly bool bottomIsRed;
        private readonly GameState topSideState;
        private readonly GameState bottomSideState;
        private readonly Dictionary<Bitmap, HashSet<Point>> topSideDrawingElements;
        private readonly Dictionary<Bitmap, HashSet<Point>> bottomSideDrawingElements;
        private Point topSidePlayerDrawingPosition;
        private Point bottomSidePlayerDrawingPosition;
        private int tickCount = 0;

        public GameWindow(ControlSettings controlSettings, int mapHeight, int mapWidth)
        {
            this.controlSettings = controlSettings;
            bottomSideState = new GameState(mapHeight, mapWidth);
            topSideState = new GameState(mapHeight, mapWidth);
            topSideDrawingElements = new Dictionary<Bitmap, HashSet<Point>>();
            bottomSideDrawingElements = new Dictionary<Bitmap, HashSet<Point>>();
            //TODO
            bottomIsRed = true;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(
                Visual.ElementSize * bottomSideState.MapWidth,
                Visual.ElementSize * (bottomSideState.MapHeight + topSideState.MapHeight + 2));
            //Size = ClientSize;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            BackColor = Color.Black;
            var timer = new Timer { Interval = 10 };
            timer.Tick += OnTick;
            timer.Start();
            //InitializeComponent();
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (tickCount == 0)
            {
                bottomSideState.GiveCommandsFromClient(new GameActCommands(controlSettings, pressedKeys));
                //bottomSideState.GiveClientCommands(new GameActCommands(controlSettings, new HashSet<Keys>() {Keys.W}));
                GameEngine.BeginAct(bottomSideState);
                GameEngine.BeginAct(topSideState);
                if (!bottomSideState.GameOver)
                    Sound.PlaySoundsAtBeginAct(bottomSideState.PlayerEntity);
            }

            Visual.UpdateDrawingElements(
                bottomSideDrawingElements,
                bottomSideState,
                true,
                bottomIsRed,
                tickCount,
                out bottomSidePlayerDrawingPosition);

            Visual.UpdateDrawingElements(
                topSideDrawingElements,
                topSideState,
                false,
                !bottomIsRed,
                tickCount,
                out topSidePlayerDrawingPosition);

            tickCount++;
            if (tickCount == 9)
            {
                GameEngine.EndAct(bottomSideState, topSideState);
                if (!bottomSideState.GameOver)
                    Sound.PlaySoundsAtEndAct(bottomSideState.PlayerEntity);
                tickCount = 0;
            }

            Invalidate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = @"Space Battle";
            DoubleBuffered = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.TranslateTransform(0, elementSize);
            foreach (var element in topSideDrawingElements)
            foreach (var point in element.Value)
                e.Graphics.DrawImage(element.Key, point);

            if (topSideState.PlayerEntity != null)
            {
                DrawPlayerHUD(e, topSideState.PlayerEntity, false);
                if (topSideState.PlayerEntity.ShieldStrength > 0)
                    e.Graphics.DrawImage(Visual.GetFlippedImage(Properties.Resources.Shield),
                        topSidePlayerDrawingPosition);
            }

            foreach (var element in bottomSideDrawingElements)
            foreach (var point in element.Value)
                e.Graphics.DrawImage(element.Key, point);

            if (bottomSideState.PlayerEntity != null)
            {
                DrawPlayerHUD(e, bottomSideState.PlayerEntity, true);
                if (bottomSideState.PlayerEntity.ShieldStrength > 0)
                    e.Graphics.DrawImage(Properties.Resources.Shield, bottomSidePlayerDrawingPosition);
            }

            //e.Graphics.ResetTransform();
        }

        private void DrawPlayerHUD(PaintEventArgs e, Player player, bool isBottom)
        {
            var middleX = ClientSize.Width / 2;
            int numericY;
            int scaledY;
            if (isBottom)
            {
                numericY = ClientSize.Height - Visual.ElementSize;
                scaledY = numericY + Visual.IconSize;
            }
            else
            {
                numericY = Visual.IconSize;
                scaledY = 0;
            }
            DrawHUDIcons(e, middleX, numericY, scaledY);

            DrawHUDNumber(e, Visual.IconSize, numericY, true, player.Armor.ToString());
            DrawHUDNumber(e, ClientSize.Width - Visual.ElementSize, numericY, false, $"{(int)(player.ShieldStrength * 100)}%");

            var rectangleSize = new Size(middleX / 20 - 1, Visual.IconSize);

            var leftX = middleX - Visual.IconSize - rectangleSize.Width;
            DrawHUDNumber(e, leftX, numericY, false, 10.ToString());//TODO: Вынести урон в public поле
            DrawHUDBar(e, leftX, scaledY, rectangleSize, Brushes.DarkRed, false, player.Health);
            DrawHUDNumber(e, leftX, scaledY, false, player.Health.ToString());

            var rightX = middleX + Visual.IconSize;
            DrawHUDNumber(e, rightX, numericY, true, 25.ToString());
            DrawHUDBar(e, rightX, scaledY, rectangleSize, Brushes.Indigo, true, player.Energy);
            DrawHUDNumber(e, rightX, scaledY, true, player.Energy.ToString());
        }

        private void DrawHUDIcons(PaintEventArgs e, int middleX, int numericY, int scaledY)
        {
            e.Graphics.DrawImage(Properties.Resources.HUDArmor, 0, numericY);
            e.Graphics.DrawImage(Properties.Resources.HUDShield, ClientSize.Width - Visual.IconSize, numericY);
            e.Graphics.DrawImage(Properties.Resources.HUDHealthDamage, middleX - Visual.IconSize, numericY);
            e.Graphics.DrawImage(Properties.Resources.HUDEnergyDamage, middleX, numericY);
            e.Graphics.DrawImage(Properties.Resources.HUDHealth, middleX - Visual.IconSize, scaledY);
            e.Graphics.DrawImage(Properties.Resources.HUDEnergy, middleX, scaledY);
        }
        
        private static void DrawHUDBar(PaintEventArgs e, int beginX, int beginY, Size rectangleSize, Brush brush, bool toRight, int value)
        {
            var deltaX = rectangleSize.Width + 1;
            if (!toRight)
                deltaX = -deltaX;

            for (int i = 0; i < value / 5; i++)
            {
                var rectangle = new Rectangle(beginX + i * deltaX, beginY, rectangleSize.Width, rectangleSize.Height);
                e.Graphics.FillRectangle(brush, rectangle);
            }
        }

        private static void DrawHUDNumber(PaintEventArgs e, int beginX, int beginY, bool toRight, string stringValue)
        {
            var numberX = beginX;
            if (!toRight)
                numberX -= stringValue.Length*9 - 2;
            e.Graphics.DrawString(stringValue, new Font("Eras Bold ITC", 10), Brushes.White, numberX, beginY);
        }
    }
}