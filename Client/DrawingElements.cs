using System.Collections.Generic;
using System.Drawing;

namespace Client
{
    public class DrawingElements
    {
        public Dictionary<Bitmap, HashSet<Point>> Images;
        public Dictionary<Rectangle, Brush> Rectangles;
        public Dictionary<string, HashSet<Point>> Strings;

        public DrawingElements()
        {
            Images = new Dictionary<Bitmap, HashSet<Point>>();
            Rectangles = new Dictionary<Rectangle, Brush>();
            Strings = new Dictionary<string, HashSet<Point>>();
        }

        public void Clear()
        {
            Images.Clear();
            Rectangles.Clear();
            Strings.Clear();
        }
    }
}