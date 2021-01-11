using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Bonds
{
    public class DirectBond : Bond
    {
        public DirectBond(Neurons.Neuron inputNeuron, Neurons.Neuron outPutBeuron, double weigh) : base(inputNeuron, outPutBeuron, weigh)
        {
        }
    }
}
