using System;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameData;
using GameData.ClientInteraction;

namespace Client.Forms
{
    public sealed class MainMenuForm : Form
    {
        private readonly Button engageButton;
        private readonly Label preferredSideLabel;
        private readonly Button preferredSideButton;
        private readonly Label nameLabel;
        private readonly TextBox nameText;
        private readonly Label serverIPLabel;
        private readonly TextBox serverIPText;

        private readonly Label connectingLabel;
        private readonly Label statusLabel;
        private readonly Button returnButton;

        public MainMenuForm(bool playerIsRed, string playerName, ControlSettings controls)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.Manual;
            Location = Point.Empty;
            BackColor = Color.Black;
            BackgroundImageLayout = ImageLayout.Center;
            BackgroundImage = Properties.Resources.BackgroundMainMenu;
            ClientSize = new Size(200, 416);

            engageButton = CreateButton(0, @"Engage");
            preferredSideLabel = CreateLabel(engageButton.Bottom, @"Preferred side:");
            preferredSideButton = CreateButton(preferredSideLabel.Bottom);
            nameLabel = CreateLabel(preferredSideButton.Bottom, @"Name:");
            nameText = CreateTextBox(nameLabel.Bottom, playerName);
            serverIPLabel = CreateLabel(nameText.Bottom, @"Server IP:");
            serverIPText = CreateTextBox(serverIPLabel.Bottom, @"127.0.0.1");
            SetMainMenuVisible(true);

            connectingLabel = CreateLabel(0);
            statusLabel = CreateLabel(connectingLabel.Bottom);
            statusLabel.Size = new Size(statusLabel.Size.Width, 320);
            SetConnectionScreenVisible(false);
            returnButton = CreateButton(statusLabel.Bottom, @"Return to menu");
            returnButton.Visible = false;

            var gameSession = new GameSession(
                11,
                10,
                10,
                controls);


            engageButton.Click += (sender, args) =>
            {
                SetMainMenuVisible(false);
                SetConnectionScreenVisible(true);

                if (IPAddress.TryParse(serverIPText.Text, out var ip))
                {
                    connectingLabel.Text = $@"Connection to {ip}";
                    Task.Run(() => TryConnect(new IPEndPoint(ip, Network.ServerPort), gameSession, playerIsRed));
                }
                else
                {
                    connectingLabel.Text = @"Wrong IP";
                    returnButton.Visible = true;
                }
            };

            SetButtonColor(preferredSideButton, playerIsRed);
            preferredSideButton.Click += (sender, args) =>
            {
                playerIsRed = !playerIsRed;
                SetButtonColor(preferredSideButton, playerIsRed);
            };

            returnButton.Click += (sender1, args1) =>
            {
                statusLabel.Text = string.Empty;
                SetConnectionScreenVisible(false);
                returnButton.Visible = false;
                SetMainMenuVisible(true);
            };

            Controls.Add(engageButton);
            Controls.Add(preferredSideLabel);
            Controls.Add(preferredSideButton);
            Controls.Add(nameLabel);
            Controls.Add(nameText);
            Controls.Add(serverIPLabel);
            Controls.Add(serverIPText);

            Controls.Add(connectingLabel);
            Controls.Add(statusLabel);
            Controls.Add(returnButton);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = @"Space Battle";
        }

        private void SetMainMenuVisible(bool visible)
        {
            nameLabel.Visible =
                nameText.Visible =
                    preferredSideLabel.Visible =
                        preferredSideButton.Visible =
                            serverIPLabel.Visible =
                                serverIPText.Visible =
                                    engageButton.Visible = visible;
        }

        private void SetConnectionScreenVisible(bool visible)
        {
            connectingLabel.Visible = statusLabel.Visible = visible;
        }

        private void TryConnect(IPEndPoint server, GameSession gameSession, bool playerIsRed)
        {
            try
            {
                var allPlayersConnected = gameSession.ConnectToServer(server, playerIsRed, nameText.Text);
                if (!allPlayersConnected)
                {
                    BeginInvoke(new Action(() => statusLabel.Text = @"Waiting for second player"));
                    gameSession.WaitForEnemy();
                }
                BeginInvoke(new Action(() =>
                {
                    gameSession.Start();
                    Visible = false;
                    gameSession.GameForm.ShowDialog(this);
                    Dispose();
                }));

            }
            catch (Exception e)
            {
                BeginInvoke(new Action(() =>
                {
                    statusLabel.Text = e.Message;
                    returnButton.Visible = true;
                }));
            }
        }

        private Label CreateLabel(int y, string text = null)
        {
            var label = new Label();
            SetControlDefaultOptions(label, y, 64, Color.Transparent, text);
            label.TextAlign = ContentAlignment.MiddleCenter;
            if (text != null)
                label.Text = text;
            return label;
        }

        private Button CreateButton(int y, string text = null)
        {
            var button = new Button();
            SetControlDefaultOptions(button, y, 32, Color.Black, text);
            button.TextAlign = ContentAlignment.MiddleCenter;
            if (text != null)
                button.Text = text;
            return button;
        }

        private TextBox CreateTextBox(int y, string text = null)
        {
            var textBox = new TextBox();
            SetControlDefaultOptions(textBox, y, 32, Color.Black, text);
            textBox.TextAlign = HorizontalAlignment.Center;
            return textBox;
        }

        private void SetControlDefaultOptions(Control control, int topY, int height, Color backColor, string text)
        {
            control.AutoSize = false;
            control.Location = new Point(0, topY);
            control.Size = new Size(ClientSize.Width, height);
            control.BackColor = backColor;
            control.ForeColor = Color.White;
            control.Font = Visual.ButtonsFont;
            if (text != null)
                control.Text = text;
        }

        private static void SetButtonColor(Button button, bool isRed)
        {
            if (isRed)
            {
                button.BackColor = Color.DarkRed;
                button.Text = @"Red";
            }
            else
            {
                button.BackColor = Color.DarkBlue;
                button.Text = @"Blue";
            }
        }
    }
}