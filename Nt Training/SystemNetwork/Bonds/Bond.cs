using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Bonds
{
    public abstract class Bond
    {
        public Bond(double weigh) => _weigh = weigh;
        public Neurons.Neuron InPutNeuron; //ИНКАПСУЛИРОВАТЬ
        public Neurons.Neuron OutPutNeuron;
        private double _weigh;
        public double PuttingResult { get; private set; }
        public double Weigh { get { return _weigh; } set { if (value >= 0 && value <= 1) _weigh = value; } }
        public void Put(double inPutValue)
        {
            PuttingResult = inPutValue * Weigh;
        }
        public void ClearPuttingResult() => PuttingResult = 0;
    }
}
