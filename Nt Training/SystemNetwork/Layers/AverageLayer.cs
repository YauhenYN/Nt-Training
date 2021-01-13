﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Layers
{
    public class AverageLayer : Layer
    {
        public AverageLayer() => AverageNeurons = new List<Neurons.AverageNeuron>(); 
        public List<Neurons.AverageNeuron> AverageNeurons { get; private set; }
        public AverageLayer(params Neurons.AverageNeuron[] averageNeurons) : this() => AverageNeurons.AddRange(averageNeurons);
    }
}
