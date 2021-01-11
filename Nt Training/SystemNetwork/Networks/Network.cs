using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Networks
{
    public abstract class Network
    {
        Layers.InputLayer _inputLayer;
        List<Layers.AverageLayer> _averageLayers;
        Layers.OutputLayer outputLayer;
    }
}
