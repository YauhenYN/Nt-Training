using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training.SystemNetwork.Bonds
{
    public class Bond
    {
        public Neurons.Neuron InputNeuron { get; }
        public Neurons.Neuron OutputNeuron { get; }
        private double _weigh;
        public double Weigh { get { return _weigh; } set { _weigh = value; } }
        public Bond(Neurons.Neuron inputNeuron, Neurons.Neuron outputNeuron, double weigh)
        {
            InputNeuron = inputNeuron;
            OutputNeuron = outputNeuron;
            _weigh = weigh;
        }
        public double PuttingResult { get; private set; }
        public void Put(double inPutValue) => PuttingResult = inPutValue * Weigh;
    }
}
