using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Nt_Training.InGraphics
{
    public enum TypeOfDisplaying
    {
        fullElementSize,
        inNaturalSize
    }
    public enum MoveTo
    {
        top,
        down,
        left,
        right
    }
    public class GraphicsMap
    {
        Rectangle[,] map;
        TypeOfDisplaying typeOfDisplaying;
        int px;
        Color colorOfBlocks;
        Color backColor;
        public GraphicsMap(Color colorOfBlocks, Color backColor, int linesCount, int columnsCount, TypeOfDisplaying type = TypeOfDisplaying.inNaturalSize, int px = 15)
        {
            map = new Rectangle[linesCount, columnsCount];
            typeOfDisplaying = type;
            this.px = px;
            this.colorOfBlocks = colorOfBlocks;
            this.backColor = backColor;
        }
        public GraphicsMap(Color colorOfBlocks, Color backColor, int linesCount, int columnsCount, int px)
        {
            map = new Rectangle[linesCount, columnsCount];
            typeOfDisplaying = TypeOfDisplaying.inNaturalSize;
            this.px = px;
            this.colorOfBlocks = colorOfBlocks;
            this.backColor = backColor;
        }
        public void setBlock(int numberOfLine, int numberOfColumn)
        {
            map[numberOfLine, numberOfColumn] = new Rectangle(px * numberOfColumn, px * numberOfLine, px, px);
        }
        public void DrawOn(Control element)
        {
            using (Graphics graphics = element.CreateGraphics())
            {
                Pen pen = new Pen(colorOfBlocks);
                Brush b = new SolidBrush(colorOfBlocks);
                if (typeOfDisplaying == TypeOfDisplaying.fullElementSize)
                {

                }
                else
                {
                    foreach (Rectangle rectangle in map)
                    {
                        if (rectangle != null)
                        {
                            graphics.FillRectangle(b, rectangle);
                        }
                    }
                }
            }
        }
        public void MoveMapOn(Control element, int px, MoveTo whereToMove)
        {
            using (Graphics graphics = element.CreateGraphics())
            {
                int value;
                bool isX;
                if (whereToMove == MoveTo.top) { value = -px; isX = false; }
                else if (whereToMove == MoveTo.down) { value = px; isX = false; }
                else { value = whereToMove == MoveTo.left ? -px : px; isX = true; }
                Bitmap bitmap = new Bitmap(element.Width, element.Height);
                using (Graphics imageGraphics = Graphics.FromImage(bitmap))
                {
                    imageGraphics.Clear(backColor);
                    Pen pen = new Pen(colorOfBlocks);
                    Brush b = new SolidBrush(colorOfBlocks);
                    if (typeOfDisplaying == TypeOfDisplaying.fullElementSize)
                    {

                    }
                    else
                    {
                        for (int step = 0 / px; step < map.GetLength(0); step++)
                        {
                            for (int inStep = 0 / px; inStep < map.GetLength(1); inStep++)
                                if (map[step, inStep] != null)
                                {
                                    if (isX)
                                    {
                                        map[step, inStep].X += value;
                                    }
                                    else
                                    {
                                        map[step, inStep].Y += value;
                                    }
                                    imageGraphics.FillRectangle(b, map[step, inStep]);
                                }
                        }
                        graphics.DrawImage(bitmap, new Point());
                        bitmap.Dispose();
                        
                    }
                }
            }
        }
        //ОГРОМНАЯ ОШИБКА, НУЖНО СОЗДАТЬ ОТДЕЛЬНЫЙ БУФЕР ДЛЯ КАЖДОГО КАДРА
    }
}
