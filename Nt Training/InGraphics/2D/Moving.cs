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
    { //СРОЧНО ИЗМЕНИТЬ ПОВЕДЕНИЕ ДВИЖЕНИЯ, УБРАТЬ RECTANGLE, А ЛУЧШЕ ИЗМЕНИТЬ ДВИЖЕНИЕ ПО POINT.X/Y
        public enum Direction
        {
            downright,
            downleft,
            topleft,
            topright
        }
        private class EntireAngle //Лучше бы переделать в обычную очередь
        {
            public class Angle
            {
                int _angleDegrees;
                public int AngleDegrees { get { return _angleDegrees; } set { if (value < 90 && value > -1) _angleDegrees = value; } }
                private double _remainder;
                private double _neededCellValue;
                public Angle(int degrees, double divider) : this(degrees)
                {
                    _remainder = degrees / divider;
                    _neededCellValue = _remainder;
                }
                public Angle(int degrees)
                {
                    AngleDegrees = degrees;
                }
                public int GetSwitch()
                {
                    if (_remainder < 1) { _remainder += _neededCellValue; return 0; }
                    else { int answer = Convert.ToInt32(_remainder); _remainder = _neededCellValue; return answer;}
                }
            }
            public Angle leftAngle { get; private set; }
            public Angle rightAngle { get; private set; }
            private int xVector = 1;
            private int yVector = 1;
            public void setAnglesValue(int leftAngleDegrees, Direction direction, double speed)
            {
                double divider = 90 / speed;
                leftAngle = new Angle(leftAngleDegrees, divider);
                rightAngle = new Angle(90 - leftAngleDegrees, divider);
                if(direction == Direction.downright) //НУЖНО УБРАТЬ IF
                {
                    xVector = 1;
                    yVector = 1;
                }
                else if (direction == Direction.downleft)
                {
                    xVector = -1;
                    yVector = 1;
                }
                else if(direction == Direction.topleft)
                {
                    xVector = -1;
                    yVector = -1;
                }
                else
                {
                    xVector = 1;
                    yVector = -1;
                }
            }
            public Point getNextPoint()
            {
                return leftAngle != null ? new Point(leftAngle.GetSwitch() * xVector, rightAngle.GetSwitch() * yVector) : new Point(0, 0);
            }
        }
        public Moving() { }
        EntireAngle _entireAngle;
        public void SetCommonDegree()
        {        //commonDegrees - НЕТ БЕЗОПАСНОСТИ ЗНАЧЕНИЙ, НУЖНО ХОТЯ-БЫ СОЗДАТЬ ОТДЕЛЬНЫЙ ТИП С УГЛОМ
            _entireAngle = new EntireAngle();
        }
        public void ChangeDirection(int leftAnglesDegrees, Direction direction, double speed)
        {
            _entireAngle.setAnglesValue(leftAnglesDegrees, direction, speed);
        }
        protected Point GetAddingPoint() => _entireAngle != null ? _entireAngle.getNextPoint() : new Point(0, 0);
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
            public abstract void AddToSide<T>(T figure, int px) where T : _2D.IFigureParameters;
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
