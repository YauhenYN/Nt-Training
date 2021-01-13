using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Neurons
{
    public abstract class Neuron
    {
        private double _value;
        public double Value { get { return _value; } protected set { _value = value; } }
        public void ClearValue() => Value = 0;
    }
}
