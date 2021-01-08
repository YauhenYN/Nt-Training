using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Nt_Training.InGraphics
{
    //Сделать класс защищённым
    static class Drawing
    {
        static Bitmap GraphicsBuffer { get; set; }
        public static void SetBuffer(Control element)
        {
            GraphicsBuffer = new Bitmap(element.Width, element.Height);
        }
        public static void Draw(Control element)
        {
            using (Graphics graphics = element.CreateGraphics())
            {
                graphics.DrawImage(GraphicsBuffer, new PointF());
            }
        }
    }
}
