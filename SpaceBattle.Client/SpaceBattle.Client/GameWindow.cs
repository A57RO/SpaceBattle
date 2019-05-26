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
    public partial class GameWindow : Form
    {
        private const int elementSize = 32;
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private ControlSettings controlSettings;
        private GameState downSide;
        private GameState upperSide;
        private bool weAreRed;
        private List<EntityAnimation> downSideAnimations;
        private List<EntityAnimation> upperSideAnimations;
        private int tickCount = 0;

        public GameWindow()
        {
            controlSettings = new ControlSettings(Keys.RControlKey, Keys.RShiftKey, Keys.Right, Keys.Left, Keys.Up, Keys.Down);
            var mapSize = new Size(10, 5);
            downSide = new GameState(mapSize);
            upperSide = new GameState(mapSize);
            //TODO
            weAreRed = true;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(
                elementSize * (downSide.MapSize.Width + upperSide.MapSize.Width),
                elementSize * (downSide.MapSize.Height + upperSide.MapSize.Height));
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            /*
            if (imagesDirectory == null)
                imagesDirectory = new DirectoryInfo("Images");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);

            initializeStartScreen();
            */
            var timer = new Timer { Interval = 10 };
            timer.Tick += (sender, args) =>
            {
                if (tickCount == 0)
                {
                    downSide.Commands = new GameActCommands(controlSettings, pressedKeys);
                    GameEngine.BeginAct(downSide);
                    downSideAnimations = downSide.Animations;
                    upperSide.Commands = new GameActCommands(controlSettings, new HashSet<Keys>());
                    GameEngine.BeginAct(upperSide);
                    upperSideAnimations = upperSide.Animations;
                }
                /*
                foreach (var animation in downSideAnimations)
                    animation.Location = new Point(animation.Location.X + 8 * animation.Command.DeltaX, animation.Location.Y + 8 * animation.Command.DeltaY);
                */
                if (tickCount == 8)
                {
                    GameEngine.EndAct(downSide, upperSide);
                    tickCount = 0;
                }

                tickCount++;

                Invalidate();
            };
            timer.Start();
            //InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = @"SugarCrusade";
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
            if (downSideAnimations == null) return;
            foreach (var a in downSideAnimations)
                e.Graphics.DrawImage(GetImageForEntity(a.Entity, weAreRed),
                    a.PositionAtBeginAct.X + 4 * a.Action.DeltaX * tickCount,
                    upperSide.MapSize.Height + a.PositionAtBeginAct.Y + 4 * a.Action.DeltaY * tickCount);
            //e.Graphics.ResetTransform();
        }

        protected Bitmap GetImageForEntity(IEntity entity, bool colorIsRed)
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
            if (colorIsRed)
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
