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
        public class EntireAngle
        {
            public class Angle
            {
                byte _angleDegrees;
                public byte AngleDegrees { get { return _angleDegrees; } set { if (value < 181) _angleDegrees = value; } }
                private double _remainder;
                private double _neededCellValue;
                public Angle(byte degrees, int divider) : this(degrees)
                {
                    _remainder = degrees / divider;                    //ГДЕ-ТО ГУЛЯЕТ ЦЕЛОЕ ЧИСЛО
                    _neededCellValue = _remainder;
                }
                public Angle(byte degrees)
                {
                    AngleDegrees = degrees;
                }
                public int GetSwitch()
                {
                    //MessageBox.Show(_remainder.ToString());
                    if (_remainder < 1) { _remainder += _neededCellValue; return 0; }
                    else { _remainder = _neededCellValue; return (int)_remainder; }
                }
            }
            Angle _commonAngle;
            public Angle leftAngle { get; private set; }
            public Angle rightAngle { get; private set; }
            public EntireAngle(byte commonDegrees)
            {
                _commonAngle = new Angle(commonDegrees);
            }
            public void setAnglesValue(byte leftAngleDegrees, int speed)
            {
                int divider = _commonAngle.AngleDegrees / speed;
                leftAngle = new Angle(leftAngleDegrees, divider);
                
                rightAngle = new Angle((byte)(_commonAngle.AngleDegrees - leftAngleDegrees), divider);
            }
            public Point getNextPoint()
            {
                return leftAngle != null ? new Point(leftAngle.GetSwitch(), rightAngle.GetSwitch()) : new Point(0, 0);
            }
        }
        public Moving() { }
        EntireAngle _entireAngle;
        public void SetCommonDegree(byte commonDegrees)
        {        //commonDegrees - НЕТ БЕЗОПАСНОСТИ, НУЖНО ХОТЯ-БЫ СОЗДАТЬ ОТДЕЛЬНЫЙ ТИП С УГЛОМ
            _entireAngle = new EntireAngle(commonDegrees);
        }
        public void ChangeDirection(byte leftAnglesDegrees, int speed)
        {
            _entireAngle.setAnglesValue(leftAnglesDegrees, speed);
        }
        protected Point GetNextCell() => _entireAngle != null ? _entireAngle.getNextPoint() : new Point(0, 0);
        protected static MovingToSide[] sides { get; } = { new MovingUp(), new MovingDown(), new MovingToLeft(), new MovingToRight() };
        public enum MoveTo
        {
            top,
            down,
            left,
            right
        }
        public abstract void DrawOn(Bitmap imageBuffer);
        public abstract void MoveOn(int px, MoveTo WhereToMove);
        protected abstract class MovingToSide
        {
            public abstract Rectangle AddToSide(Rectangle rectangle, int px);
        }
        protected class MovingUp : MovingToSide
        {
            public override Rectangle AddToSide(Rectangle rectangle, int px)
            {
                rectangle.Y -= px;
                return rectangle;
            }
        }
        protected class MovingDown : MovingToSide
        {
            public override Rectangle AddToSide(Rectangle rectangle, int px)
            {
                rectangle.Y += px;
                return rectangle;
            }
        }
        protected class MovingToLeft : MovingToSide
        {
            public override Rectangle AddToSide(Rectangle rectangle, int px)
            {
                rectangle.X -= px;
                return rectangle;
            }
        }
        protected class MovingToRight : MovingToSide
        {
            public override Rectangle AddToSide(Rectangle rectangle, int px)
            {
                rectangle.X += px;
                return rectangle;
            }
        }
    }
}
