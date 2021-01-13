using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training.SystemNetwork.Neurons
{
    public class OutputNeuron : Neuron
    {
        public OutputNeuron() => _inputBonds = new List<Bonds.Bond>();
        List<Bonds.Bond> _inputBonds;
        public Bonds.Bond[] InputBonds { get { return _inputBonds.ToArray(); } }
        public void AddInputBond(Bonds.Bond bond)
        {
            if (!_inputBonds.Contains(bond)) _inputBonds.Add(bond);
        }
        public void RemoveInputBond(Bonds.Bond bond) => _inputBonds.Remove(bond);
        public void AdderActivation()
        {
            foreach (Bonds.Bond bond in _inputBonds)
            {
                Value += bond.PuttingResult;
            }
        }
        public double ActivateFunction(Func<double, double> activation) => activation(Value);
    }
}
