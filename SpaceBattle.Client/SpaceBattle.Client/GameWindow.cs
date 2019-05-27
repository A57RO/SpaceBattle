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
        private const int elementSize = 32;
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private readonly ControlSettings controlSettings = new ControlSettings(Keys.W, Keys.S, Keys.Right, Keys.Left, Keys.Up, Keys.Down);
        private readonly Size mapSize = new Size(10, 5);
        private bool bottomIsRed;
        private GameState topSide;
        private GameState bottomSide;
        private Dictionary<Bitmap, Point> topSideGraphics;
        private Dictionary<Bitmap, Point> bottomSideGraphics;

        public GameWindow()
        {
            bottomSide = new GameState(mapSize.Height, mapSize.Width);
            topSide = new GameState(mapSize.Height, mapSize.Width);
            topSideGraphics = new Dictionary<Bitmap, Point>();
            bottomSideGraphics = new Dictionary<Bitmap, Point>();
            //TODO
            bottomIsRed = true;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(
                elementSize * bottomSide.MapWidth,
                elementSize * (bottomSide.MapHeight + topSide.MapHeight));
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            BackColor = Color.Black;
            /*
            if (imagesDirectory == null)
                imagesDirectory = new DirectoryInfo("Images");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);

            initializeStartScreen();
            */
            var timer = new Timer { Interval = 10 };
            var tickCount = 0;
            timer.Tick += (sender, args) =>
            {
                if (tickCount == 0)
                {
                    bottomSide.GiveClientCommands(new GameActCommands(controlSettings, pressedKeys));
                    //bottomSide.GiveClientCommands(new GameActCommands(controlSettings, new HashSet<Keys>() {Keys.W}));
                    GameEngine.BeginAct(bottomSide);
                    GameEngine.BeginAct(topSide);
                }
                /*
                foreach (var animation in downSideAnimations)
                    animation.Location = new Point(animation.Location.X + 8 * animation.Command.DeltaX, animation.Location.Y + 8 * animation.Command.DeltaY);
                */

                topSideGraphics = topSide.Animations.ToDictionary(
                    a => GetImageForEntity(a.Entity, false),
                    a => new Point(
                        a.BeginActLocation.X * elementSize + tickCount * 4 * a.Action.DeltaX,
                        a.BeginActLocation.Y * elementSize + tickCount * 4 * a.Action.DeltaY));
                bottomSideGraphics = bottomSide.Animations.ToDictionary(
                    a => GetImageForEntity(a.Entity, true),
                    a => new Point(
                        a.BeginActLocation.X * elementSize + tickCount * 4 * a.Action.DeltaX,
                        (topSide.MapHeight + a.BeginActLocation.Y) * elementSize + tickCount * 4 * a.Action.DeltaY));

                
                tickCount++;
                if (tickCount == 9)
                {
                    GameEngine.EndAct(bottomSide, topSide);
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
            foreach (var a in topSideGraphics)
                e.Graphics.DrawImage(a.Key, a.Value);
            foreach (var a in bottomSideGraphics)
                e.Graphics.DrawImage(a.Key, a.Value);
            //e.Graphics.ResetTransform();
        }

        protected Bitmap GetImageForEntity(IEntity entity, bool sideIsBottom)
        {
            var red = new Dictionary<string, Bitmap>
            {
                {typeof(Data.Entities.Player).Name, Properties.Resources.PlayerRed},
                {typeof(Data.Entities.FriendlyLaserShot).Name, Properties.Resources.ShotRed},
                {typeof(Data.Entities.EnemyLaserShot).Name, Properties.Resources.ShotBlue}
            };
            var blue = new Dictionary<string, Bitmap>
            {
                {typeof(Data.Entities.Player).Name, Properties.Resources.PlayerBlue},
                {typeof(Data.Entities.FriendlyLaserShot).Name, Properties.Resources.ShotBlue},
                {typeof(Data.Entities.EnemyLaserShot).Name, Properties.Resources.ShotRed}
            };
            
            var common = new Dictionary<string, Bitmap>
            {
            };

            var name = entity.GetType().Name;
            if (sideIsBottom && bottomIsRed || !sideIsBottom && !bottomIsRed)
            {
                if (red.TryGetValue(name, out var redSprite))
                    return redSprite;
            }
            else
            {
                if (blue.TryGetValue(name, out var blueSprite))
                    return blueSprite;
            }
            if (common.TryGetValue(name, out var sprite))
                return sprite;

            throw new ArgumentException($"Unknown entity {entity}");
        }
    }
}
