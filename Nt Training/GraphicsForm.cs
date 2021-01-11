using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Nt_Training
{
    public partial class GraphicsForm : Form
    {
        public GraphicsForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        InGraphics.Drawing _drawing;
        InGraphics.StandardDrawingElements.Square2DMap _map;
        InGraphics.StandardDrawingElements.SquareElement _character;

        private void GraphicsForm_Shown(object sender, EventArgs e)
        {
            _map = new InGraphics.StandardDrawingElements.Square2DMap(Color.Gray, 45, 45, 15);
            _map.SetCommonDegree();
            _map.ChangeDirection(80, InGraphics.Moving.Direction.top, 1);
            _character = new InGraphics.StandardDrawingElements.SquareElement(Color.Blue, 50, 50, 30);
            for(int step = 0; step < 40; step++)
            {
                _map.setBlock(0, step);
            }
            for (int step = 1; step < 40; step++)
            {
                _map.setBlock(step, step);
            }
            _map.setBlock(2, 6);
            _map.setBlock(3, 9);
            _drawing = new InGraphics.Drawing().SetBuffer(panelForDrawing, Color.White);
            _drawing.OnDraw += _map.DrawOn;
            _drawing.OnDraw += _character.DrawOn;
            timer1.Enabled = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.right);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            _map.Move();
            _drawing.Draw();
            _drawing.DisposeBuffer();
            _drawing.RefreshBuffer();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.top);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.left);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.down);
        }
    }
}
