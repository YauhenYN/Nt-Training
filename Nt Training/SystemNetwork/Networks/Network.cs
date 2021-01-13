using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training.SystemNetwork.Networks
{
    public class Network
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
        public void AddInPutNeurons(params Neurons.InputNeuron[] inputNeurons) => _inputLayer.InputNeurons.AddRange(inputNeurons);
        public void AddInPutNeurons(Layers.InputLayer inputLayer) => _inputLayer = inputLayer;
        public void AddAverageLayer(Layers.AverageLayer averageLayer) => _averageLayers.Add(averageLayer);
        public void AddOutPutNeurons(params Neurons.OutputNeuron[] outputNeurons) => _outputLayer.OutputNeurons.AddRange(outputNeurons);
        public void AddOutPutNeurons(Layers.OutputLayer outputLayer) => _outputLayer = outputLayer;
        public double[] Start(params double[] entranceData)
        {
            double[] resultsOfNetwork = new double[_outputLayer.OutputNeurons.Count];
            for(int step = 0; step < _inputLayer.InputNeurons.Count; step++)
            {
                _inputLayer.InputNeurons[step].InPut(entranceData[step]);
                for (int inStep = 0; inStep < _inputLayer.InputNeurons[step].OutPutBonds.Length; inStep++)
                {
                    _inputLayer.InputNeurons[step].OutPutBonds[inStep].Put(_inputLayer.InputNeurons[step].Value);
                }
            }
            for(int step = 0; step < _averageLayers.Count; step++)
            {
                for(int inStep = 0; inStep < _averageLayers[step].AverageNeurons.Count; inStep++)
                {
                    _averageLayers[step].AverageNeurons[inStep].AdderActivation();
                    double activatedFunctionResult = _averageLayers[step].AverageNeurons[inStep].ActivateFunction(_activationFunction.Activate);
                    for (int bondNumber = 0; bondNumber < _averageLayers[step].AverageNeurons[inStep].OutputBonds.Length; bondNumber++)
                    {
                        _averageLayers[step].AverageNeurons[inStep].OutputBonds[bondNumber].Put(activatedFunctionResult);
                    }
                    _averageLayers[step].AverageNeurons[inStep].ClearValue();
                }
            }
            for(int step = 0; step < _outputLayer.OutputNeurons.Count; step++)
            {
                _outputLayer.OutputNeurons[step].AdderActivation();
                resultsOfNetwork[step] = _outputLayer.OutputNeurons[step].ActivateFunction(_activationFunction.Activate);
            }
            return resultsOfNetwork;
        }
    }
}
