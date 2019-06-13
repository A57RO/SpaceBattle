using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Forms
{
    public sealed class GameForm : Form
    {
        public readonly HashSet<Keys> PressedKeys = new HashSet<Keys>();

        public DrawingElements TopSideHUD;
        public DrawingElements TopSideField;
        public DrawingElements BottomSideHUD;
        public DrawingElements BottomSideField;

        public GameForm(int mapWidth, int mapHeight)
        {
            Visible = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Black;
            ClientSize = new Size(Visual.ElementSize * mapWidth, Visual.ElementSize * (mapHeight + 2));
            //Size = ClientSize;
            BackgroundImageLayout = ImageLayout.Center;
            BackgroundImage = Properties.Resources.BackgroundGame;

            TopSideHUD = new DrawingElements();
            TopSideField = new DrawingElements();
            BottomSideHUD = new DrawingElements();
            BottomSideField = new DrawingElements();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Owner != null)
            {
                Text = Owner.Text + Text;
                Location = Owner.Location;
            }
            DoubleBuffered = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            PressedKeys.Add(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            PressedKeys.Remove(e.KeyCode);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (BottomSideHUD)
            {
                DrawElements(e, BottomSideHUD);
            }
            lock (BottomSideField)
            {
                DrawElements(e, BottomSideField);
            }
            lock (TopSideHUD)
            {
                DrawElements(e, TopSideHUD);
            }
            lock (TopSideField)
            {
                DrawElements(e, TopSideField);
            }
        }

        private void DrawElements(PaintEventArgs e, DrawingElements elements)
        {
            foreach (var element in elements.Images)
                foreach (var point in element.Value)
                    e.Graphics.DrawImage(element.Key, point);

            foreach (var drawingRectangle in elements.Rectangles)
                e.Graphics.FillRectangle(drawingRectangle.Value, drawingRectangle.Key);

            foreach (var drawingString in elements.Strings)
                foreach (var point in drawingString.Value)
                    e.Graphics.DrawString(drawingString.Key, Visual.HUDNumbersFont, Brushes.White, point);
        }
    }
}