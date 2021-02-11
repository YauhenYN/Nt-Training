using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nt_Training.InGraphics._2D
{
    public enum Direction
    {
        downright,
        downleft,
        topleft,
        topright
    }
    public interface IDraw
    {
        public void Draw(Bitmap imageBuffer);
    }
    public interface IFill
    {
        public void Fill(Bitmap imageBuffer);
    }
    public interface IMoving
    {
        public void MoveOn(int px, Moving.MoveTo whereToMove);
        public void MoveByDegrees(Point addingPoint);
    }
    public class SimpleGElement<T> : Moving where T : IDraw, IFill, IMoving
    {
        private T _startingElement;
        public T Element { get; private set; }
        public SimpleGElement(T element)
        {
            Element = element;
            _startingElement = element;
        }
        public virtual void DrawOn(Bitmap imageBuffer) => Element.Draw(imageBuffer);
        public virtual void FillOn(Bitmap imageBuffer) => Element.Fill(imageBuffer);
        public virtual void MoveOn(int px, Moving.MoveTo whereToMove) => Element.MoveOn(px, whereToMove);
        public virtual void MoveByDegrees()
        {
            Point addingPoint = GetNextCell();
            Element.MoveByDegrees(addingPoint);
        }
        /// <summary>
        /// When necessary to use several parameters
        /// </summary>
        public virtual void MoveByDegrees(Point addingPoint)
        {
            Element.MoveByDegrees(addingPoint);
        }
        public void ReturnToStart() => Element = _startingElement;
    }
}
