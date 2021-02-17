using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.InGraphics._2D
{
    public class SimpleGMap<T> : Moving where T : IDraw, IFill, IMoving, IFigureParameters
    {
        SimpleGElement<T>[,] _map;
        int _blockPx;
        public SimpleGMap(int linesCount, int columnsCount, int px = 15)
        {
            _map = new SimpleGElement<T>[linesCount, columnsCount];
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
                        element.DrawOn(imageBuffer);
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
                        element.FillOn(imageBuffer);
                    }
                }
            }
        }

        public virtual void MoveOn(int px, Moving.MoveTo whereToMove)
        {
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                    if (_map[step, inStep] != null)
                    {
                        _map[step, inStep].MoveOn(px, whereToMove);
                    }
            }
        }
        public virtual void MoveByDegrees()
        {
            Point addingPoint = GetAddingPoint();
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                    if (_map[step, inStep] != null)
                    {
                        _map[step, inStep].MoveByDegrees(addingPoint);
                    }
            }
        }
        public void SetBlock<I>(int numberOfLine, int numberOfColumn, I drawingElement) where I : SimpleGElement<T>
        {
            drawingElement.X = numberOfColumn * _blockPx;
            drawingElement.Y = numberOfLine * _blockPx;
            drawingElement.Heigh = _blockPx;
            drawingElement.Width = _blockPx;
            drawingElement.FixPosition();
            _map[numberOfLine, numberOfColumn] = drawingElement;
        }
        public void ReturnToStart()
        {
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                {
                    if(_map[step, inStep] != null) _map[step, inStep].ReturnToStart();
                }
            }
        }
        public bool[,] getAreaMap(Rectangle localArea) //ПЕРЕРАБОТАТЬ
        {
            Bitmap bitmap = new Bitmap(_map.GetLength(1) * _blockPx, _map.GetLength(0) * _blockPx);
            DrawOn(bitmap);
            bool[,] map = new bool[localArea.Height, localArea.Width];
            for (int step = 0; step < map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < map.GetLength(1); inStep++)
                { 
                    if (bitmap.GetPixel(localArea.Y + inStep, localArea.X + step) != Color.FromArgb(0)) map[step, inStep] = true;
                }
            }
            bitmap.Dispose();
            return map;
        }
    }
}
