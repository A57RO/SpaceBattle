using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SpaceBattle.Data;
using SpaceBattle.Data.ClientInteraction;

namespace SpaceBattle.Client
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
                Visual.ElementSize * (bottomSideState.MapHeight + topSideState.MapHeight));
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
                //bottomSide.GiveClientCommands(new GameActCommands(controlSettings, new HashSet<Keys>() {Keys.W}));
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

            if (topSideState.PlayerEntity != null && topSideState.PlayerEntity.ShieldStrength > 0)
                e.Graphics.DrawImage(Visual.GetFlippedImage(Properties.Resources.Shield), topSidePlayerDrawingPosition);

            foreach (var element in bottomSideDrawingElements)
            foreach (var point in element.Value)
                e.Graphics.DrawImage(element.Key, point);

            if (bottomSideState.PlayerEntity != null && bottomSideState.PlayerEntity.ShieldStrength > 0)
                e.Graphics.DrawImage(Properties.Resources.Shield, bottomSidePlayerDrawingPosition);
            //e.Graphics.ResetTransform();
        }
    }
}