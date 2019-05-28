using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpaceBattle.Data;
using SpaceBattle.Data.ClientInteraction;

namespace SpaceBattle.Client
{
    public class GameWindow : Form
    {
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private readonly ControlSettings controlSettings = new ControlSettings(Keys.ControlKey, Keys.ShiftKey, Keys.Right, Keys.Left, Keys.Up, Keys.Down);
        private readonly Size mapSize = new Size(10, 5);
        private bool bottomIsRed;
        private GameState topSideState;
        private GameState bottomState;
        private readonly Dictionary<Bitmap, HashSet<Point>> topStateDrawingElements;
        private readonly Dictionary<Bitmap, HashSet<Point>> bottomDrawingElements;

        public GameWindow()
        {
            bottomState = new GameState(mapSize.Height, mapSize.Width);
            topSideState = new GameState(mapSize.Height, mapSize.Width);
            topStateDrawingElements = new Dictionary<Bitmap, HashSet<Point>>();
            bottomDrawingElements = new Dictionary<Bitmap, HashSet<Point>>();
            //TODO
            bottomIsRed = true;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(
                Visual.ElementSize * bottomState.MapWidth,
                Visual.ElementSize * (bottomState.MapHeight + topSideState.MapHeight));
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            BackColor = Color.Black;
            var timer = new Timer { Interval = 10 };
            var tickCount = 0;
            timer.Tick += (sender, args) =>
            {
                if (tickCount == 0)
                {
                    bottomState.GiveCommandsFromClient(new GameActCommands(controlSettings, pressedKeys));
                    //bottomSide.GiveClientCommands(new GameActCommands(controlSettings, new HashSet<Keys>() {Keys.W}));
                    GameEngine.BeginAct(bottomState);
                    GameEngine.BeginAct(topSideState);
                }

                Visual.UpdateDrawingElements(
                    bottomDrawingElements,
                    bottomState.Animations,
                    true,
                    bottomIsRed,
                    bottomState.MapWidth,
                    bottomState.MapHeight,
                    tickCount);

                Visual.UpdateDrawingElements(
                    topStateDrawingElements,
                    topSideState.Animations,
                    false,
                    !bottomIsRed,
                    topSideState.MapWidth,
                    topSideState.MapHeight,
                    tickCount);

                tickCount++;
                if (tickCount == 9)
                {
                    GameEngine.EndAct(bottomState, topSideState);
                    tickCount = 0;
                }

                Invalidate();

            };
            timer.Start();
            //InitializeComponent();
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
            foreach (var element in topStateDrawingElements)
            foreach (var point in element.Value)
                e.Graphics.DrawImage(element.Key, point);
            foreach (var element in bottomDrawingElements)
            foreach (var point in element.Value)
                e.Graphics.DrawImage(element.Key, point);
            //e.Graphics.ResetTransform();
        }
    }
}