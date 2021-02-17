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
using Nt_Training.InGraphics._2D;
using Nt_Training.InGraphics._2D.StandardDrawingElements;
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
        SimpleGMap<GRectangle> _map;
        SimpleGElement<GRectangle> _character;
        Point locationOfObject;
        SystemNetwork.Networks.Network _network;
        InGraphics.Moving.MoveTo _direction;
        private void GraphicsForm_Shown(object sender, EventArgs e)
        {
            locationOfObject.X = 200;
            locationOfObject.Y = 200;
            _map = new SimpleGMap<GRectangle>(45, 45, 15);
            _map.SetCommonDegree();
            _character = new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(locationOfObject.X, locationOfObject.Y, 30, 30), Color.Blue));
            for(int step = 0; step < 40; step++)
            {
                if (step < 25)
                {
                    _map.SetBlock(0, step, new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(), Color.Aqua)));
                    _map.SetBlock(8, step, new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(), Color.AliceBlue)));
                }
                _map.SetBlock(24, step, new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(), Color.Red)));
                _map.SetBlock(32, step, new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(), Color.Azure)));
                _map.SetBlock(step, 40, new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(), Color.BlueViolet)));
                _map.SetBlock(step, 0, new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(), Color.Brown)));
            }


            _drawing = new InGraphics.Drawing().SetBuffer(panelForDrawing, Color.White);
            _drawing.OnDraw += _map.FillOn;
            _drawing.OnDraw += _character.FillOn;
            timer1.Enabled = true;

            SystemNetwork.Networks.LearningMethods.Learning learning = new SystemNetwork.Networks.LearningMethods.MOPLearning() { SpeedE = 0.3, MomentA = 0.4};
            SystemNetwork.Neurons.InputNeuron[] inputNeurons = new SystemNetwork.Neurons.InputNeuron[4];
            for (int step = 0; step < inputNeurons.Length; step++) inputNeurons[step] = new SystemNetwork.Neurons.InputNeuron();

            SystemNetwork.Neurons.HiddenNeuron[] averageNeurons = new SystemNetwork.Neurons.HiddenNeuron[8];
            for (int step = 0; step < averageNeurons.Length; step++) averageNeurons[step] = new SystemNetwork.Neurons.HiddenNeuron();

            SystemNetwork.Neurons.HiddenNeuron[] averageNeurons1 = new SystemNetwork.Neurons.HiddenNeuron[8];
            for (int step = 0; step < averageNeurons1.Length; step++) averageNeurons1[step] = new SystemNetwork.Neurons.HiddenNeuron();

            SystemNetwork.Neurons.OutputNeuron[] outputNeurons = new SystemNetwork.Neurons.OutputNeuron[4];
            for (int step = 0; step < outputNeurons.Length; step++) outputNeurons[step] = new SystemNetwork.Neurons.OutputNeuron();

            List<SystemNetwork.Bonds.Bond> bonds = new List<SystemNetwork.Bonds.Bond>();
            Random rand = new Random();
            foreach(SystemNetwork.Neurons.InputNeuron inputNeuron in inputNeurons)
            {
                foreach(SystemNetwork.Neurons.HiddenNeuron averageNeuron in averageNeurons)
                {
                    bonds.Add(new SystemNetwork.Bonds.Bond(inputNeuron, averageNeuron, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                    inputNeuron.AddOutPutBond(bonds.Last());
                    averageNeuron.AddInputBond(bonds.Last());
                    foreach (SystemNetwork.Neurons.HiddenNeuron averageNeuron1 in averageNeurons1)
                    {
                        bonds.Add(new SystemNetwork.Bonds.Bond(averageNeuron, averageNeuron1, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                        averageNeuron.AddOutputBond(bonds.Last());
                        averageNeuron1.AddInputBond(bonds.Last());
                        foreach (SystemNetwork.Neurons.OutputNeuron outputNeuron in outputNeurons)
                        {
                            bonds.Add(new SystemNetwork.Bonds.Bond(averageNeuron1, outputNeuron, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                            averageNeuron1.AddOutputBond(bonds.Last());
                            outputNeuron.AddInputBond(bonds.Last());
                        }
                    }
                }
            }

            SystemNetwork.Layers.HiddenLayer averageLayer = new SystemNetwork.Layers.HiddenLayer(averageNeurons);
            SystemNetwork.Layers.HiddenLayer averageLayer1 = new SystemNetwork.Layers.HiddenLayer(averageNeurons1);

            SystemNetwork.Neurons.ActivationFunctions.LogisticFunction function = new SystemNetwork.Neurons.ActivationFunctions.LogisticFunction(2);

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
            _map.MoveOn(1, _mirroredDirections[(int)_direction]);
            _drawing.Draw();
            _drawing.DisposeBuffer();
            _drawing.RefreshBuffer();
            // РАССТОЯНИЕ(0-1)[4]
            if (count % 10 == 0) {
                bool[,] map = _map.getAreaMap(new Rectangle(0, 0, locationOfObject.X * 2, locationOfObject.X * 2));
                Point location = new Point(locationOfObject.X, locationOfObject.Y);
                int topDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.top);
                int downDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.down);
                int leftDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.left);
                int rightDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.right);
                double[] results = _network.Start((double)topDistance / 200, (double)downDistance / 200, (double)leftDistance / 200, (double)rightDistance / 200);
                _direction = IntoDirection(results);
                double[] distances = { topDistance, downDistance, leftDistance, rightDistance };
                if (distances[(int)_direction] < 30)
                {
                    int directionOfMax = (int)IntoDirection(distances);
                    double[] copying = (double[])results.Clone();
                    copying[(int)_direction] = 0;
                    copying[directionOfMax] = 1;
                    _network.TeachNetwork(copying);
                    _map.ReturnToStart();
                    //MessageBox.Show(topDistance.ToString() + " - " + downDistance.ToString() + " - " + leftDistance.ToString() + " - " + rightDistance.ToString());
                    //MessageBox.Show(results[0].ToString() + " " + results[1].ToString() + " " + results[2].ToString() + " " + results[3].ToString());
                }
                _network.DisposeNeurons();
            }
            count++;
        }
        private InGraphics.Moving.MoveTo IntoDirection(double[] results)
        {
            double max = results.Max();
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
            return int.MaxValue;
        }
        public int FindObstacle(bool[,] map, Point position, int length, InGraphics.Moving.MoveTo direction)
        {
            int count = 0, xSum = 0, ySum = 0;
            if (direction == InGraphics.Moving.MoveTo.down)
            {
                ySum = 1;
                position.Y += length;
            }
            else if(direction == InGraphics.Moving.MoveTo.left)
            {
                xSum = -1;
            }
            else if(direction == InGraphics.Moving.MoveTo.right)
            {
                xSum = 1;
                position.X += length;
            }
            else
            {
                ySum = -1;
            }
            for (int step = 0; step < length; step++)
            {
                for(int xBuffer = position.X, yBuffer = position.Y; yBuffer + (step * Math.Abs(xSum)) < map.GetLength(0) && yBuffer + (step * Math.Abs(xSum)) >= 0 && xBuffer + (step * Math.Abs(ySum)) < map.GetLength(1) && xBuffer + (step * Math.Abs(ySum)) >= 0; xBuffer += xSum, yBuffer += ySum)
                {
                    count++;
                    if (map[yBuffer + (step * Math.Abs(xSum)), xBuffer + (step * Math.Abs(ySum))]) return count;
                }
                count = 0;
            }
            return int.MaxValue;
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
