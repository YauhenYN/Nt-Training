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
        struct Action
        {
            public int Degrees { get; }
            public InGraphics.Moving.Direction Direction { get; }
            public int NumberOfArray { get; }
            public Action(int degrees, InGraphics.Moving.Direction direction, int numberOfArray)
            {
                Degrees = degrees;
                Direction = direction;
                NumberOfArray = numberOfArray;
            }
        }
        struct State : IEquatable<State>
        {
            int _distanceToAim;
            public int DistanceToAim{ get => _distanceToAim; }
            Point _locationOfObject;
            public Point Location { get => _locationOfObject; }
            public State(int distanceToAim, Point locationOfObject)
            {
                _distanceToAim = distanceToAim;
                _locationOfObject = locationOfObject;
            }

            public bool Equals(State other)
            {
                if (DistanceToAim == other.DistanceToAim && _locationOfObject == other.Location) return true;
                else return false;
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        InGraphics.Drawing _drawing;
        SimpleGMap<GRectangle> _map;
        SimpleGElement<GRectangle> _character;
        SimpleGElement<GRectangle> _aim;
        Point locationOfCharacter;
        Point locationOfAim;
        SystemNetwork.Networks.Network _network;
        SystemNetwork.Networks.LearningMethods.QLearning<State, Action> qLearning;
        int _degreesOfDirection;
        private double _speed;
        Thread _drawingThread;
        private void GraphicsForm_Shown(object sender, EventArgs e)
        {
            locationOfCharacter.X = 100;
            locationOfCharacter.Y = 50;
            locationOfAim.X = 400;
            locationOfAim.Y = 50;
            _speed = 1;
            _map = new SimpleGMap<GRectangle>(100, 100, 15);
            _map.SetDegreeParameters();
            _character = new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(locationOfCharacter.X, locationOfCharacter.Y, 30, 30), Color.Blue));
            _aim = new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(locationOfAim.X, locationOfAim.Y, 30, 30), Color.Green));
            _aim.SetDegreeParameters();
            _aim.FixPosition();
            for (int step = 0; step < 99; step++)
            {
                if(step < 8) _map.SetBlock(step, 0, new GRectangle(new Rectangle(), Color.Brown));
                if (step < 60) _map.SetBlock(0, step, new GRectangle(new Rectangle(), Color.Brown));
                if (step < 50 && step > 40) _map.SetBlock(8, step, new GRectangle(new Rectangle(), Color.Brown));
                if (step < 30) _map.SetBlock(8, step, new GRectangle(new Rectangle(), Color.Brown));
                if (step > 8 && step < 20)
                {
                    _map.SetBlock(step, 29, new GRectangle(new Rectangle(), Color.Brown));
                    _map.SetBlock(step, 40, new GRectangle(new Rectangle(), Color.Brown));
                }
                if (step > 28 && step < 42) _map.SetBlock(20, step, new GRectangle(new Rectangle(), Color.Brown));
                if (step > 8 && step < 40) _map.SetBlock(step, 50, new GRectangle(new Rectangle(), Color.Brown));
                if(step < 32)_map.SetBlock(step, 60, new GRectangle(new Rectangle(), Color.Brown));
                _map.SetBlock(39, step, new GRectangle(new Rectangle(), Color.Brown));
            }


            _drawing = new InGraphics.Drawing().SetBuffer(panelForDrawing, Color.White);
            _drawing.OnDraw += _map.FillOn;
            _drawing.OnDraw += _character.FillOn;
            _drawing.OnDraw += _aim.FillOn;

            
            SystemNetwork.Neurons.InputNeuron[] inputNeurons = new SystemNetwork.Neurons.InputNeuron[6];
            for (int step = 0; step < inputNeurons.Length; step++) inputNeurons[step] = new SystemNetwork.Neurons.InputNeuron();

            SystemNetwork.Neurons.HiddenNeuron[] averageNeurons = new SystemNetwork.Neurons.HiddenNeuron[16];
            for (int step = 0; step < averageNeurons.Length; step++) averageNeurons[step] = new SystemNetwork.Neurons.HiddenNeuron();

            SystemNetwork.Neurons.HiddenNeuron[] averageNeurons1 = new SystemNetwork.Neurons.HiddenNeuron[16];
            for (int step = 0; step < averageNeurons1.Length; step++) averageNeurons1[step] = new SystemNetwork.Neurons.HiddenNeuron();

            SystemNetwork.Neurons.OutputNeuron[] outputNeurons = new SystemNetwork.Neurons.OutputNeuron[8];
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
            SystemNetwork.Networks.LearningMethods.Learning learning = new SystemNetwork.Networks.LearningMethods.MOPLearning() { SpeedE = 0.2, MomentA = 0.3 };
            _network.SetTeaching(learning);

            label1.Text = "Generation: " + generaton;

            qLearning = new SystemNetwork.Networks.LearningMethods.QLearning<State, Action>(0.1, 0.8, 0.2);
            qLearning.SetAndUpdate(new State(FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y)), new Point(_map.X, _map.Y)), actions); //distance - расстояние от объекта до конечной цели, Location - позиция самой карты
            timer1.Enabled = true;
            _drawingThread = new Thread(new ThreadStart(() => { while (true) { _drawing.Draw(); _drawing.DisposeBuffer(); _drawing.RefreshBuffer(); } }));
            _drawingThread.Start();
        }
        private int FindDistance(Point firstLocation, Point secondLocation)
        {
            return Convert.ToInt32(Math.Sqrt(Math.Pow(secondLocation.X - firstLocation.X, 2) + Math.Pow(secondLocation.Y - firstLocation.Y, 2)));
        }
        Action[] actions = { new Action(90, InGraphics.Moving.Direction.downright, 0), new Action(0, InGraphics.Moving.Direction.topright, 1), new Action(90, InGraphics.Moving.Direction.topleft, 2), new Action(0, InGraphics.Moving.Direction.downleft, 3), new Action(45, InGraphics.Moving.Direction.topleft, 4), new Action(45, InGraphics.Moving.Direction.downleft, 5), new Action(45, InGraphics.Moving.Direction.downright, 6), new Action(45, InGraphics.Moving.Direction.topright, 7)};
        private int generaton = 0;
        private int radiusOfAreaMap = 200;
        //ДЛЯ НАЧАЛА НЕОБХОДИМО ОБУЧИТЬ Q-LEARNING, А ПОТОМ Q-LEARNING БУДЕТ ОБУЧАТЬ НЕЙРОННУЮ СЕТЬ
        //ПЕРЕДЕЛАТЬ QLearning, ПОСТОЯННО ДОБАВЛЯЕТ НОВЫЙ ЭЛЕМЕНТ
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Я ДУМАЮ ЛУЧШЕ ВСЕГО ОБУЧАТЬ ДЕЛАТЬ ХОДЫ ПО Q-LEARNING, А СЕТЬ ОБУЧАТЬ ПО ТЕКУЩИМ ДАННЫМ
            _map.MoveByDegrees();
            _aim.MoveByDegrees();
            bool[,] map = _map.getAreaMap(new Rectangle(locationOfCharacter.X - radiusOfAreaMap, locationOfCharacter.Y - radiusOfAreaMap, radiusOfAreaMap * 2, radiusOfAreaMap * 2));
            Point location = new Point(radiusOfAreaMap, radiusOfAreaMap);
            int topDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.top);
            int downDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.down);
            int leftDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.left);
            int rightDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.right);
            int distanceToAim = FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y));
            double[] results = _network.Start(distanceToAim / 1000, _degreesOfDirection / 360, (double)topDistance / radiusOfAreaMap, (double)downDistance / radiusOfAreaMap, (double)leftDistance / radiusOfAreaMap, (double)rightDistance / radiusOfAreaMap);
            //Как нейронная сеть может понять куда ей двигаться если она смотрит только по сторонам
            int indexOfMax(double[] results) { int index = 0; for (int step = 0; step < results.Length; step++) if (results[step] > results[index]) index = step; return index; }
            int indexOfMaxResult = indexOfMax(results);
            _map.ChangeDirection(actions[indexOfMaxResult].Degrees, actions[indexOfMaxResult].Direction, _speed);
            _aim.ChangeDirection(actions[indexOfMaxResult].Degrees, actions[indexOfMaxResult].Direction, _speed);
            _degreesOfDirection = (int)actions[indexOfMaxResult].Direction * 90 + actions[indexOfMaxResult].Degrees;
            //MessageBox.Show("top " + topDistance.ToString() + " down " + downDistance.ToString() + " left " + leftDistance.ToString() + " right " + rightDistance.ToString());

            //string str = ""; foreach (double d in results) str += d.ToString() + " "; MessageBox.Show(str);
            textBox1.Text = qLearning.CountStates.ToString();
            textBox2.Text = distanceToAim.ToString();

            if (new int[]{ topDistance, downDistance, leftDistance, rightDistance }.Min() < 15)
            {
                Action outAction = qLearning.Step(new State(distanceToAim, new Point(_map.X, _map.Y)), actions, -15);
                results[outAction.NumberOfArray] = 1;
                for (int step = 0; step < results.Length; step++) if (step != outAction.NumberOfArray) results[step] = 0;
                _network.TeachNetwork(results);
                _map.ReturnToStart();
                _aim.ReturnToStart();
                qLearning.SetAndUpdate(new State(distanceToAim, new Point(_map.X, _map.Y)), actions);
                label1.Text = "Generation: " + ++generaton;
                textBox3.Text = outAction.Degrees.ToString() + " - " + outAction.Direction.ToString();
            }
            else
            {
                Action outAction = qLearning.Step(new State(distanceToAim, new Point(_map.X, _map.Y)), actions, int.MaxValue - distanceToAim * 5);
                results[outAction.NumberOfArray] = 1;
                _network.TeachNetwork(results);
                textBox3.Text = outAction.Degrees.ToString() + " - " + outAction.Direction.ToString();
            }
            _network.DisposeNeurons();
        }
        public int FindObstacle(bool[,] map, Point position, int length, InGraphics.Moving.MoveTo direction)
        {
            int xSum = 0, ySum = 0;
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
                    if (map[yBuffer + (step * Math.Abs(xSum)), xBuffer + (step * Math.Abs(ySum))]) return FindDistance(position, new Point(xBuffer + (step * Math.Abs(ySum)), yBuffer + (step * Math.Abs(xSum))));
                }
            }
            return int.MaxValue;
        }




        private void button1_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.left);
            _aim.MoveOn(5, InGraphics.Moving.MoveTo.left);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.right);
            _aim.MoveOn(5, InGraphics.Moving.MoveTo.right);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.down);
            _aim.MoveOn(5, InGraphics.Moving.MoveTo.down);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _map.MoveOn(5, InGraphics.Moving.MoveTo.top);
            _aim.MoveOn(5, InGraphics.Moving.MoveTo.top);
        }



        private void button5_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled) timer1.Enabled = false;
            else timer1.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _map.ReturnToStart();
            _aim.ReturnToStart();
        }
    }
}
