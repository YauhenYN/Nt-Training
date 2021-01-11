using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Networks
{
    public abstract class Network
    {
        Layers.InputLayer _inputLayer;
        List<Layers.AverageLayer> _averageLayers;
        Layers.OutputLayer _outputLayer;
        Neurons.ActivationFunctions.ActivationFuncton _activationFunction;
        public Network(Neurons.ActivationFunctions.ActivationFuncton function)
        {
            _activationFunction = function;
            _inputLayer = new Layers.InputLayer();
            _averageLayers = new List<Layers.AverageLayer>();
            _outputLayer = new Layers.OutputLayer();
        }
        public void AddInPutNeurons(params Neurons.InputNeuron[] inputNeurons) => _inputLayer.inputNeurons.AddRange(inputNeurons);
        public void AddInPutNeurons(Layers.InputLayer inputLayer) => _inputLayer = inputLayer;
        public void AddAverageLayer(Layers.AverageLayer averageLayer) => _averageLayers.Add(averageLayer);
        public void AddOutPutNeurons(params Neurons.OutputNeuron[] outputNeurons) => _outputLayer.OutPutNeurons.AddRange(outputNeurons);
        public void AddOutPutNeurons(Layers.OutputLayer outputLayer) => _outputLayer = outputLayer;
        /*public double[] Start(params bool[] entranceData)
        {
            for(int step = 0; step < _inputLayer.inputNeurons.Count; step++)
            {
                _inputLayer.inputNeurons[step].InPut(entranceData[step]);
            }
        }*/
    }
}
