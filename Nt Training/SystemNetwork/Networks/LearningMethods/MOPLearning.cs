using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training.SystemNetwork.Networks.LearningMethods
{
    public class MOPLearning : Learning
    {
        public double SpeedE { get; set; }
        public double MomentA { get; set; }
        public double ErrorCount { get; private set; }
        class DeltaNeuron
        {
            public double delta { get; set; }
            public Neurons.Neuron neuron { get; }
            public DeltaBond[] outputBonds { get; }
            public DeltaNeuron(double delta, Neurons.Neuron neuron)
            {
                this.delta = delta;
                this.neuron = neuron;
            }
            public DeltaNeuron(double delta, Neurons.Neuron neuron, Bonds.Bond[] outputBonds) :this(delta, neuron)
            {
                this.outputBonds = new DeltaBond[outputBonds.Length];
                for (int step = 0; step < outputBonds.Length; step++) this.outputBonds[step] = new DeltaBond(0, outputBonds[step]);
            }
        }
        class DeltaBond
        {
            public Bonds.Bond bond { get; }
            public double lastWeigh { get; set; }
            public DeltaBond(double lastWeigh, Bonds.Bond bond)
            {
                this.lastWeigh = lastWeigh;
                this.bond = bond;
            }
        }
        private double[] _waitingResults;
        private List<List<DeltaNeuron>> _deltaNeurons;
        public override void SetLayers(Layers.InputLayer inputLayer, Layers.HiddenLayer[] averageLayers, Layers.OutputLayer outputLayer)
        {
            _deltaNeurons = new List<List<DeltaNeuron>>();
            _deltaNeurons.Add(new List<DeltaNeuron>());
            for (int step = 0; step < inputLayer.InputNeurons.Count; step++) _deltaNeurons[0].Add(new DeltaNeuron(0, inputLayer.InputNeurons[step], inputLayer.InputNeurons[step].OutPutBonds));
            for(int step = 0; step < averageLayers.Length; step++)
            {
                _deltaNeurons.Add(new List<DeltaNeuron>());
                for(int inStep = 0; inStep < averageLayers[step].AverageNeurons.Count; inStep++) _deltaNeurons[step + 1].Add(new DeltaNeuron(0, averageLayers[step].AverageNeurons[inStep], averageLayers[step].AverageNeurons[inStep].OutputBonds));
            }
            int lastNumber = _deltaNeurons.Count;
            _deltaNeurons.Add(new List<DeltaNeuron>());
            for (int step = 0; step < outputLayer.OutputNeurons.Count; step++) _deltaNeurons[lastNumber].Add(new DeltaNeuron(0, outputLayer.OutputNeurons[step]));
        }
        public override double Learn(Network network, params double[] waitingResults)
        {
            _waitingResults = waitingResults;
            double errorCount = MSEError(network.currentResults);
            ErrorCount = errorCount;
            for(int step = 0; step < _deltaNeurons[_deltaNeurons.Count-1].Count; step++) _deltaNeurons[_deltaNeurons.Count-1][step].delta = (_waitingResults[step] - network.currentResults[step]) * Fsigmoid(network.currentResults[step]);
            for (int step = _deltaNeurons.Count-2; step > 0; step--)
            {
                for (int inStep = 0; inStep < _deltaNeurons[step].Count; inStep++)
                {
                    _deltaNeurons[step][inStep].delta = Fsigmoid(_deltaNeurons[step][inStep].neuron.Value) * SumAverage(_deltaNeurons[step][inStep]);
                    for(int lStep = 0; lStep < _deltaNeurons[step][inStep].outputBonds.Length; lStep++)
                    {
                        DeltaBond bond = _deltaNeurons[step][inStep].outputBonds[lStep];
                        double GRAD = _deltaNeurons[step][inStep].neuron.Value * FindInLists(_deltaNeurons, bond.bond.OutputNeuron).delta;
                        double inter = SpeedE * GRAD + MomentA * bond.lastWeigh;
                        bond.bond.Weigh += inter;
                        bond.lastWeigh = inter;
                    }
                }
            }
            for(int step = 0; step < _deltaNeurons[0].Count; step++)
            {
                for(int inStep = 0; inStep < _deltaNeurons[0][step].outputBonds.Length; inStep++)
                {
                    DeltaBond bond = _deltaNeurons[0][step].outputBonds[inStep];
                    double GRAD = _deltaNeurons[0][step].neuron.Value * FindInLists(_deltaNeurons, bond.bond.OutputNeuron).delta;
                    double inter = SpeedE * GRAD + MomentA * bond.lastWeigh;
                    bond.bond.Weigh += inter;
                    bond.lastWeigh = inter;
                }
            }
            return errorCount;
        }
        private double SumAverage(DeltaNeuron neuron)
        {
            double sum = 0;
            for(int step = 0; step < neuron.outputBonds.Length; step++)
            {
                sum = neuron.outputBonds[step].bond.Weigh * FindInLists(_deltaNeurons, neuron.outputBonds[step].bond.OutputNeuron).delta;
            }
            return sum;
        }
        private static DeltaNeuron FindInLists(List<List<DeltaNeuron>> neurons, Neurons.Neuron neuron)
        {
            for(int step = 0; step < neurons.Count; step++)
            {
                for(int inStep = 0; inStep < neurons[step].Count; inStep++)
                {
                    if(neurons[step][inStep].neuron == neuron) return neurons[step][inStep];
                }
            }
            return null;
        }
        private double Fsigmoid(double outValue) => (1 - outValue) * outValue;
        private double Ftangh(double outValue) => 1 - Math.Pow(outValue, 2);
        private double MSEError(params double[] currentResults)
        {
            double sum = 0;
            for(int step = 0; step < currentResults.Length; step++)
            {
                sum += Math.Pow(_waitingResults[step] - currentResults[step], 2);
            }
            return sum / currentResults.Length;
        }
        private double RootMSEError(params double[] currentResults) => Math.Sqrt(MSEError(currentResults));
        private double ArctanError(params double[] currentResults)
        {
            double sum = 0;
            for(int step = 0; step < _waitingResults.Length; step++)
            {
                sum += Math.Pow(Math.Atan(_waitingResults[step] - currentResults[step]), 2);
            }
            return sum / _waitingResults.Length;
        }
    }
}
