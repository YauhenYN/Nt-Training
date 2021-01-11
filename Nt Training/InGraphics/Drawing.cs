using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Nt_Training.InGraphics
{
    public interface IDraw
    {
        void DrawOn(Bitmap GraphicsBuffer);
    }
    public class Drawing
    {
        public delegate void DrawingHandler(Bitmap GraphicsBuffer);
        public event DrawingHandler OnDraw;
        Control _control;
        Color _backColor = Color.Red; //ЗАМЕНИТЬ НА НОРМАЛЬНЫЙ ЭЛЕМЕНТ, ЛУЧШЕ СТРУКТУРУ ГДЕ МОЖНО БЫЛО БЫ ИСПОЛЬЗОВАТЬ ЛИБО ЦВЕТ, ЛИБО КАРТИНКУ
        Bitmap _graphicsBuffer { get; set; }
        public Drawing SetBuffer(Control control, Color backColor)
        {
            _control = control;
            _backColor = backColor;
            _graphicsBuffer = new Bitmap(control.Width, control.Height);
            return this;
        }
        public void Draw()
        {
            using (Graphics graphics = _control.CreateGraphics())
            {
                using (Graphics imageGraphics = Graphics.FromImage(_graphicsBuffer))
                {
                    imageGraphics.Clear(_backColor);
                }
                OnDraw(_graphicsBuffer);
                graphics.DrawImage(_graphicsBuffer, new Point());
            }
        }
        public void DisposeBuffer()
        {
            _graphicsBuffer.Dispose();
        }
        public void RefreshBuffer()
        {
            _graphicsBuffer = new Bitmap(_control.Width, _control.Height);
        }
    }
}
