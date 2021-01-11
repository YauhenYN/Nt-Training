using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Neurons
{
    public class OutputNeuron : Neuron
    {
        public List<Bonds.Bond> InPutBonds;
        public void AdderActivation()
        {
            foreach (Bonds.Bond bond in InPutBonds)
            {
                _value += bond.PuttingResult;
            }
        }
        public double ActivateFunction(Func<double, double> activation)
        {
            return activation(_value);
        }
    }
}
