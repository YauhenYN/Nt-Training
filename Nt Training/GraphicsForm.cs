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
            _map.ChangeDirection(90, InGraphics.Moving.Direction.left, 1);
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

            SystemNetwork.Networks.LearningMethods.Learning learning = new SystemNetwork.Networks.LearningMethods.MOPLearning() { SpeedE = 1, MomentA = 0.6};
            SystemNetwork.Neurons.InputNeuron[] inputNeurons = new SystemNetwork.Neurons.InputNeuron[3];
            inputNeurons[0] = new SystemNetwork.Neurons.InputNeuron();
            inputNeurons[1] = new SystemNetwork.Neurons.InputNeuron();
            inputNeurons[2] = new SystemNetwork.Neurons.InputNeuron();

            SystemNetwork.Neurons.AverageNeuron[] averageNeurons = new SystemNetwork.Neurons.AverageNeuron[4];
            averageNeurons[0] = new SystemNetwork.Neurons.AverageNeuron();
            averageNeurons[1] = new SystemNetwork.Neurons.AverageNeuron();
            averageNeurons[2] = new SystemNetwork.Neurons.AverageNeuron();
            averageNeurons[3] = new SystemNetwork.Neurons.AverageNeuron();

            SystemNetwork.Neurons.OutputNeuron[] outputNeurons = new SystemNetwork.Neurons.OutputNeuron[2];
            outputNeurons[0] = new SystemNetwork.Neurons.OutputNeuron();
            outputNeurons[1] = new SystemNetwork.Neurons.OutputNeuron();

            SystemNetwork.Bonds.Bond[] bonds = new SystemNetwork.Bonds.Bond[12];
            Random rand = new Random();
            bonds[0] = new SystemNetwork.Bonds.Bond(inputNeurons[0], averageNeurons[0], 0.90);
            bonds[1] = new SystemNetwork.Bonds.Bond(inputNeurons[0], averageNeurons[1], 0.534);
            bonds[2] = new SystemNetwork.Bonds.Bond(inputNeurons[1], averageNeurons[1], 0.29);
            bonds[3] = new SystemNetwork.Bonds.Bond(inputNeurons[1], averageNeurons[2], -0.48);
            bonds[4] = new SystemNetwork.Bonds.Bond(inputNeurons[2], averageNeurons[2], -0.11);
            bonds[5] = new SystemNetwork.Bonds.Bond(inputNeurons[2], averageNeurons[3], 0.9);
            bonds[6] = new SystemNetwork.Bonds.Bond(averageNeurons[0], outputNeurons[0], -0.8);
            bonds[7] = new SystemNetwork.Bonds.Bond(averageNeurons[1], outputNeurons[0], 0.5);
            bonds[8] = new SystemNetwork.Bonds.Bond(averageNeurons[1], outputNeurons[1], 0.65);
            bonds[9] = new SystemNetwork.Bonds.Bond(averageNeurons[2], outputNeurons[0], 0.24);
            bonds[10] = new SystemNetwork.Bonds.Bond(averageNeurons[2], outputNeurons[1], 0.116);
            bonds[11] = new SystemNetwork.Bonds.Bond(averageNeurons[3], outputNeurons[1], -0.98);

            inputNeurons[0].AddOutPutBond(bonds[0]);
            inputNeurons[0].AddOutPutBond(bonds[1]);
            inputNeurons[1].AddOutPutBond(bonds[2]);
            inputNeurons[1].AddOutPutBond(bonds[3]);
            inputNeurons[2].AddOutPutBond(bonds[4]);
            inputNeurons[2].AddOutPutBond(bonds[5]);

            averageNeurons[0].AddOutputBond(bonds[6]);
            averageNeurons[0].AddInputBond(bonds[0]);
            averageNeurons[1].AddOutputBond(bonds[7]);
            averageNeurons[1].AddOutputBond(bonds[8]);
            averageNeurons[1].AddInputBond(bonds[1]);
            averageNeurons[1].AddInputBond(bonds[2]);
            averageNeurons[2].AddOutputBond(bonds[9]);
            averageNeurons[2].AddOutputBond(bonds[10]);
            averageNeurons[2].AddInputBond(bonds[3]);
            averageNeurons[2].AddInputBond(bonds[4]);
            averageNeurons[3].AddOutputBond(bonds[11]);
            averageNeurons[3].AddInputBond(bonds[5]);
            outputNeurons[0].AddInputBond(bonds[6]);
            outputNeurons[0].AddInputBond(bonds[7]);
            outputNeurons[0].AddInputBond(bonds[9]);
            outputNeurons[1].AddInputBond(bonds[8]);
            outputNeurons[1].AddInputBond(bonds[10]);
            outputNeurons[1].AddInputBond(bonds[11]);

            SystemNetwork.Layers.AverageLayer averageLayer = new SystemNetwork.Layers.AverageLayer(averageNeurons);

            SystemNetwork.Neurons.ActivationFunctions.LogisticFunction function = new SystemNetwork.Neurons.ActivationFunctions.LogisticFunction(2);

            SystemNetwork.Networks.Network network = new SystemNetwork.Networks.Network(function);
            network.AddInPutNeurons(inputNeurons);
            network.AddAverageLayer(averageLayer);
            network.AddOutPutNeurons(outputNeurons);
            network.SetTeaching(learning);
            for (int step = 0; step < 10; step++)
            {
                double[] results = network.Start(0, 1, 0.2);
                foreach (double result in results) MessageBox.Show(result.ToString() + " result");
                MessageBox.Show(network.TeachNetwork(0.4, 0.5).ToString());
                network.DisposeNeurons();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            _map.ChangeDirection(60, InGraphics.Moving.Direction.left, 1);
            //_map.MoveOn(5, InGraphics.Moving.MoveTo.right);
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
            _map.ChangeDirection(70, InGraphics.Moving.Direction.right, 1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _map.ChangeDirection(70, InGraphics.Moving.Direction.left, 1);
            //_map.MoveOn(5, InGraphics.Moving.MoveTo.left);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _map.ChangeDirection(70, InGraphics.Moving.Direction.left, 1);
            //_map.MoveOn(5, InGraphics.Moving.MoveTo.down);
        }
    }
}
