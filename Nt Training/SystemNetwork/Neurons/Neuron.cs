﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Neurons
{
    public abstract class Neuron
    {
        public double Value { get; protected set; }
        public void ClearValue() => Value = 0;
    }
}
