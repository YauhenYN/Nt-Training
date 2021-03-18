using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Nt_Training.SystemNetwork.Networks.LearningMethods
{
    public class MOPLearning : Learning
    {
        public double SpeedE { get; set; }
        public double MomentA { get; set; }
        private double[] _waitingResults;
        Layers.InputLayer _inputLayer;
        double[,] inputChanging;
        Layers.HiddenLayer[] _hiddenLayers;
        double[][,] hiddenChanging;
        Layers.OutputLayer _outputLayer;
        public override void SetLayers(Layers.InputLayer inputLayer, Layers.HiddenLayer[] averageLayers, Layers.OutputLayer outputLayer)
        {
            _inputLayer = inputLayer;
            _hiddenLayers = averageLayers;
            _outputLayer = outputLayer;
            inputChanging = new double[_inputLayer.InputNeurons.Count, _hiddenLayers[0].AverageNeurons.Count];
            hiddenChanging = new double[_hiddenLayers.Length][,];
            hiddenChanging[_hiddenLayers.Length-1] = new double[_hiddenLayers[_hiddenLayers.Length-1].AverageNeurons.Count, _outputLayer.OutputNeurons.Count];
            for (int step = 0; step < _hiddenLayers.Length-1; step++)
            {
                hiddenChanging[step] = new double[_hiddenLayers[step].AverageNeurons.Count, _hiddenLayers[step+1].AverageNeurons.Count];
            }
        }
        public override double Learn(Network network, params double[] waitingResults)
        {
            _waitingResults = waitingResults;
            //1 - ДЕЛЬТА ДЛЯ КАЖДОГО СЛОЯ КРОМЕ ВХОДНОГО
            double[] delta_output = new double[_outputLayer.OutputNeurons.Count];
            //Выходной слой
            for(int step = 0; step < _outputLayer.OutputNeurons.Count; step++)
            {
                delta_output[step] = (waitingResults[step] - network.currentResults[step]) * Fsigmoid(network.currentResults[step]);
            }
            double[][] delta_hidden = new double[_hiddenLayers.Length][];
            for (int step = 0; step < _hiddenLayers.Length; step++) delta_hidden[step] = new double[_hiddenLayers[step].AverageNeurons.Count];
            //Последний скрытый слой
            for (int inStep = 0; inStep < _hiddenLayers[_hiddenLayers.Length - 1].AverageNeurons.Count; inStep++)
            {
                double Sum(Neurons.HiddenNeuron neuron)
                {
                    double sum = 0;
                    for (int step = 0; step < neuron.OutputBonds.Length; step++)
                    {
                        sum += neuron.OutputBonds[step].Weigh * delta_output[step];
                    }
                    return sum;
                }
                delta_hidden[_hiddenLayers.Length - 1][inStep] = Fsigmoid(_hiddenLayers[_hiddenLayers.Length - 1].AverageNeurons[inStep].Value) * Sum(_hiddenLayers[_hiddenLayers.Length - 1].AverageNeurons[inStep]);
                for (int bondStep = 0; bondStep < _hiddenLayers[_hiddenLayers.Length - 1].AverageNeurons[inStep].OutputBonds.Length; bondStep++)
                {
                    double grad = _hiddenLayers[_hiddenLayers.Length - 1].AverageNeurons[inStep].Value * delta_output[bondStep];
                    double changing = (SpeedE * grad) + (MomentA * hiddenChanging[_hiddenLayers.Length - 1][inStep, bondStep]);
                    _hiddenLayers[_hiddenLayers.Length - 1].AverageNeurons[inStep].OutputBonds[bondStep].Weigh += changing;
                    hiddenChanging[_hiddenLayers.Length - 1][inStep, bondStep] = changing;
                }
            }
            //Остальные скрытые слои
            for (int step = _hiddenLayers.Length - 2; step > -1; step--)
            {
                for (int inStep = 0; inStep < _hiddenLayers[step].AverageNeurons.Count; inStep++)
                {
                    double Sum(Neurons.HiddenNeuron neuron)
                    {
                        double sum = 0;
                        for(int cstep = 0; cstep < neuron.OutputBonds.Length; cstep++)
                        {
                            sum += neuron.OutputBonds[step].Weigh * delta_hidden[step+1][cstep];
                        }
                        return sum;
                    }
                    delta_hidden[step][inStep] = Fsigmoid(_hiddenLayers[step].AverageNeurons[inStep].Value) * Sum(_hiddenLayers[step].AverageNeurons[inStep]);
                    for(int bondStep = 0; bondStep < _hiddenLayers[step].AverageNeurons[inStep].OutputBonds.Length; bondStep++)
                    {
                        double grad = _hiddenLayers[step].AverageNeurons[inStep].Value * delta_hidden[step+1][bondStep];
                        double changing = (SpeedE * grad) + (MomentA * hiddenChanging[step][inStep, bondStep]);
                        _hiddenLayers[step].AverageNeurons[inStep].OutputBonds[bondStep].Weigh += changing;
                        hiddenChanging[step][inStep, bondStep] = changing;
                    }
                }
            }
            //Входной слой
            for(int step = 0; step < _inputLayer.InputNeurons.Count; step++)
            {
                for(int bondStep = 0; bondStep < _inputLayer.InputNeurons[step].OutPutBonds.Length; bondStep++)
                {
                    double grad = _inputLayer.InputNeurons[step].Value * delta_hidden[0][bondStep];
                    double changing = (SpeedE * grad) + (MomentA * inputChanging[step, bondStep]);
                    _inputLayer.InputNeurons[step].OutPutBonds[bondStep].Weigh += changing;
                    inputChanging[step,bondStep] = changing;
                }
            }
            return MSEError(network.currentResults);
        }
        private double Fsigmoid(double outValue) => (1 - outValue) * outValue;
        private double Ftangh(double outValue) => 1 - Math.Pow(outValue, 2);
        private double MSEError(params double[] currentResults)
        {
            double sum = 0;
            for(int step = 0; step < currentResults.Length; step++)
            {
                sum += Math.Pow(_waitingResults[step] - currentResults[step], 2);
            }
            return sum / currentResults.Length;
        }
        private double RootMSEError(params double[] currentResults) => Math.Sqrt(MSEError(currentResults));
        private double ArctanError(params double[] currentResults)
        {
            double sum = 0;
            for(int step = 0; step < _waitingResults.Length; step++)
            {
                sum += Math.Pow(Math.Atan(_waitingResults[step] - currentResults[step]), 2);
            }
            return sum / _waitingResults.Length;
        }
    }
}
