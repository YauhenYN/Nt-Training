using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Nt_Training.InGraphics._2D
{
    public interface IDraw
    {
        public void Draw(Bitmap imageBuffer);
    }
    public interface IFill
    {
        public void Fill(Bitmap imageBuffer);
    }
    public interface ILocation
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public interface IFigureParameters
    {
        public int Heigh { get; set; }
        public int Width { get; set; }
    }
    public class SimpleGElement<T> : Moving, IFigureParameters, ILocation where T : IDraw, IFill, IFigureParameters, ILocation
    {
        private Rectangle _startingParameters;
        public T Element { get; private set; }
        public SimpleGElement(T element)
        {
            Element = element;
        }
        public void FixPosition() => _startingParameters = new Rectangle(Element.X, Element.Y, Element.Width, Element.Heigh);
        public virtual void DrawOn(Bitmap imageBuffer) => Element.Draw(imageBuffer);
        public virtual void FillOn(Bitmap imageBuffer) => Element.Fill(imageBuffer);
        public virtual void MoveOn(int px, MoveTo whereToMove)
        {
            sides[(int)whereToMove].AddToSide(this, px);
        }
        public virtual void MoveByDegrees()
        {
            Point addingPoint = GetAddingPoint();
            Element.X += addingPoint.X;
            Element.Y += addingPoint.Y;
        }
        /// <summary>
        /// When it's necessary to use several parameters
        /// </summary>
        public virtual void MoveByDegrees(Point addingPoint)
        {
            Element.X += addingPoint.X;
            Element.Y += addingPoint.Y;
        }
        public int X { get => Element.X; set => Element.X = value; }
        public int Y { get => Element.Y; set => Element.Y = value; }
        public int Heigh { get => Element.Heigh; set => Element.Heigh = value; }
        public int Width { get => Element.Width; set => Element.Width = value; }
        public void ReturnToStart()
        {
            Element.X = _startingParameters.X;
            Element.Y = _startingParameters.Y;
            Element.Heigh = _startingParameters.Height;
            Element.Width = _startingParameters.Width;
        }
    }
}
