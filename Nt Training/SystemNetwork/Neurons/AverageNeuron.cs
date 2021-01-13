using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training.SystemNetwork.Neurons
{
    public class AverageNeuron : Neuron
    {
        public AverageNeuron()
        {
            _outputBonds = new List<Bonds.Bond>();
            _inputBonds = new List<Bonds.Bond>();
        }
        List<Bonds.Bond> _inputBonds;
        public Bonds.Bond[] InputBonds { get { return _inputBonds.ToArray(); } }
        public void AddInputBond(Bonds.Bond bond)
        {
            if (!_inputBonds.Contains(bond)) _inputBonds.Add(bond);
        }
        public void RemoveInputBond(Bonds.Bond bond) => _inputBonds.Remove(bond);

        List<Bonds.Bond> _outputBonds;
        public Bonds.Bond[] OutputBonds { get { return _outputBonds.ToArray(); } }
        public void AddOutputBond(Bonds.Bond bond)
        {
            if (!_outputBonds.Contains(bond)) _outputBonds.Add(bond);
        }
        public void RemoveOutputBond(Bonds.Bond bond) => _outputBonds.Remove(bond);
        public void AdderActivation()
        {
            foreach(Bonds.Bond bond in _inputBonds)
            {
                Value += bond.PuttingResult;
            }
        }
        public double ActivateFunction(Func<double, double> activation) => activation(Value);
    }
}
