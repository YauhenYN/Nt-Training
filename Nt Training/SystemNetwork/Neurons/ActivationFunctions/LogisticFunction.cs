using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Neurons.ActivationFunctions
{
    public class LogisticFunction : ActivationFuncton
    {
        int _a;
        public LogisticFunction(int a) => _a = a;
        public override double Activate(double inPutValue)
        {
            return 1 / (1 + Math.E * (-_a * inPutValue));
        }
    }
}
