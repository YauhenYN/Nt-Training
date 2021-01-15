using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Neurons
{
    public class InputNeuron : Neuron
    {
        public InputNeuron() =>  _outPutBonds = new List<Bonds.Bond>();
        List<Bonds.Bond> _outPutBonds;
        public Bonds.Bond[] OutPutBonds { get { return _outPutBonds.ToArray(); } }
        public void AddOutPutBond(Bonds.Bond bond)
        {
            if (!_outPutBonds.Contains(bond)) _outPutBonds.Add(bond);
        }
        public void RemoveOutputBond(Bonds.Bond bond) => _outPutBonds.Remove(bond);
        public void InPut(double InValue) { if(InValue >= 0 && InValue <= 1)Value = Convert.ToDouble(InValue); }
        //ДОБАВИТЬ В НЕЙРОНЫ АВТОМАТИЧЕСКУЮ ПЕРЕДАЧУ В СВЯЗИ
    }
}
