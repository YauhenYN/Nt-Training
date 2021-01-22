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
        Point locationOfObject;
        SystemNetwork.Networks.Network _network;
        InGraphics.Moving.MoveTo _direction;
        private void GraphicsForm_Shown(object sender, EventArgs e)
        {
            locationOfObject.X = 200;
            locationOfObject.Y = 200;
            _map = new InGraphics.StandardDrawingElements.Square2DMap(Color.Gray, 45, 45, 15);
            _map.SetCommonDegree();
            _direction = InGraphics.Moving.MoveTo.left;
            _character = new InGraphics.StandardDrawingElements.SquareElement(Color.Blue, locationOfObject.X, locationOfObject.Y, 30);
            for(int step = 0; step < 40; step++)
            {
                if(step < 25)_map.setBlock(0, step);
                _map.setBlock(8, step);
                _map.setBlock(16, step);
                _map.setBlock(24, step);
                _map.setBlock(32, step);
                _map.setBlock(step, 40);
                _map.setBlock(step, 0);
            }


            _drawing = new InGraphics.Drawing().SetBuffer(panelForDrawing, Color.White);
            _drawing.OnDraw += _map.DrawOn;
            _drawing.OnDraw += _character.DrawOn;
            timer1.Enabled = true;

            SystemNetwork.Networks.LearningMethods.Learning learning = new SystemNetwork.Networks.LearningMethods.MOPLearning() { SpeedE = 0.8, MomentA = 0.4};
            SystemNetwork.Neurons.InputNeuron[] inputNeurons = new SystemNetwork.Neurons.InputNeuron[4];
            for (int step = 0; step < inputNeurons.Length; step++) inputNeurons[step] = new SystemNetwork.Neurons.InputNeuron();

            SystemNetwork.Neurons.AverageNeuron[] averageNeurons = new SystemNetwork.Neurons.AverageNeuron[8];
            for (int step = 0; step < averageNeurons.Length; step++) averageNeurons[step] = new SystemNetwork.Neurons.AverageNeuron();

            SystemNetwork.Neurons.AverageNeuron[] averageNeurons1 = new SystemNetwork.Neurons.AverageNeuron[8];
            for (int step = 0; step < averageNeurons1.Length; step++) averageNeurons1[step] = new SystemNetwork.Neurons.AverageNeuron();

            SystemNetwork.Neurons.OutputNeuron[] outputNeurons = new SystemNetwork.Neurons.OutputNeuron[4];
            for (int step = 0; step < outputNeurons.Length; step++) outputNeurons[step] = new SystemNetwork.Neurons.OutputNeuron();

            List<SystemNetwork.Bonds.Bond> bonds = new List<SystemNetwork.Bonds.Bond>();
            Random rand = new Random();
            foreach(SystemNetwork.Neurons.InputNeuron inputNeuron in inputNeurons)
            {
                foreach(SystemNetwork.Neurons.AverageNeuron averageNeuron in averageNeurons)
                {
                    bonds.Add(new SystemNetwork.Bonds.Bond(inputNeuron, averageNeuron, Convert.ToDouble(rand.Next(-100, 100)) / 100));
                    inputNeuron.AddOutPutBond(bonds.Last());
                    averageNeuron.AddInputBond(bonds.Last());
                    foreach (SystemNetwork.Neurons.AverageNeuron averageNeuron1 in averageNeurons1)
                    {
                        bonds.Add(new SystemNetwork.Bonds.Bond(averageNeuron, averageNeuron1, Convert.ToDouble(rand.Next(-100, 100)) / 100));
                        averageNeuron.AddOutputBond(bonds.Last());
                        averageNeuron1.AddInputBond(bonds.Last());
                        foreach (SystemNetwork.Neurons.OutputNeuron outputNeuron in outputNeurons)
                        {
                            bonds.Add(new SystemNetwork.Bonds.Bond(averageNeuron1, outputNeuron, Convert.ToDouble(rand.Next(-100, 100)) / 100));
                            averageNeuron1.AddOutputBond(bonds.Last());
                            outputNeuron.AddInputBond(bonds.Last());
                        }
                    }
                }
            }

            SystemNetwork.Layers.AverageLayer averageLayer = new SystemNetwork.Layers.AverageLayer(averageNeurons);
            SystemNetwork.Layers.AverageLayer averageLayer1 = new SystemNetwork.Layers.AverageLayer(averageNeurons1);

            SystemNetwork.Neurons.ActivationFunctions.LogisticFunction function = new SystemNetwork.Neurons.ActivationFunctions.LogisticFunction(0.5);

            _network = new SystemNetwork.Networks.Network(function);
            _network.AddInPutNeurons(inputNeurons);
            _network.AddAverageLayer(averageLayer);
            _network.AddAverageLayer(averageLayer1);
            _network.AddOutPutNeurons(outputNeurons);
            _network.SetTeaching(learning);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //_map.ChangeDirection(60, InGraphics.Moving.Direction.downleft, 1);
            _map.MoveOn(5, InGraphics.Moving.MoveTo.left);
        }
        private int count = 0;
        private InGraphics.Moving.MoveTo[] _mirroredDirections = { InGraphics.Moving.MoveTo.down, InGraphics.Moving.MoveTo.top, InGraphics.Moving.MoveTo.right, InGraphics.Moving.MoveTo.left };
        private void timer1_Tick(object sender, EventArgs e)
        {
            _map.MoveOn(1, _direction);
            _drawing.Draw();
            _drawing.DisposeBuffer();
            _drawing.RefreshBuffer();
            // РАССТОЯНИЕ(0-1)[4] - НАПРАВЛЕНИЕ(0.25, 0.5, 0.75, 1) - ГРАДУСЫ(0-0.9)
            if (count % 25 == 0) {
                bool[,] map = _map.getAreaMap(new Rectangle(0, 0, locationOfObject.X * 2, locationOfObject.X * 2));
                Point location = new Point(locationOfObject.X, locationOfObject.Y);
                int topDistance = FindObstacle(map, location, InGraphics.Moving.Direction.topleft, 90);
                int downDistance = FindObstacle(map, location, InGraphics.Moving.Direction.downright, 90);
                int leftDistance = FindObstacle(map, location, InGraphics.Moving.Direction.downleft, 0);
                int rightDistance = FindObstacle(map, location, InGraphics.Moving.Direction.downright, 0);
                double[] results = _network.Start((double)topDistance / 200, (double)downDistance / 200, (double)leftDistance / 200, (double)rightDistance / 200);
                _direction = _mirroredDirections[(int)IntoDirection(results)];
                if ((topDistance > 0 && topDistance < 30) || (rightDistance > 0 && rightDistance < 30) || (downDistance > 0 && downDistance < 30) || (leftDistance > 0 && leftDistance < 30))
                {
                    int indexOfMax = (int)IntoDirection(new double[] { topDistance, downDistance, leftDistance, rightDistance });
                    double[] copying = (double[])results.Clone();
                    for (int step = 0; step < copying.Length; step++) copying[step] = 0;
                    copying[indexOfMax] = 1;
                    _network.TeachNetwork(copying);
                    _map.ReturnToStart();
                    //MessageBox.Show(indexOfMax.ToString());
                    //MessageBox.Show(copying[0] + " " + copying[1] + " " + copying[2] + " " + copying[3]);
                    //MessageBox.Show(topDistance + " " + downDistance + " " + leftDistance + " " + rightDistance);
                    MessageBox.Show(results[0].ToString() + " " + results[1].ToString() + " " + results[2].ToString() + " " + results[3].ToString());
                }
                _network.DisposeNeurons();
            }
            count++;
        }
        private InGraphics.Moving.MoveTo IntoDirection(double[] results)
        {
            double max = results.Contains(0) ? 0 : results.Max();
            int number = 0;
            for (int step = 0; step < results.Length; step++) if (results[step] == max) { number = step; break; }
            return (InGraphics.Moving.MoveTo)number;
        }
        public int FindObstacle(bool[,] map, Point position, InGraphics.Moving.Direction direction, int leftDegrees)
        { //НУЖНО ПЕРЕДЕЛАТЬ ВСЁ, Т.К. POINT.X, POINT.Y != [X,Y]
            int count = 0;
            double divider = 90 / 1;
            double sumX = leftDegrees / divider;
            double sumY = (90 - leftDegrees) / divider;
            if (direction == InGraphics.Moving.Direction.downleft)
            {
                sumY *= -1;
            }
            else if(direction == InGraphics.Moving.Direction.topright)
            {
                sumX *= -1;
            }
            else if(direction == InGraphics.Moving.Direction.topleft)
            {
                sumX *= -1;
                sumY *= -1;
            }
            for (double xBuffer = position.X + sumX, yBuffer = position.Y + sumY; Convert.ToInt32(xBuffer) < map.GetLength(0) && Convert.ToInt32(yBuffer) < map.GetLength(1) && xBuffer >= 0 && yBuffer >= 0; xBuffer += sumX, yBuffer += sumY)
            {
                count++;
                if (map[Convert.ToInt32(xBuffer), Convert.ToInt32(yBuffer)]) return count;
            }
            return 0;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.down);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //_map.ChangeDirection(70, InGraphics.Moving.Direction.downleft, 1);
            _map.MoveOn(5, InGraphics.Moving.MoveTo.right);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //_map.ChangeDirection(70, InGraphics.Moving.Direction.topleft, 1);
            _map.MoveOn(5, InGraphics.Moving.MoveTo.top);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled) timer1.Enabled = false;
            else timer1.Enabled = true;
        }
    }
}
