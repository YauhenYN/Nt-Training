using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Neurons
{
    public class InputNeuron : Neuron
    {
        public List<Bonds.Bond> outPutBonds { get; }
        public void InPut(bool InValue) => _value = Convert.ToDouble(InValue);
    }
}
