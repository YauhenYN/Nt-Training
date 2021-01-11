using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Neurons
{
    public abstract class Neuron
    {
        protected double _value;
        public void ClearValue() => _value = 0;
    }
}
