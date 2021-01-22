using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Nt_Training.InGraphics.StandardDrawingElements
{
    public enum TypeOfDisplaying
    {
        fullElementSize,
        inNaturalSize
    }
    public class Square2DMap : Moving, IDraw
    {
        Rectangle[,] _map;
        TypeOfDisplaying _typeOfDisplaying;
        int _blockPX;
        Color _colorOfBlocks;
        public Square2DMap(Color colorOfBlocks, int linesCount, int columnsCount, TypeOfDisplaying type = TypeOfDisplaying.inNaturalSize, int px = 15)
        {
            _map = new Rectangle[linesCount, columnsCount];
            _typeOfDisplaying = type;
            _blockPX = px;
            _colorOfBlocks = colorOfBlocks;
        }
        public Square2DMap(Color colorOfBlocks, int linesCount, int columnsCount, int px) : this(colorOfBlocks, linesCount, columnsCount, TypeOfDisplaying.inNaturalSize, px)
        {
        }
        public void setBlock(int numberOfLine, int numberOfColumn) => _map[numberOfLine, numberOfColumn] = numberOfLine < _map.GetLength(0) && numberOfColumn < _map.GetLength(1) ? new Rectangle(_blockPX * numberOfColumn, _blockPX * numberOfLine, _blockPX, _blockPX) : throw new ArgumentOutOfRangeException();
        public override void DrawOn(Bitmap imageBuffer)
        {
            using (Graphics graphics = Graphics.FromImage(imageBuffer))
            {
                Brush b = new SolidBrush(_colorOfBlocks);
                if (_typeOfDisplaying == TypeOfDisplaying.fullElementSize)
                {

                }
                else
                {
                    foreach (Rectangle rectangle in _map)
                    {
                        if (rectangle != null)
                        {
                            graphics.FillRectangle(b, rectangle);
                        }
                    }
                }
            }
        }
        public override void MoveOn(int px, MoveTo whereToMove)
        {
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                    if (_map[step, inStep] != null)
                    {
                        _map[step, inStep] = sides[(int)whereToMove].AddToSide(_map[step, inStep], px);
                    }
            }
        }
        public void Move() //НЕТ В MOVING
        {
            Point nextCell = GetNextCell();
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                    if (_map[step, inStep] != null)
                    {
                        _map[step, inStep].X += nextCell.X;
                        _map[step, inStep].Y += nextCell.Y;
                    }
            }
        }
        public bool[,] getAreaMap(Rectangle localArea)
        {
            Bitmap bitmap = new Bitmap(_map.GetLength(1) * _blockPX, _map.GetLength(0) * _blockPX);
            DrawOn(bitmap);
            bool[,] map = new bool[localArea.Height, localArea.Width];
            for(int step = 0; step < map.GetLength(0); step++)
            {
                for(int inStep = 0; inStep < map.GetLength(1); inStep++)
                { //НЕТ X И Y ОТКУДА НАЧИНАТЬ
                    //MessageBox.Show(bitmap.GetPixel(localArea.Width + step, localArea.Height + inStep).ToArgb().ToString());
                    if (bitmap.GetPixel(localArea.Y + inStep, localArea.X + step) != Color.FromArgb(0)) map[step, inStep] = true;
                }
            } //ПРОВЕРИТЬ
            bitmap.Dispose();
            return map;
        }
        public void ReturnToStart()
        {
            for (int step = 0; step < _map.GetLength(0); step++)
            {
                for (int inStep = 0; inStep < _map.GetLength(1); inStep++)
                {
                    _map[step, inStep].Y = _blockPX * step;
                    _map[step, inStep].X = _blockPX * inStep;
                }
            }
        }
    }
}
