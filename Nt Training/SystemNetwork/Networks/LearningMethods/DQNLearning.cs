using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Networks.LearningMethods
{
    public class DQNLearning
    {
        public class QNetwork
        {
            Network _network;
            public QNetwork(int inputCount, int outputCount, params int[] hiddenCount) //Вход для сети является текущим, в то время как выход является соответствующим Q-значением для каждого действия.
            {
                Neurons.InputNeuron[] inputNeurons = new Neurons.InputNeuron[inputCount];
                for (int step = 0; step < inputNeurons.Length; step++) inputNeurons[step] = new Neurons.InputNeuron();
                Neurons.HiddenNeuron[][] averageNeurons = new Neurons.HiddenNeuron[hiddenCount.Length][];
                for (int step = 0; step < hiddenCount.Length; step++)
                {
                    for (int inStep = 0; inStep < hiddenCount[step]; inStep++) averageNeurons[step][inStep] = new Neurons.HiddenNeuron();
                }
                Neurons.OutputNeuron[] outputNeurons = new Neurons.OutputNeuron[outputCount];
                for (int step = 0; step < outputNeurons.Length; step++) outputNeurons[step] = new Neurons.OutputNeuron();
                List<Bonds.Bond> bonds = new List<Bonds.Bond>();
                Random rand = new Random();
                foreach (Neurons.InputNeuron inputNeuron in inputNeurons)
                {
                    foreach (Neurons.HiddenNeuron averageNeuron in averageNeurons[0])
                    {
                        bonds.Add(new Bonds.Bond(inputNeuron, averageNeuron, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                        inputNeuron.AddOutPutBond(bonds.Last());
                        averageNeuron.AddInputBond(bonds.Last());
                    }
                }
                for(int step = 1; step < hiddenCount.Length-1; step++)
                {
                    foreach (Neurons.HiddenNeuron averageNeuron in averageNeurons[step])
                    {
                        foreach (Neurons.HiddenNeuron averageNeuron1 in averageNeurons[step+1])
                        {
                        bonds.Add(new Bonds.Bond(averageNeuron, averageNeuron1, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                        averageNeuron.AddOutputBond(bonds.Last());
                        averageNeuron1.AddInputBond(bonds.Last());
                        }
                    }
                }
                foreach (Neurons.HiddenNeuron averageNeuron in averageNeurons[hiddenCount.Length-1])
                {
                    foreach (Neurons.OutputNeuron outputNeuron in outputNeurons)
                    {
                        bonds.Add(new Bonds.Bond(averageNeuron, outputNeuron, Convert.ToDouble(rand.Next(-50, 50)) / 100));
                        averageNeuron.AddOutputBond(bonds.Last());
                        outputNeuron.AddInputBond(bonds.Last());
                    }
                }
                Layers.HiddenLayer[] averageLayer = new Layers.HiddenLayer[hiddenCount.Length];
                for (int step = 0; step < averageLayer.Length; step++) averageLayer[step] = new Layers.HiddenLayer(averageNeurons[step]);
                Neurons.ActivationFunctions.LogisticFunction function = new Neurons.ActivationFunctions.LogisticFunction(2); //Гипер-параметр
                _network = new Network(function);
                _network.AddInPutNeurons(inputNeurons);
                foreach (Layers.HiddenLayer hiddenLayer in averageLayer) _network.AddAverageLayer(hiddenLayer); //Названия не совпадают (Average - Hidden)
                _network.AddOutPutNeurons(outputNeurons);
            }
            public void Forward()
            { //Определение графа вычислений //определение прямого прохода нейронной сети
                //param x: вход
                //return:
                // x = F.relu(self.fc1(x)) первое линейное преобразование
                // x = self.fc2(x) второе линейное преобразование
                //return x
            }
            public int Sample_action(double obs, double epsilon)
            { //Сэмплирование действия
                //param obs:
                //param epsilon:
                //return:

                //out = self.forward(obs)
                Random rand = new Random();
                var coin = Convert.ToDouble(rand.Next(100)) / 100;
                if (coin < epsilon) return rand.Next(2);
                else return 1;//out.argmax().item() //выбираем то действие, которое оптимально // 1 - удалить
            }
        }
        public class ReplayBuffer
        {
            
        }
        ReplayBuffer replayBuffer;
        public DQNLearning()
        {
            replayBuffer = new ReplayBuffer();
        }
        //Представляет собой цикл, только каждая итерация - это каждый выхов метода
        public void SetAndUpdate() //для 1...D (при каждом обновлении среды)
        {
            //Получаем x0(первые получаемые данные)
            //s0 = CNN(x0)
        }
        /*private Y Get_action(T state)
        {

        }*/
        public void Step() //для 1...T (при каждом ходе)
        {
            //Get_Action()
            //(rt, xt+1, st+1 = CNN(xt+1))
            //(st, at, rt, st+1) -> M
            //Сэмплируем минибатч из Memory
            //y(оценка) = 
            //обновление весов
        }
        //DQN использует нейронную сеть для оценки функции Q-значения
    }
}
