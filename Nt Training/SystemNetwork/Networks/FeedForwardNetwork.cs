using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Networks
{
    public class FeedForwardNetwork : Network
    {
        public FeedForwardNetwork(Neurons.ActivationFunctions.ActivationFuncton function) : base(function)
        {
        }
    }
}
