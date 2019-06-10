using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Client.Forms;
using GameData.ClientInteraction;

namespace Client
{
    public static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //TODO: установка сохранённых имени, цвета, настроек управления
            RunOnline();
            //RunSolo();
        }

        private static void RunOnline()
        {
            Application.Run(new MainMenuForm(
                true,
                "Player1",
                new ControlSettings(Keys.ControlKey, Keys.ShiftKey, Keys.Right, Keys.Left, Keys.Up, Keys.Down)));
        }

        private static void RunSolo()
        {
            var gameSession = new GameSession(
                11, 
                10, 
                10,
                new ControlSettings(Keys.ControlKey, Keys.ShiftKey, Keys.Right, Keys.Left, Keys.Up, Keys.Down));
            gameSession.Start();
            Application.Run(gameSession.GameForm);
        }
    }
}