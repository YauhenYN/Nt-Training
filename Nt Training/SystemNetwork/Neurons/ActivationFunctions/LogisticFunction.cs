using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Neurons.ActivationFunctions
{
    public class LogisticFunction : ActivationFuncton
    {
        double _a;
        public LogisticFunction(double a) => _a = a;
        public override double Activate(double inPutValue) => 1 / (1 + Math.Exp(-_a * inPutValue)); 
    }
}
