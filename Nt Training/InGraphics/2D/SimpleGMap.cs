using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training.InGraphics._2D
{
    public class SimpleGMap<T> : Moving, ILocation where T : IDraw, IFill, IFigureParameters, ILocation
    {
        private T[,] _map;
        private int _blockPx;
        private int _x;
        private int _y;
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        public SimpleGMap(int linesCount, int columnsCount, int px = 15)
        {
            _map = new T[linesCount, columnsCount];
            _blockPx = px;
        }

        public virtual void DrawOn(Bitmap imageBuffer)
        {
            using (Graphics graphics = Graphics.FromImage(imageBuffer))
            {
                foreach (var element in _map)
                {
                    if (element != null)
                    {
                        element.Draw(imageBuffer);
                    }
                }
            }
        }

        public virtual void FillOn(Bitmap imageBuffer)
        {
            using (Graphics graphics = Graphics.FromImage(imageBuffer))
            {
                foreach (var element in _map)
                {
                    if (element != null)
                    {
                        element.Fill(imageBuffer);
                    }
                }
            }
        }

        public virtual void MoveOn(int px, Moving.MoveTo whereToMove)
        {
            sides[(int)whereToMove].AddToSide(this, px);
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                    if (_map[step, inStep] != null)
                    {
                        sides[(int)whereToMove].AddToSide(_map[step, inStep], px);
                    }
            }
        }
        public virtual void MoveByDegrees()
        {
            Point addingPoint = GetAddingPoint();
            _x += addingPoint.X;
            _y += addingPoint.Y;
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                    if (_map[step, inStep] != null)
                    {
                        _map[step, inStep].X += addingPoint.X;
                        _map[step, inStep].Y += addingPoint.Y;
                    }
            }
            //MessageBox.Show(addingPoint.X.ToString() + "     -     " + addingPoint.Y.ToString());
        }
        public void SetBlock(int numberOfLine, int numberOfColumn, T drawingElement)
        {
            drawingElement.X = numberOfColumn * _blockPx;
            drawingElement.Y = numberOfLine * _blockPx;
            drawingElement.Heigh = _blockPx;
            drawingElement.Width = _blockPx;
            _map[numberOfLine, numberOfColumn] = drawingElement;
        }
        public void ReturnToStart()
        {
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                {
                    if(_map[step, inStep] != null)
                    {
                        _map[step, inStep].X = inStep * _blockPx;
                        _map[step, inStep].Y = step * _blockPx;
                    }
                }
            }
            _x = 0;
            _y = 0;
        }
        public bool[,] GetAreaMap(Rectangle localArea) //ПЕРЕРАБОТАТЬ
        {
            /*Bitmap bitmap = new Bitmap(_map.GetLength(1) * _blockPx, _map.GetLength(0) * _blockPx);
            FillOn(bitmap);
            bool[,] map = new bool[localArea.Height, localArea.Width];
            for (int step = 0; step < map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < map.GetLength(1); inStep++)
                { 
                    if (bitmap.GetPixel(localArea.Y + inStep, localArea.X + step) != Color.FromArgb(0)) map[step, inStep] = true;
                }
            }
            bitmap.Dispose();
            return map;*/
            bool[,] map = new bool[localArea.Height, localArea.Width];
            for (int line = 0; line < _map.GetLength(0); line++)
            {
                for (int column = 0; column < _map.GetLength(1); column++)
                {
                    if(_map[line, column] != null)
                    {
                        if(_map[line, column].X + _blockPx > localArea.X && _map[line, column].Y + _blockPx > localArea.Y && _map[line, column].X < localArea.Height + localArea.X && _map[line, column].Y < localArea.Width + localArea.Y)
                        {
                            for(int step = _map[line, column].X; step < _map[line, column].X + _blockPx; step++)
                            {
                                for(int inStep = _map[line, column].Y; inStep < _map[line, column].Y + _blockPx; inStep++)
                                {
                                    if (step >= localArea.X && step < localArea.Height + localArea.X && inStep >= localArea.Y && inStep < localArea.Width + localArea.Y) map[inStep - localArea.Y, step - localArea.X] = true;
                                }
                            }
                        }
                    }
                }
            }
            return map;
        }
    }
}
