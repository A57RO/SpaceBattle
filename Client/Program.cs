using System;
using System.Windows.Forms;
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
            Application.Run(
                new GameWindow(
                    new ControlSettings(Keys.ControlKey, Keys.ShiftKey, Keys.Right, Keys.Left, Keys.Up, Keys.Down),
                    10,
                    11));
        }
    }
}