using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using GameData.ClientInteraction;
using GameData.Packets;

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

            var engageButton = CreateButton(0, @"Engage");
            var preferredSideLabel = CreateLabel(engageButton.Bottom, @"Preferred side:");
            var preferredSideButton = CreateButton(preferredSideLabel.Bottom);
            var nameLabel = CreateLabel(preferredSideButton.Bottom, @"Name:");
            var nameText = CreateTextBox(nameLabel.Bottom, @"Player");
            var serverIPLabel = CreateLabel(nameText.Bottom, @"Server IP:");
            var serverIPText = CreateTextBox(serverIPLabel.Bottom, @"127.0.0.1");
            
            engageButton.Click += (sender, args) =>
            {
                
                var gameSession = new GameSession(
                    new ControlSettings(Keys.ControlKey, Keys.ShiftKey, Keys.Right, Keys.Left, Keys.Up, Keys.Down),
                    isRed,
                    11,
                    10,
                    10);
                Visible = false;
                gameSession.GameForm.ShowDialog(this);
                
                /*
                var connectingLabel = CreateLabel(0);
                var ipIsCorrect = IPAddress.TryParse(serverIPText.Text, out var ip);
                connectingLabel.Text = ipIsCorrect ? $@"Connecting to {ip}" : @"Wrong IP";
                Controls.Add(connectingLabel);

                if (ipIsCorrect)
                {
                    
                    var clientHello = new ClientHello(nameText.Text, isRed);
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ip, 1997);
                    socket.Send(clientHello, 0, SocketFlags.None);
                    socket.Receive()
                    

                }
                else
                {
                    var returnButton = CreateButton(connectingLabel.Bottom, @"Return to menu");
                    //TODO: возврат в главное меню
                    //returnButton.Click += (sender, args) => { return; };.
                    Controls.Add(returnButton);
                }
                */
                nameLabel.Dispose();
                nameText.Dispose();
                preferredSideLabel.Dispose();
                preferredSideButton.Dispose();
                serverIPLabel.Dispose();
                serverIPText.Dispose();
                engageButton.Dispose();
                Dispose();
            };
            
            SetButtonColor(preferredSideButton, isRed);
            preferredSideButton.Click += (sender, args) =>
            {
                isRed = !isRed;
                SetButtonColor(preferredSideButton, isRed);
            };

            Controls.Add(engageButton);
            Controls.Add(preferredSideLabel);
            Controls.Add(preferredSideButton);
            Controls.Add(nameLabel);
            Controls.Add(nameText);
            Controls.Add(serverIPLabel);
            Controls.Add(serverIPText);

            //InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = @"Space Battle";
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
            control.Font = new Font(Visual.FontName, 16);
            if (text != null)
                control.Text = text;
        }

        private void SetButtonColor(Button button, bool isRed)
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