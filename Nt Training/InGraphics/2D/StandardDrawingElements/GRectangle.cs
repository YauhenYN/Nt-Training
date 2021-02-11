using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nt_Training.InGraphics._2D.StandardDrawingElements
{
    public class GRectangle : Moving, IDraw, IFill, IMoving // ИЗМЕНИТЬ MOVING
    {
        public Rectangle Rectangle { get; private set; }
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
        public override void MoveOn(int px, MoveTo whereToMove) => Rectangle = sides[(int)whereToMove].AddToSide(Rectangle, px);

        public void MoveByDegrees(Point addingPoint) => Rectangle = new Rectangle(Rectangle.X + addingPoint.X, Rectangle.Y + addingPoint.Y, Rectangle.Width, Rectangle.Height);
    }
}
