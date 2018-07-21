using System;
using System.Collections.Generic;
namespace GameStateSystem
{
    public class GameStateConstructor<T> where T : class, IGameWorld
    {
        public StateParameters Parameters { get; private set; }

        public Type[] SwitcherTypes
        {
            get
            {
                return _switcherTypes.ToArray();
            }
        }

        private List<Type> _switcherTypes = new List<Type>();

        public static GameStateConstructor<T> Create()
        {
            return new GameStateConstructor<T>().Initialize();
        }

        public static GameStateConstructor<T> Create(StateParameters parameters)
        {
            return new GameStateConstructor<T>().Initialize(parameters);
        }

        public static GameStateConstructor<T> Create(GameStateItem item)
        {
            return new GameStateConstructor<T>().Initialize(item);
        }

        public GameStateConstructor<T> AddSwitcher<U>() where U : BaseStateSwitcher<T>
        {
            _switcherTypes.Add(typeof(U));
            return this;
        }

        public GameStateConstructor<T> SetStateParameters(params IStateParameter[] parameters)
        {
            Parameters = new StateParameters(parameters);
            return this;
        }

        private GameStateConstructor<T> Initialize()
        {
            return Initialize(new StateParameters(new IStateParameter[] { }));
        }

        private GameStateConstructor<T> Initialize(StateParameters parameters)
        {
            Parameters = parameters;
            _switcherTypes = new List<Type>();
            return this;
        }

        private GameStateConstructor<T> Initialize(GameStateItem item)
        {
            Initialize(item.Parameters);

            if (item.SwitcherTypes != null)
                _switcherTypes.AddRange(item.SwitcherTypes);

            return this;
        }
    }
}