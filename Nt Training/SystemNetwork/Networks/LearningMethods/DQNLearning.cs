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
            public struct Transition
            {
                public double s, a, r, next_s; 
                public bool done_mask;
                public Transition(double s, double a, double r, double next_s, bool done_mask)
                {
                    this.s = s;
                    this.a = a;
                    this.r = r;
                    this.next_s = next_s;
                    this.done_mask = done_mask;
                }
            }
            public Transition[] transitions;
            private int index;
            public int Legth { get => index + 1; }
            public ReplayBuffer(int max_size)
            { //Создаём структуру для хранения данных
                transitions = new Transition[max_size];
                index = 0;
            }
            public void Put(double s, double a, double r, double next_s, bool done_mask)
            {
                //Помещаем данные в replay buffer
                //param transition: (s, a, r, next_s, done_mask)
                transitions[index++] = new Transition(s, a, r, next_s, done_mask);
            }
            public Transition[] Sample(int n)
            {
                //сэмплируем батч заданного размера
                //param n: размер мини-батча
                //return:
                Random rand = new Random();
                Transition[] transitions = new Transition[n];
                for(int step = 0; step < n; step++)
                {
                    transitions[step] = this.transitions[rand.Next(this.transitions.Length)];
                }
                return transitions;
            }
        }
        ReplayBuffer replayBuffer;
        public DQNLearning()
        {
            replayBuffer = new ReplayBuffer(5);
        }//q - наша сеть, q-target - target сеть, optimizer - оптимизатор, 
        public void train(QNetwork q, double q_target, ReplayBuffer replayBuffer, int optimizer, int batch_size, double gamma, int updates_number = 10)
        {//тренируем нашу архитектуру
            //param q: policy сеть
            //param q_target: target сеть
            //param replay_buffer
            //param optimizer
            //param batch_size: размер мини-батча
            //param gamma: дисконтирующий множитель
            //param updates_number: количество обновлений, которые необходимо выполнить
            //return
            for(int step = 0; step < updates_number; step++)
            {
                //сэмплируем мини-батч из replay buffer-а
                ReplayBuffer.Transition[] transitions = replayBuffer.Sample(batch_size);
                //Получаем полезность для выбранного действия q сети
                //double q_out = q(s);
                //q_a = q_out.gather(1, a)
                //получаем значение max_q target сети и считаем значение target
                //max_q_prime = q_target(s_prime).max(1)[0].unsqueeze(1)
                //target = r + gamma * max_q_prime * done_mask
                //определяем Loss функцию для q
                //loss = F.smooth_l1_loss(q_a, target.detach())
                //optimizer.zero_grad()
                //loss.backward()
                //optimizer.step()
            }
        }
        public void run() //learning_rate, gamma, buffer_max_size, batch_size, target_update_interval, replay_buffer_start_size, print_interval = 20, n_episodes = 2000
        {
            //создаём окружение
            //создаём q и target_q
            //копируем веса q в target_q
            //создаём replay_buffer
            //инициализируем оптимизатор, полученным Lr
            //в цикле for n_epi in range(n_episodes):
            //постепенно меняем eps с 8% до 1%
            //выполняем 600 шагов в окружении и сохраняем полученные данные for t in range(600)
            //получаем действие используя сеть q
            //выполняем действие в окружении
            //
        }
    }
}
