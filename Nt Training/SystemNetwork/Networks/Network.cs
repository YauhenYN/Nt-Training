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
        private Layers.InputLayer _inputLayer;
        private List<Layers.HiddenLayer> _averageLayers;
        private Layers.OutputLayer _outputLayer;
        private Neurons.ActivationFunctions.ActivationFuncton _activationFunction;
        public double[] currentResults { get; private set; }
        public Network(Neurons.ActivationFunctions.ActivationFuncton function)
        {
            _activationFunction = function;
            _inputLayer = new Layers.InputLayer();
            _averageLayers = new List<Layers.HiddenLayer>();
            _outputLayer = new Layers.OutputLayer();
        }
        public void AddInPutNeurons(params Neurons.InputNeuron[] inputNeurons) => _inputLayer.InputNeurons.AddRange(inputNeurons);
        public void AddInPutNeurons(Layers.InputLayer inputLayer) => _inputLayer = inputLayer;
        public void AddAverageLayer(Layers.HiddenLayer averageLayer) => _averageLayers.Add(averageLayer);
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
                }
            }
            for(int step = 0; step < _outputLayer.OutputNeurons.Count; step++)
            {
                _outputLayer.OutputNeurons[step].AdderActivation();
                resultsOfNetwork[step] = _outputLayer.OutputNeurons[step].ActivateFunction(_activationFunction.Activate);
            }
            currentResults = resultsOfNetwork;
            return (double[])resultsOfNetwork.Clone();
        }
        public void DisposeNeurons()
        {
            foreach (Neurons.InputNeuron inputNeuron in _inputLayer.InputNeurons) inputNeuron.ClearValue();
            foreach (Layers.HiddenLayer averageLayer in _averageLayers) foreach (Neurons.HiddenNeuron averageNeuron in averageLayer.AverageNeurons) averageNeuron.ClearValue();
            foreach (Neurons.OutputNeuron outputNeuron in _outputLayer.OutputNeurons) outputNeuron.ClearValue(); 
        }
        private LearningMethods.Learning _learning;
        public void SetTeaching(LearningMethods.Learning learning)
        {
            _learning = learning;
            _learning.SetLayers(_inputLayer, _averageLayers.ToArray(), _outputLayer);
        }
        public double TeachNetwork(params double[] waitingResults) => _learning.Learn(this, waitingResults);
        public double[][] GetWeights()
        {
            double[][] weights = new double[2 + _averageLayers.Count - 1][];
            weights[0] = new double[_inputLayer.InputNeurons.Count * _averageLayers[0].AverageNeurons.Count];
            for(int neuronNumber = 0; neuronNumber < _inputLayer.InputNeurons.Count; neuronNumber++)
            {
                for(int bondNumber = 0; bondNumber < _inputLayer.InputNeurons[neuronNumber].OutPutBonds.Length; bondNumber++)
                {
                    weights[0][(neuronNumber * _inputLayer.InputNeurons[neuronNumber].OutPutBonds.Length) + bondNumber] = _inputLayer.InputNeurons[neuronNumber].OutPutBonds[bondNumber].Weigh;
                }
            }
            for(int step = 0; step < _averageLayers.Count-1; step++)
            {
                weights[step + 1] = new double[_averageLayers[step].AverageNeurons.Count * _averageLayers[step+1].AverageNeurons.Count];
                for(int neuronNumber = 0; neuronNumber < _averageLayers[step].AverageNeurons.Count; neuronNumber++)
                {
                    for(int bondNumber = 0; bondNumber < _averageLayers[step].AverageNeurons[neuronNumber].OutputBonds.Length; bondNumber++)
                    {
                        weights[step + 1][(neuronNumber * _averageLayers[step].AverageNeurons[neuronNumber].OutputBonds.Length) + bondNumber] = _averageLayers[step].AverageNeurons[neuronNumber].OutputBonds[bondNumber].Weigh;
                    }
                }
            }
            weights[weights.Length - 1] = new double[_averageLayers[_averageLayers.Count - 1].AverageNeurons.Count * _outputLayer.OutputNeurons.Count];
            for (int neuronNumber = 0; neuronNumber < _averageLayers[_averageLayers.Count - 1].AverageNeurons.Count; neuronNumber++)
            {
                for (int bondNumber = 0; bondNumber < _averageLayers[_averageLayers.Count - 1].AverageNeurons[neuronNumber].OutputBonds.Length; bondNumber++)
                {
                    weights[weights.Length - 1][(neuronNumber * _averageLayers[_averageLayers.Count - 1].AverageNeurons[neuronNumber].OutputBonds.Length) + bondNumber] = _averageLayers[_averageLayers.Count - 1].AverageNeurons[neuronNumber].OutputBonds[bondNumber].Weigh;
                }
            }
            return weights;
        }
        public void Integration(double[][] weights)
        {
            for (int neuronNumber = 0; neuronNumber < _inputLayer.InputNeurons.Count; neuronNumber++)
            {
                for (int bondNumber = 0; bondNumber < _inputLayer.InputNeurons[neuronNumber].OutPutBonds.Length; bondNumber++)
                {
                    _inputLayer.InputNeurons[neuronNumber].OutPutBonds[bondNumber].Weigh = weights[0][(neuronNumber * _inputLayer.InputNeurons[neuronNumber].OutPutBonds.Length) + bondNumber];
                }
            }
            for (int step = 0; step < _averageLayers.Count - 1; step++)
            {
                for (int neuronNumber = 0; neuronNumber < _averageLayers[step].AverageNeurons.Count; neuronNumber++)
                {
                    for (int bondNumber = 0; bondNumber < _averageLayers[step].AverageNeurons[neuronNumber].OutputBonds.Length; bondNumber++)
                    {
                        _averageLayers[step].AverageNeurons[neuronNumber].OutputBonds[bondNumber].Weigh = weights[step + 1][(neuronNumber * _averageLayers[step].AverageNeurons[neuronNumber].OutputBonds.Length) + bondNumber];
                    }
                }
            }
            for (int neuronNumber = 0; neuronNumber < _averageLayers[_averageLayers.Count-1].AverageNeurons.Count; neuronNumber++)
            {
                for (int bondNumber = 0; bondNumber < _averageLayers[_averageLayers.Count - 1].AverageNeurons[neuronNumber].OutputBonds.Length; bondNumber++)
                {
                    _averageLayers[_averageLayers.Count - 1].AverageNeurons[neuronNumber].OutputBonds[bondNumber].Weigh = weights[weights.Length - 1][(neuronNumber * _averageLayers[_averageLayers.Count - 1].AverageNeurons[neuronNumber].OutputBonds.Length) + bondNumber];
                }
            }
        }
    }
}
