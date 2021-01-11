﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Bonds
{
    public abstract class Bond
    {
        public Neurons.Neuron InPutNeuron { get; }
        public Neurons.Neuron OutPutNeuron { get; }
        private double _weigh;
        public double Weigh { get { return _weigh; } set { if (value >= 0 && value <= 1) _weigh = value; } }
        public Bond(Neurons.Neuron inputNeuron, Neurons.Neuron outPutBeuron, double weigh)
        {
            InPutNeuron = inputNeuron;
            OutPutNeuron = outPutBeuron;
            _weigh = weigh;
        }
        public double PuttingResult { get; private set; }
        public void Put(double inPutValue)
        {
            PuttingResult = inPutValue * Weigh;
        }
        public void ClearPuttingResult() => PuttingResult = 0;
    }
}
