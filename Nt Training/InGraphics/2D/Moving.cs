using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Nt_Training.InGraphics
{
    public abstract class Moving
    { 
        public enum Direction
        {
            downright,
            downleft,
            topleft,
            topright
        }
        public Moving() { }
        private double _sumX, _sumY;
        private int _lastLeftDegrees;
        private Direction _lastDirection;
        private double _lastSpeed;
        public void ChangeDirection(int leftDegrees, Direction direction, double speed)
        {
            if (_lastLeftDegrees != leftDegrees || _lastDirection != direction || _lastSpeed != speed)
            {
                _sumX = (double)leftDegrees / 90 * speed;
                _sumY = (90 - (double)leftDegrees) / 90 * speed;
                if (direction == Direction.downleft)
                {
                    _sumY *= -1;
                }
                else if (direction == Direction.topright)
                {
                    _sumX *= -1;
                }
                else if (direction == Direction.topleft)
                {
                    _sumX *= -1;
                    _sumY *= -1;
                }
                if(Math.Abs(leftDegrees) != 90 && Math.Abs(leftDegrees) != 0)
                {
                    _sumX *= 2;
                    _sumY *= 2;
                }
                _xBuffer = 0;
                _yBuffer = 0;
                _lastLeftDegrees = leftDegrees;
                _lastDirection = direction;
                _lastSpeed = speed;
            }
        }
        private double _xBuffer, _yBuffer;
        protected Point GetAddingPoint()
        {
            _xBuffer += _sumX;
            _yBuffer += _sumY;
            Point outPoint = new Point((int)_xBuffer, (int)_yBuffer);
            if (Math.Abs(_xBuffer) >= 1) _xBuffer -= (int)_xBuffer;
            if (Math.Abs(_yBuffer) >= 1) _yBuffer -= (int)_yBuffer;
            return outPoint;
        }
        protected static MovingToSide[] sides { get; } = { new MovingUp(), new MovingDown(), new MovingToLeft(), new MovingToRight() };
        public enum MoveTo
        {
            top,
            down,
            left,
            right
        }
        protected abstract class MovingToSide
        {
            public abstract void AddToSide<T>(T figure, int px) where T : _2D.ILocation;
        }
        protected class MovingUp : MovingToSide
        {
            public override void AddToSide<T>(T figure, int px)
            {
                figure.Y -= px;
            }
        }
        protected class MovingDown : MovingToSide
        {

            public override void AddToSide<T>(T figure, int px)
            {
                figure.Y += px;
            }
        }
        protected class MovingToLeft : MovingToSide
        {

            public override void AddToSide<T>(T figure, int px)
            {
                figure.X -= px;
            }
        }
        protected class MovingToRight : MovingToSide
        {

            public override void AddToSide<T>(T figure, int px)
            {
                figure.X += px;
            }
        }
    }
}
