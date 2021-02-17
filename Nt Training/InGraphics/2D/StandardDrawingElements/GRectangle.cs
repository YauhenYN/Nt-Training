using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nt_Training.InGraphics._2D.StandardDrawingElements
{
    public class GRectangle : Moving, IDraw, IFill, IMoving, IFigureParameters // ИЗМЕНИТЬ MOVING
    {
        public Rectangle Rectangle { get; private set; }
        public int X { get => Rectangle.X; set => Rectangle = new Rectangle(value, Y, Width, Heigh); }
        public int Y { get => Rectangle.Y; set => Rectangle = new Rectangle(X, value, Width, Heigh); }
        public int Heigh { get => Rectangle.Height; set => Rectangle = new Rectangle(X, Y, Width, value); }
        public int Width { get => Rectangle.Width; set => Rectangle = new Rectangle(X, Y, value, Heigh); }

        private Color _color;
        public GRectangle(Rectangle rectangle, Color color)
        {
            Rectangle = rectangle;
            _color = color;
        }
        public void Draw(Bitmap imageBuffer)
        {
            using (Graphics graphics = Graphics.FromImage(imageBuffer))
            {
                Pen pen = new Pen(_color);
                graphics.DrawRectangle(pen, Rectangle);
            }
        }
        public void Fill(Bitmap imageBuffer)
        {
            using (Graphics graphics = Graphics.FromImage(imageBuffer))
            {
                Brush b = new SolidBrush(_color);
                graphics.FillRectangle(b, Rectangle);
            }
        }
        public void MoveOn(int px, MoveTo whereToMove) => sides[(int)whereToMove].AddToSide(this, px);

        public void MoveByDegrees(Point addingPoint) => Rectangle = new Rectangle(Rectangle.X + addingPoint.X, Rectangle.Y + addingPoint.Y, Rectangle.Width, Rectangle.Height);
    }
}
