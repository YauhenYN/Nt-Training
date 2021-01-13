using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Layers
{
    public class InputLayer
    {
        public List<Neurons.InputNeuron> InputNeurons { get; private set; }
        public InputLayer() => InputNeurons = new List<Neurons.InputNeuron>();
        public InputLayer(params Neurons.InputNeuron[] inputNeurons) : this() => InputNeurons.AddRange(inputNeurons);
    }
}
