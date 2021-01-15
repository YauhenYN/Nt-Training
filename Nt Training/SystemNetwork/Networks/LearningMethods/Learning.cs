using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Networks.LearningMethods
{
    public abstract class Learning
    {
        public abstract double Learn(Network network, params double[] waitingResults);
        public abstract void SetLayers(Layers.InputLayer inputLayer, Layers.AverageLayer[] averageLayer, Layers.OutputLayer outputLayer);
    }
}
