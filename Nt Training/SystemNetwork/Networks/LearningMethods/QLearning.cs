using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training.SystemNetwork.Networks.LearningMethods
{
    //ПОД НАГРАДОЙ ПОНИМАЕТСЯ РАССТОЯНИЕ ДО ЦЕЛИ
    //СТРАТЕГИЯ ЗАКЛЮЧАЕТСЯ В ВЫБОРЕ НАИЛУЧШЕГО ДЕЙСТВИЯ В СООТВЕТCТВИИ С ОЦЕНКАМИ Q
    public class QLearning<T, Y> where T : struct, IEquatable<T> where Y : struct
    {
        private class State : IEquatable<State>
        {
            public class Action
            {
                public double q_value;
                Y _outAction;
                public Y OutAction { get => _outAction; }
                public Action(Y outAction, double q_value = 0)
                {
                    this.q_value = q_value;
                    _outAction = outAction;
                }
            }
            public List<Action> actions;
            T _outState;
            public T OutState { get => _outState; }
            public State(Action[] actions, T outState)
            {
                this.actions = new List<Action>();
                this.actions.AddRange(actions);
                _outState = outState;
            }

            public bool Equals(State other)
            {
                if (_outState.Equals(other._outState)) return true;
                else return false;
            }
        }
        private double _alpha;
        private double _eps;
        private double _discount; //штраф(Гамма)
        private List<State> states;
        public int CountStates { get => states.Count; }
        public QLearning(double alpha, double epsilon, double discount)
        {
            states = new List<State>();
            _alpha = alpha;
            _eps = epsilon;
            _discount = discount; 
        }
        private double Get_V(State state)
        {//Вычисление оценки V
         //Получаем возможные действия для нашего текущего состояния
            //Выбираем максимальное Q_value для всех возможных действий, которые мы можем совершить
            //Возвращаем максимальное Q_value
            return state.actions.Max(n => n.q_value);
        }
        private State.Action Get_Policy(State state)
        { //По состоянию, возвращает лучшее действие в этом состоянии используя текущие оценки Q-функции
          //Выбираем лучшее действие согласно стратегии
            State.Action best_action = null; //лучшего действия мы пока не знаем
            foreach (State.Action action in state.actions)
            {//Для каждого действия выбираем
                if (best_action == null) best_action = action;
                else if (action.q_value > best_action.q_value) best_action = action; //Если текущее q_value > запомненное => best_action = action
            }
            return best_action;
        }
        private State.Action Get_action(State state)
        { //Для конкретной ситуации выбирается действие, используя e-жадный подход
          //Выбирает действие, предпринимаемое в данном состоянии, включая исследование (eps greedy)
          //С вероятностью epsilon берём случайное действие, иначе действие согласно стратегии (Get_policy)
            Random rand = new Random();
            if ((double)rand.Next(100) / 100 < _eps) return state.actions[rand.Next(state.actions.Count)];
            else return Get_Policy(state);
        }
        private void Update(State.Action action, State next_state, double reward)
        { //Функция Q-обновления
            //Выполняем Q-обновление,
            double t = _alpha * (reward + _discount * Get_V(next_state) - action.q_value);
            double q_value = action.q_value + t;
            action.q_value = q_value;
        }
        double total_reward;
        public double Total_Reward { get => total_reward; }
        State state;
        private State NewOrOldState(T outState, Y[] outActions)
        {
            State.Action[] newActions = new State.Action[outActions.Length];
            for (int step = 0; step < newActions.Length; step++) newActions[step] = new State.Action(outActions[step]);
            State newState = new State(newActions, outState);
            if (!states.Contains(newState)) states.Add(newState);
            else foreach (State state in states) if (state.Equals(newState)) newState = state;
            return newState;
        }
        public void SetAndUpdate(T outState, Y[] outActions) //Вызывается при каждом обновлении карты первым
        { //Теперь нужно придумать как одновременно передавать и состояние и действия для него
            total_reward = 0.0;
            state = NewOrOldState(outState, outActions);
        }
        public Y Step(T nextState, Y[] outActions, double reward) //s - внешнее состояние
        {//Вызывается каждый ход
            State newState = NewOrOldState(nextState, outActions);
            var a = Get_action(state); //Получаем Action от текущего State //Получается нужно найти State, который содержит s, но если создавать новый, то т.к. это ссылочный тип, оно будет считать их разными элементами
            Update(a, newState, reward); //Нужно передавать action от текущего state - a
            state = newState;
            total_reward += reward;
            return Get_action(newState).OutAction;
        }
    }
}
