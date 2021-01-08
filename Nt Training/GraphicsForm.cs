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
        InGraphics.GraphicsMap map;
        private void GraphicsForm_Shown(object sender, EventArgs e)
        {
            map = new InGraphics.GraphicsMap(Color.Gray, Color.White, 45, 45, 15);
            for(int step = 0; step < 40; step++)
            {
                map.setBlock(0, step);
            }
            for (int step = 1; step < 40; step++)
            {
                map.setBlock(step, step);
            }
            map.setBlock(2, 6);
            map.setBlock(3, 9);
            map.DrawOn(panelForDrawing);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            map.MoveMapOn(panelForDrawing, 5, InGraphics.MoveTo.right);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            map.MoveMapOn(panelForDrawing, 1, InGraphics.MoveTo.right);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            map.MoveMapOn(panelForDrawing, 5, InGraphics.MoveTo.top);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            map.MoveMapOn(panelForDrawing, 5, InGraphics.MoveTo.left);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            map.MoveMapOn(panelForDrawing, 5, InGraphics.MoveTo.down);
        }
    }
}
