using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training.SystemNetwork.Networks.LearningMethods
{
    //ПОД НАГРАДОЙ ПОНИМАЕТСЯ РАССТОЯНИЕ ДО ЦЕЛИ
    //СТРАТЕГИЯ ЗАКЛЮЧАЕТСЯ В ВЫБОРЕ НАИЛУЧШЕГО ДЕЙСТВИЯ В СООТВЕТCТВИИ С ОЦЕНКАМИ Q
    public class RLearning<T, Y> where T : RLearning<T, Y>.State, IComparable<T> where Y : RLearning<T, Y>.State.Action //where T : struct, IComparable<T>
    {
        public abstract class State //Состояние хранит параметры и другое, передаваемые самим пользователем (outState), тоесть это то, что передаёт сам пользователь и он может их использовать
        {
            public abstract class Action
            {
                public double q_value;
                public Action(double q_value = 0)
                {
                    this.q_value = q_value;
                }
            }
            public List<Y> actions;
            public State(Y[] actions)
            {
                this.actions = new List<Y>();
                this.actions.AddRange(actions);
            }
        }
        private double _alpha;
        private double _eps;
        private double _discount; //штраф(Гамма)
        List<T> states;
        public RLearning(double alpha, double epsilon, double discount)
        {
            states = new List<T>();
            _alpha = alpha;
            _eps = epsilon;
            _discount = discount; 
        }
        private double Get_V(T state)
        {//Вычисление оценки V
         //Получаем возможные действия для нашего текущего состояния
            //Выбираем максимальное Q_value для всех возможных действий, которые мы можем совершить
            //Возвращаем максимальное Q_value
            return state.actions.Max(n => n.q_value);
        }
        private Y Get_Policy(T state)
        { //По состоянию, возвращает лучшее действие в этом состоянии используя текущие оценки Q-функции
          //Выбираем лучшее действие согласно стратегии
            Y best_action = null; //Типо лучшего действия мы пока не знаем
            foreach (Y action in state.actions)
            {//Для каждого действия выбираем
                if (best_action == null) best_action = action;
                else if (action.q_value > best_action.q_value) best_action = action; //Если текущее q_value > запомненное => best_action = action
            }
            return best_action;
        }
        private Y Get_action(T state)
        { //Для конкретной ситуации выбирается действие, используя e-жадный подход
          //Выбирает действие, предпринимаемое в данном состоянии, включая исследование (eps greedy)
          //С вероятностью epsilon берём случайное действие, иначе действие согласно стратегии (Get_policy)
            Random rand = new Random();
            if (rand.Next(100) / 100 < _eps) return state.actions[rand.Next(states.Count)]; //А что делать если это новый State и у него ещё нет действий
            else return Get_Policy(state);
        }
        //get_q_value - возвращает q_value для данного state и action
        //set_q_value - добавляет value в коллекцию по state и action
        private void Update(Y action, T next_state, double reward)
        { //Функция Q-обновления
            //Выполняем Q-обновление,
            double t = _alpha * (reward + _discount * Get_V(next_state) - action.q_value);
            double q_value = action.q_value + t;
            action.q_value = q_value; 
        }
        //Перед всеми шагами нужно обновить окружение
        double total_reward;
        public double Total_Reward { get => total_reward; }
        T state;
        public void SetAndUpdate(T s) //Вызывается при каждом обновлении карты первым
        {
            if (!states.Contains(s)) states.Add(s);
            total_reward = 0.0;
            state = s;
        }
        public void Step(T nextState, double reward) //s - внешнее состояние
        {//Вызывается каждый ход
            if (!states.Contains(nextState)) states.Add(nextState);
            var a = Get_action(state); //Получаем Action от текущего State //Получается нужно найти State, который содержит s, но если создавать новый, то т.к. это ссылочный тип, оно будет считать их разными элементами
            Update(a, nextState, reward); //Нужно передавать action от текущего state - a
            state = nextState;
            total_reward += reward;
        }
        //Похоже, что нужно передавать сразу State с Action-ами
        //Что же делать с действиями?
        //А что если сразу убрать State и Action классы, а использовать вместо них пользовательские
        //Если нет такого State s states, то нужно добавлять новый
    }
}
