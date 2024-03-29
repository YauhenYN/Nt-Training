﻿using System;
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
using System.IO;
using System.Text.Json;
using System.Globalization;

namespace Nt_Training
{
    public partial class GraphicsForm : Form
    {
        public GraphicsForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            UpdateStyles();

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
            foreach (SystemNetwork.Neurons.InputNeuron inputNeuron in inputNeurons)
            {
                foreach (SystemNetwork.Neurons.HiddenNeuron averageNeuron in averageNeurons)
                {
                    bonds.Add(new SystemNetwork.Bonds.Bond(inputNeuron, averageNeuron, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                    inputNeuron.AddOutPutBond(bonds.Last());
                    averageNeuron.AddInputBond(bonds.Last());
                }
            }
            foreach (SystemNetwork.Neurons.HiddenNeuron averageNeuron in averageNeurons)
            {
                foreach (SystemNetwork.Neurons.HiddenNeuron averageNeuron1 in averageNeurons1)
                {
                    bonds.Add(new SystemNetwork.Bonds.Bond(averageNeuron, averageNeuron1, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                    averageNeuron.AddOutputBond(bonds.Last());
                    averageNeuron1.AddInputBond(bonds.Last());
                }
            }
            foreach (SystemNetwork.Neurons.HiddenNeuron averageNeuron1 in averageNeurons1)
            {
                foreach (SystemNetwork.Neurons.OutputNeuron outputNeuron in outputNeurons)
                {
                    bonds.Add(new SystemNetwork.Bonds.Bond(averageNeuron1, outputNeuron, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                    averageNeuron1.AddOutputBond(bonds.Last());
                    outputNeuron.AddInputBond(bonds.Last());
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
            SystemNetwork.Networks.LearningMethods.Learning learning = new SystemNetwork.Networks.LearningMethods.MOPLearning() { SpeedE = 0.15, MomentA = 0.05 };
            _network.SetTeaching(learning);
        }
        public GraphicsForm(string path) : this()
        {
            _path = path;
            TextReader tr = new StreamReader(path);
            string str = tr.ReadLine();
            tr.Close();
            Integration integrated = JsonSerializer.Deserialize<Integration>(str);
            qLearning = new SystemNetwork.Networks.LearningMethods.QLearning<State, Action>(integrated._alpha, integrated._eps, integrated._discount);
            State[] states = new State[integrated.locations.Length];
            for (int step = 0; step < states.Length; step++)
            {
                states[step] = new State(integrated.distancesToAim[step], integrated.locations[step]);
            }
            List<List<Action>> actions = new List<List<Action>>();
            for (int step = 0; step < integrated.q_values.Count; step++) actions.Add(new List<Action>(this.actions));
            qLearning.Integrate(states, actions, integrated.q_values);
            _network.Integration(integrated.weights);
            textBox4.Text = integrated._alpha.ToString(CultureInfo.InvariantCulture);
            textBox5.Text = integrated._eps.ToString(CultureInfo.InvariantCulture);
            textBox6.Text = integrated._discount.ToString(CultureInfo.InvariantCulture);
            isFirst = false;
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
            public double DistanceToAim { get; }
            public Point Location { get; }
            public State(double distanceToAim, Point locationOfObject)
            {
                DistanceToAim = distanceToAim;
                Location = locationOfObject;
            }

            public bool Equals(State other)
            {
                if (Location == other.Location) return true;
                else return false;
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _drawingThread.Abort();
            Application.Exit();
        }
        private InGraphics.Drawing _drawing;
        private SimpleGMap<GRectangle> _map;
        private SimpleGElement<GRectangle> _character;
        private SimpleGElement<GRectangle> _aim;
        private Point locationOfCharacter, locationOfAim;
        private SystemNetwork.Networks.Network _network;
        private SystemNetwork.Networks.LearningMethods.QLearning<State, Action> qLearning;
        private double _speed;
        private Thread _drawingThread;
        private void GraphicsForm_Shown(object sender, EventArgs e)
        {
            locationOfCharacter.X = 100;
            locationOfCharacter.Y = 50;
            locationOfAim.X = 400;
            locationOfAim.Y = 50;
            _speed = 1;
            _map = new SimpleGMap<GRectangle>(100, 100, 15);
            _character = new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(locationOfCharacter.X, locationOfCharacter.Y, 30, 30), Color.Blue));
            _aim = new SimpleGElement<GRectangle>(new GRectangle(new Rectangle(locationOfAim.X, locationOfAim.Y, 30, 30), Color.Green));
            _aim.FixPosition();
            for (int step = 0; step < 99; step++)
            {
                if (step < 8) _map.SetBlock(step, 0, new GRectangle(new Rectangle(), Color.Brown));
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
                if (step < 32) _map.SetBlock(step, 60, new GRectangle(new Rectangle(), Color.Brown));
                _map.SetBlock(39, step, new GRectangle(new Rectangle(), Color.Brown));
            }


            _drawing = new InGraphics.Drawing().SetBuffer(panelForDrawing, Color.White);
            _drawing.OnDraw += _map.FillOn;
            _drawing.OnDraw += _character.FillOn;
            _drawing.OnDraw += _aim.FillOn;

            label1.Text = "Generation: " + generaton;

            _drawingThread = new Thread(new ThreadStart(() => { while (true) { _drawing.Draw(); _drawing.DisposeBuffer(); _drawing.RefreshBuffer(); } }));
            _drawingThread.Start();
        }
        private async void TeachQLearningAsync()
        {
            await Task.Run(() => TeachQLearning());
        }
        bool isQTeaches;
        delegate void SetTextCallback(string firstText, string secondText);
        private void SetTextBoxes(string firstText, string secondText)
        {
            if (textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextBoxes);
                Invoke(d, new object[] { firstText, secondText });
            }
            else
            {
                textBox1.Text = firstText;
                textBox2.Text = secondText;
            }
        }
        bool isFirst = true;
        private void TeachQLearning()
        {
            isQTeaches = true;
            if (isFirst)
            {
                qLearning = new SystemNetwork.Networks.LearningMethods.QLearning<State, Action>
                    (
                    Convert.ToDouble(textBox4.Text, CultureInfo.InvariantCulture),
                    Convert.ToDouble(textBox5.Text.Replace(",", "."), CultureInfo.InvariantCulture),
                    Convert.ToDouble(textBox6.Text.Replace(",", "."), CultureInfo.InvariantCulture)
                    );
                isFirst = false;
            }
            qLearning.SetAndUpdate(new State(FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y)), new Point(_map.X, _map.Y)), actions);
            for (int step = 0; isQTeaches; step++)
            {
                for (double distanceToAim = FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y)); distanceToAim > 5 && isQTeaches; distanceToAim = FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y)))
                {
                    _map.MoveByDegrees();
                    _aim.MoveByDegrees();
                    bool[,] map = _map.GetAreaMap(new Rectangle(locationOfCharacter.X - radiusOfAreaMap, locationOfCharacter.Y - radiusOfAreaMap, radiusOfAreaMap * 2, radiusOfAreaMap * 2));
                    Point location = new Point(radiusOfAreaMap, radiusOfAreaMap);
                    int topDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.top);
                    int downDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.down);
                    int leftDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.left);
                    int rightDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.right);
                    if (new int[] { topDistance, downDistance, leftDistance, rightDistance }.Min() < 15)
                    {
                        qLearning.Step(new State(distanceToAim, new Point(_map.X, _map.Y)), actions, double.MinValue);
                        _map.ReturnToStart();
                        _aim.ReturnToStart();
                        qLearning.SetAndUpdate(new State(distanceToAim, new Point(_map.X, _map.Y)), actions);
                        SetTextBoxes(qLearning.CountStates.ToString(), distanceToAim.ToString());
                    }
                    else
                    {
                        Action outAction = qLearning.Step(new State(distanceToAim, new Point(_map.X, _map.Y)), actions, 0 - distanceToAim);
                        _map.ChangeDirection(outAction.Degrees, outAction.Direction, _speed);
                        _aim.ChangeDirection(outAction.Degrees, outAction.Direction, _speed);
                    }
                }
                _map.ReturnToStart();
                _aim.ReturnToStart();
            }
            isQTeaches = false;
        }
        private Action[] actions = { new Action(90, InGraphics.Moving.Direction.downright, 0), new Action(0, InGraphics.Moving.Direction.topright, 1), new Action(90, InGraphics.Moving.Direction.topleft, 2), new Action(0, InGraphics.Moving.Direction.downleft, 3), new Action(45, InGraphics.Moving.Direction.topleft, 4), new Action(45, InGraphics.Moving.Direction.downleft, 5), new Action(45, InGraphics.Moving.Direction.downright, 6), new Action(45, InGraphics.Moving.Direction.topright, 7) };
        private int generaton = 0;
        private int radiusOfAreaMap = 200;
        private bool isMOPTeached = true;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000 / Convert.ToInt32(textBox7.Text);
            int indexOfMax(double[] results)
            {
                int index = 0;
                for (int step = 0; step < results.Length; step++) if (results[step] > results[index]) index = step;
                return index;
            }
            bool[,] map = _map.GetAreaMap(new Rectangle(locationOfCharacter.X - radiusOfAreaMap, locationOfCharacter.Y - radiusOfAreaMap, radiusOfAreaMap * 2, radiusOfAreaMap * 2)); //правильно
            double distanceToAim = FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y));

            Point location = new Point(radiusOfAreaMap, radiusOfAreaMap);
            int topDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.top);
            int downDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.down);
            int leftDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.left);
            int rightDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.right);
            Action outAction = qLearning.Get_Policy(new State(distanceToAim, new Point(_map.X, _map.Y)));
            double[] results = _network.Start((double)distanceToAim / (142 * 15), ((double)(((int)outAction.Direction) * 90) + outAction.Degrees) / 360, topDistance == int.MaxValue ? 1 : (double)topDistance / radiusOfAreaMap, downDistance == int.MaxValue ? 1 : (double)downDistance / radiusOfAreaMap, leftDistance == int.MaxValue ? 1 : (double)leftDistance / radiusOfAreaMap, rightDistance == int.MaxValue ? 1 : (double)rightDistance / radiusOfAreaMap);
            int indexOfMaxResult = indexOfMax(results);
            if (outAction.NumberOfArray == indexOfMaxResult)
            {
                if (distanceToAim < 7)
                {
                    if (isMOPTeached) timer1.Enabled = false;
                    _map.ReturnToStart();
                    _aim.ReturnToStart();
                    label1.Text = "Generation: " + ++generaton;
                }
                _map.ChangeDirection(actions[indexOfMaxResult].Degrees, actions[indexOfMaxResult].Direction, _speed);
                _aim.ChangeDirection(actions[indexOfMaxResult].Degrees, actions[indexOfMaxResult].Direction, _speed);
                _map.MoveByDegrees();
                _aim.MoveByDegrees();
            }
            else
            {
                isMOPTeached = false;
                double[] newResults = (double[])results.Clone();
                newResults[outAction.NumberOfArray] = 1;
                for (int step = 0; step < results.Length; step++) if (step != outAction.NumberOfArray) newResults[step] = 0;
                _network.TeachNetwork(newResults);
            }

            textBox1.Text = qLearning.CountStates.ToString();
            textBox2.Text = distanceToAim.ToString();
            textBox3.Text = outAction.Degrees.ToString() + " - " + outAction.Direction.ToString();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Interval = 1000 / Convert.ToInt32(textBox7.Text);
            int indexOfMax(double[] results)
            {
                int index = 0;
                for (int step = 0; step < results.Length; step++) if (results[step] > results[index]) index = step;
                return index;
            }
            bool[,] map = _map.GetAreaMap(new Rectangle(locationOfCharacter.X - radiusOfAreaMap, locationOfCharacter.Y - radiusOfAreaMap, radiusOfAreaMap * 2, radiusOfAreaMap * 2)); //правильно
            double distanceToAim = FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y));

            Point location = new Point(radiusOfAreaMap, radiusOfAreaMap);
            int topDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.top);
            int downDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.down);
            int leftDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.left);
            int rightDistance = FindObstacle(map, location, 30, InGraphics.Moving.MoveTo.right);
            Action outAction = qLearning.Get_Policy(new State(distanceToAim, new Point(_map.X, _map.Y)));
            if (distanceToAim < 7 || new double[] { topDistance, downDistance, leftDistance, rightDistance }.Min() < 3 || (outAction.NumberOfArray == 0 && outAction.Degrees == 0))
            {
                _map.ReturnToStart();
                _aim.ReturnToStart();
                label1.Text = "Generation: " + ++generaton;
            }
            double[] results = _network.Start((double)distanceToAim / (142 * 15), ((double)(((int)outAction.Direction) * 90) + outAction.Degrees) / 360, topDistance == int.MaxValue ? 1 : (double)topDistance / radiusOfAreaMap, downDistance == int.MaxValue ? 1 : (double)downDistance / radiusOfAreaMap, leftDistance == int.MaxValue ? 1 : (double)leftDistance / radiusOfAreaMap, rightDistance == int.MaxValue ? 1 : (double)rightDistance / radiusOfAreaMap);
            int indexOfMaxResult = indexOfMax(results);
            int commonDegrees = (((int)actions[indexOfMaxResult].Direction % 2 == 0 ? -1 : 1) * actions[indexOfMaxResult].Degrees + ((((int)actions[indexOfMaxResult].Direction / 2) + 1) * 180)) % 360;
            double speed = _speed;
            if (isPhysics)
            {
                Physics.Wind(ref commonDegrees, ref speed, 0.3, 270);
            }
            int direction = (commonDegrees / 90) - 1;
            int degrees;
            degrees = Math.Abs((((direction / 2) + 1) * 180) - -((direction % 2 == 0 ? -1 : 1) * commonDegrees)) % 360;
            _map.ChangeDirection(degrees, (InGraphics.Moving.Direction)direction, speed);
            _aim.ChangeDirection(degrees, (InGraphics.Moving.Direction)direction, speed); 
            _map.MoveByDegrees();
            _aim.MoveByDegrees();

            textBox1.Text = qLearning.CountStates.ToString();
            textBox2.Text = distanceToAim.ToString();
            textBox3.Text = outAction.Degrees.ToString() + " - " + outAction.Direction.ToString();
        }
        private double FindDistance(Point firstLocation, Point secondLocation)
        {
            return Math.Sqrt(Math.Pow(secondLocation.X - firstLocation.X, 2) + Math.Pow(secondLocation.Y - firstLocation.Y, 2));
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
                    if (map[yBuffer + (step * Math.Abs(xSum)), xBuffer + (step * Math.Abs(ySum))]) return Convert.ToInt32(FindDistance(position, new Point(xBuffer + (step * Math.Abs(ySum)), yBuffer + (step * Math.Abs(xSum)))));
                }
            }
            return int.MaxValue;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0 && timer1.Enabled == false)
            {
                if (!isQTeaches) { TeachQLearningAsync(); isQTeaches = true; }
                else if (isQTeaches) { isQTeaches = false; }
            }
            else if (comboBox1.SelectedIndex == 1 && !isQTeaches && qLearning != null && !timer2.Enabled)
            {
                if (timer1.Enabled == false)
                {
                    qLearning.SetAndUpdate(new State(FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y)), new Point(_map.X, _map.Y)), actions); timer1.Enabled = true;
                }
                else
                {
                    timer1.Enabled = false;
                    _map.ReturnToStart();
                    _aim.ReturnToStart();
                }
            }
            else if(comboBox1.SelectedIndex == 2)
            {
                if (!timer1.Enabled)
                {
                    if (timer2.Enabled == false)
                    {
                        qLearning.SetAndUpdate(new State(FindDistance(locationOfCharacter, new Point(_aim.X, _aim.Y)), new Point(_map.X, _map.Y)), actions); timer2.Enabled = true;
                    }
                    else
                    {
                        timer2.Enabled = false;
                        _map.ReturnToStart();
                        _aim.ReturnToStart();
                    }
                }
            }
        }
        bool isPhysics = false;
        private void button2_Click(object sender, EventArgs e)
        {
            if (isPhysics) isPhysics = false;
            else isPhysics = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_path != null && _path.Length > 0)
            {
                SystemNetwork.Networks.LearningMethods.QLearning<State, Action>.Integration integration = qLearning.GetIntegration();
                Integration inIntegration = new Integration();
                inIntegration.q_values = integration.q_values;
                inIntegration._alpha = integration._alpha;
                inIntegration._discount = integration._discount;
                inIntegration._eps = integration._eps;
                inIntegration.locations = new Point[integration.outStates.Length];
                inIntegration.distancesToAim = new double[integration.outStates.Length];
                inIntegration.weights = _network.GetWeights();
                for (int step = 0; step < integration.outStates.Length; step++)
                {
                    inIntegration.locations[step] = integration.outStates[step].Location;
                    inIntegration.distancesToAim[step] = integration.outStates[step].DistanceToAim;
                }
                string wPage = JsonSerializer.Serialize(inIntegration);
                File.WriteAllText(_path, wPage);
            }
        }
        public class Integration
        {
            public Point[] locations { get; set; }
            public List<List<double>> q_values { get; set; }
            public double[][] weights { get; set; }
            public double[] distancesToAim { get; set; }
            public double _alpha { get; set; }
            public double _eps { get; set; }
            public double _discount { get; set; }
        }
        string _path;
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileDialog fileDialog = new SaveFileDialog();
            fileDialog.DefaultExt = "nt";
            fileDialog.Filter = "(*.nt)|*.nt";
            fileDialog.ShowDialog();
            if (fileDialog.FileName.Length > 0 && qLearning != null)
            {
                SystemNetwork.Networks.LearningMethods.QLearning<State, Action>.Integration integration = qLearning.GetIntegration();

                Integration inIntegration = new Integration();
                inIntegration.q_values = integration.q_values;
                inIntegration._alpha = integration._alpha;
                inIntegration._discount = integration._discount;
                inIntegration._eps = integration._eps;
                inIntegration.weights = _network.GetWeights();
                inIntegration.locations = new Point[integration.outStates.Length];
                inIntegration.distancesToAim = new double[integration.outStates.Length];
                for(int step = 0; step < integration.outStates.Length; step++)
                {
                    inIntegration.locations[step] = integration.outStates[step].Location;
                    inIntegration.distancesToAim[step] = integration.outStates[step].DistanceToAim;
                }
                string wPage = JsonSerializer.Serialize<Integration>(inIntegration);
                File.WriteAllText(fileDialog.FileName, wPage);
                _path = fileDialog.FileName;
            }
        }

        private void GraphicsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 47 || e.KeyChar > 58) && (e.KeyChar != 08 || ((TextBox)sender).Text.Length < 3))
            {
                e.Handled = true;
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 47 || e.KeyChar > 58) && e.KeyChar != 08)
            {
                e.Handled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != 0)
            {
                _map.ReturnToStart();
                _aim.ReturnToStart();
            }
        }
    }
}
