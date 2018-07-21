using System;

namespace GameStateSystem
{
    /// <summary>
    /// State which can create other classes (including create objects & components) which would determine the rules of the Game World (T)
    /// </summary>
    public abstract class BaseGameState<T> where T : class, IGameWorld
    {
        public T GameWorld
        {
            get;
            private set;
        }

        private IGame<T> _game;
        private bool _initialized = false;
        private StateParameters _currentStateParameters;

        public void Initialize(T gameWorld, IGame<T> game)
        {
            if (_initialized)
            {
                return;
            }

            _game = game;
            GameWorld = gameWorld;
            _initialized = true;
            OnInitialized();
        }

        public void Activate(StateParameters parameters)
        {
            if (_currentStateParameters != null)
            {
                return;
            }
            _currentStateParameters = parameters;
            OnActivated(parameters);
        }

        public GameStateItem? Deactivate(Type[] allSwitchers)
        {
            if (_currentStateParameters == null)
            {
                return null;
            }
            OnDeactivated();
            GameStateItem item = GetGameStateItem(allSwitchers);
            _currentStateParameters.Clean();
            _currentStateParameters = null;
            return item;
        }

        public void Deinitialize()
        {
            if (!_initialized)
            {
                return;
            }
            OnDeinitialize();
            GameWorld = null;
            _initialized = false;
        }

        public GameStateItem GetGameStateItem(Type[] allSwitchers)
        {
            return new GameStateItem(GetType(), _currentStateParameters.Copy(), allSwitchers);
        }

        protected IGame<T> GetGame()
        {
            return _game;
        }

        protected abstract void OnInitialized();
        protected abstract void OnDeinitialize();
        protected abstract void OnActivated(StateParameters parameters);
        protected abstract void OnDeactivated();
    }
}