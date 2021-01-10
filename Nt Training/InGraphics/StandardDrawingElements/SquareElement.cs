using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.InGraphics.StandardDrawingElements
{
    public class SquareElement : Moving
    {
        Color _color;
        Rectangle _squareElement;
        public SquareElement(Color color, int firstPositionX, int firstPositionY, int sizePx)
        {
            _color = color;
            _squareElement = new Rectangle(firstPositionX, firstPositionY, sizePx, sizePx);
        }
        public override void DrawOn(Bitmap imageBuffer)
        {
            using (Graphics graphics = Graphics.FromImage(imageBuffer))
            {
                Brush b = new SolidBrush(_color);
                graphics.FillRectangle(b, _squareElement);
            }
        }
        public override void MoveOn(int px, MoveTo whereToMove)
        {
            _squareElement = sides[(int)whereToMove].AddToSide(_squareElement, px);
        }
    }
}
