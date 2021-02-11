using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Layers
{
    public class HiddenLayer : Layer
    {
        public HiddenLayer() => AverageNeurons = new List<Neurons.HiddenNeuron>(); 
        public List<Neurons.HiddenNeuron> AverageNeurons { get; private set; }
        public HiddenLayer(params Neurons.HiddenNeuron[] averageNeurons) : this() => AverageNeurons.AddRange(averageNeurons);
    }
}
