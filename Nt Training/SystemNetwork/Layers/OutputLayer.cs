using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Layers
{
    public class OutputLayer : Layer
    {
        public List<Neurons.OutputNeuron> OutputNeurons { get; private set; }
        public OutputLayer() => OutputNeurons = new List<Neurons.OutputNeuron>();
        public OutputLayer(params Neurons.OutputNeuron[] outputNeurons) : this() => OutputNeurons.AddRange(outputNeurons);
    }
}
