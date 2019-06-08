using System;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using GameData.ClientInteraction;

namespace Client.Forms
{
    public sealed class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            BackColor = Color.Black;
            BackgroundImageLayout = ImageLayout.Center;
            BackgroundImage = Properties.Resources.BackgroundMainMenu;
            ClientSize = new Size(200, 400);

            var isRed = true;

            /*
            var playLabel = new Label();
            playLabel.AutoSize = false;
            playLabel.Location = new Point(0, 0);
            playLabel.Size = new Size(ClientSize.Width, 64);
            playLabel.BackColor = Color.Transparent;
            playLabel.ForeColor = Color.White;
            playLabel.Font = new Font(Visual.FontName, 16);
            playLabel.TextAlign = ContentAlignment.MiddleCenter;
            playLabel.Text = @"Play";
            Controls.Add(playLabel);
            */
            var engageButton = new Button();
            var preferredSideLabel = new Label();
            var choseSideButton = new Button();

            engageButton.AutoSize = false;
            engageButton.Location = new Point(0, ClientSize.Height / 3);
            engageButton.Size = new Size(ClientSize.Width, 32);
            engageButton.BackColor = Color.Black;
            engageButton.ForeColor = Color.White;
            engageButton.Font = new Font(Visual.FontName, 16);
            engageButton.TextAlign = ContentAlignment.MiddleCenter;
            engageButton.Text = @"Engage";
            engageButton.Click += (sender, args) =>
            {
                var gameForm = new GameForm(
                    new ControlSettings(Keys.ControlKey, Keys.ShiftKey, Keys.Right, Keys.Left, Keys.Up, Keys.Down),
                    11,
                    10);
                Visible = false;
                gameForm.ShowDialog(this);
                preferredSideLabel.Dispose();
                choseSideButton.Dispose();
                engageButton.Dispose();
                Dispose();
            };
            Controls.Add(engageButton);

            preferredSideLabel.AutoSize = false;
            preferredSideLabel.Location = new Point(0, engageButton.Bottom);
            preferredSideLabel.Size = new Size(ClientSize.Width, 64);
            preferredSideLabel.BackColor = Color.Transparent;
            preferredSideLabel.ForeColor = Color.White;
            preferredSideLabel.Font = new Font(Visual.FontName, 16);
            preferredSideLabel.TextAlign = ContentAlignment.MiddleCenter;
            preferredSideLabel.Text = @"Preferred side";
            Controls.Add(preferredSideLabel);

            choseSideButton.AutoSize = false;
            choseSideButton.Location = new Point(0, preferredSideLabel.Bottom);
            choseSideButton.Size = new Size(ClientSize.Width, 32);
            choseSideButton.BackColor = isRed ? Color.DarkRed : Color.DarkBlue;
            choseSideButton.ForeColor = Color.White;
            choseSideButton.Font = new Font(Visual.FontName, 16);
            choseSideButton.TextAlign = ContentAlignment.MiddleCenter;
            choseSideButton.Text = @"Red";
            
            choseSideButton.Click += (sender, args) =>
            {
                if (isRed)
                {
                    choseSideButton.BackColor = Color.DarkBlue;
                    choseSideButton.Text = @"Blue";
                }
                else
                {
                    choseSideButton.BackColor = Color.DarkRed;
                    choseSideButton.Text = @"Red";
                }
                isRed = !isRed;
            };
            Controls.Add(choseSideButton);
            
            //InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = @"Space Battle";
        }
    }
}