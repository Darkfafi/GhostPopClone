using System;
using System.Collections.Generic;
using UnityEngine;

public class Game<T, U> : IGame<T> where T : class, IGameWorld where U : BaseGameState<T>, new()
{
    private T _gameWorld;
    private Dictionary<Type, BaseGameState<T>> _cachedGameStates = new Dictionary<Type, BaseGameState<T>>();
    private GameStateConstructor<T> _defaultStateConstructor;
    private Stack<GameStateItem> _gameStateHistory = new Stack<GameStateItem>();
    private Dictionary<Type, BaseGlobalStateSwitcher<T>> _globalStateSwitchers = new Dictionary<Type, BaseGlobalStateSwitcher<T>>();
    private Dictionary<Type, BaseStateSwitcher<T>> _currentGameStateSwitchers = new Dictionary<Type, BaseStateSwitcher<T>>();
    private Type[] _currentSwitcherTypes;
    private BaseGameState<T> _currentGameState;

    public Game(T gameWorld, GameStateConstructor<T> stateConstructor = null)
    {
        _gameWorld = gameWorld;
        _cachedGameStates = new Dictionary<Type, BaseGameState<T>>();
        _gameStateHistory = new Stack<GameStateItem>();
        _globalStateSwitchers = new Dictionary<Type, BaseGlobalStateSwitcher<T>>();
        _currentGameStateSwitchers = new Dictionary<Type, BaseStateSwitcher<T>>();
        _defaultStateConstructor = stateConstructor;
        SwitchGameState<U>(stateConstructor);
    }

    public T GetGameWorld()
    {
        return _gameWorld;
    }

    public Type GetCurrentStateType()
    {
        return _currentGameState == null ? null : _currentGameState.GetType();
    }

    public GameStateItem GetPreviousState()
    {
        return _gameStateHistory.Count == 0 ? new GameStateItem(null, null, null) : _gameStateHistory.Peek();
    }

    public GameStateItem[] GetStateHistory()
    {
        return _gameStateHistory.ToArray();
    }

    public void SwitchGameState<GameState>(GameStateConstructor<T> constructor = null) where GameState : BaseGameState<T>, new()
    {
        SwitchGameState(typeof(GameState), constructor);
    }

    public void SwitchGameState(GameStateItem gameStateItem)
    {
        InternalSwitchGameState(gameStateItem.GameStateType, GameStateConstructor<T>.Create(gameStateItem));
    }

    public void SwitchGameState(Type gameStateType, GameStateConstructor<T> constructor = null)
    {
        InternalSwitchGameState(gameStateType, constructor);
    }

    public void SwitchToPreviousGameState()
    {
        if(_gameStateHistory.Count > 0)
        {
            GameStateItem item = _gameStateHistory.Peek();
            SwitchGameState(item); // Not Pop, because we want the history chain to stay untouched.
        }
    }

    public void SwitchToPreviousGameState(IStateParameter[] parametersOverride)
    {
        if (_gameStateHistory.Count > 0)
        {
            GameStateItem item = _gameStateHistory.Peek();
            item.Parameters = new StateParameters(parametersOverride);
            SwitchGameState(item); // Not Pop, because we want the history chain to stay untouched.
        }
    }

    public void AddGlobalStateSwitcher<GSS>() where GSS : BaseGlobalStateSwitcher<T>
    {
        if(!HasGlobalStateSwitcherType<GSS>())
        {
            GSS stateSwitcher = Activator.CreateInstance(typeof(GSS)) as GSS;
            _globalStateSwitchers.Add(typeof(GSS), stateSwitcher);
            stateSwitcher.Activate(this);
        }
    }

    public bool HasGlobalStateSwitcherType<GSS>() where GSS : BaseGlobalStateSwitcher<T>
    {
        return _globalStateSwitchers.ContainsKey(typeof(GSS));
    }

    public void RemoveGlobalStateSwitcher<GSS>() where GSS : BaseGlobalStateSwitcher<T>
    {
        if (HasGlobalStateSwitcherType<GSS>())
        {
            BaseGlobalStateSwitcher<T> ss = _globalStateSwitchers[typeof(GSS)];
            _globalStateSwitchers.Remove(typeof(GSS));
            ss.Deactivate();
        }
    }

    public void RemoveAllGlobalSwitchers()
    {
        foreach(var p in _globalStateSwitchers)
        {
            p.Value.Deactivate();
        }

        _globalStateSwitchers.Clear();
    }

    public void ResetGame()
    {
        T gw = _gameWorld;
        CleanGame();
        _gameWorld = gw;
        _cachedGameStates = new Dictionary<Type, BaseGameState<T>>();
        _gameStateHistory = new Stack<GameStateItem>();
        _globalStateSwitchers = new Dictionary<Type, BaseGlobalStateSwitcher<T>>();
        _currentGameStateSwitchers = new Dictionary<Type, BaseStateSwitcher<T>>();
        SwitchGameState<U>(_defaultStateConstructor);
    }

    public void CleanGame()
    {
        StateSwitchOperation(null, null);
        _currentGameStateSwitchers = null;

        RemoveAllGlobalSwitchers();
        _globalStateSwitchers = null;

        foreach (var pair in _cachedGameStates)
        {
            pair.Value.Deinitialize();
        }

        _gameWorld = null;

        _cachedGameStates.Clear();
        _cachedGameStates = null;

        _gameStateHistory.Clear();
        _gameStateHistory = null;
    }


    private void InternalSwitchGameState(Type gameStateType, GameStateConstructor<T> constructor)
    {
        BaseGameState<T> gameStateInstance;

        if (!_cachedGameStates.TryGetValue(gameStateType, out gameStateInstance))
        {
            gameStateInstance = Activator.CreateInstance(gameStateType) as BaseGameState<T>;
        }

        if (gameStateInstance == null)
        {
            Debug.LogErrorFormat("Can't switch to game state of type {0} because it is was not created for GameWorld type {1} or the constructor has parameters", gameStateType, _gameWorld.GetType());
        }
        else
        {
            StateSwitchOperation(gameStateInstance, constructor);
        }
    }

    private void StateSwitchOperation(BaseGameState<T> gameStateInstance, GameStateConstructor<T> constructor)
    {
        if (_currentGameState != null)
        {
            foreach(var p in _currentGameStateSwitchers)
            {
                p.Value.Deactivate(_currentGameState);
            }

            GameStateItem? i = _currentGameState.Deactivate(_currentSwitcherTypes);
            if (i.HasValue)
            {
                _gameStateHistory.Push(i.Value);
            }

            _currentSwitcherTypes = null;
            _currentGameStateSwitchers.Clear();
        }

        _currentGameState = null;

        if (gameStateInstance != null)
        {
            Type t = gameStateInstance.GetType();
            if (!_cachedGameStates.ContainsKey(t))
            {
                _cachedGameStates.Add(t, gameStateInstance);
                gameStateInstance.Initialize(_gameWorld, this);
            }
            else
            {
                gameStateInstance = _cachedGameStates[t];
            }

            _currentGameState = gameStateInstance;

            StateParameters parameters = null;

            if (constructor != null)
            {

                _currentSwitcherTypes = constructor.SwitcherTypes;
                parameters = constructor.Parameters;

                if (_currentSwitcherTypes != null && _currentSwitcherTypes.Length > 0)
                {
                    for (int i = 0; i < _currentSwitcherTypes.Length; i++)
                    {
                        Type switcherType = _currentSwitcherTypes[i];
                        if (!_currentGameStateSwitchers.ContainsKey(switcherType))
                        {
                            BaseStateSwitcher<T> switcher = Activator.CreateInstance(switcherType) as BaseStateSwitcher<T>;
                            _currentGameStateSwitchers.Add(_currentSwitcherTypes[i], switcher);
                            switcher.Activate(this, _currentGameState);
                        }
                    }
                }
            }

            if(parameters == null)
                parameters = new StateParameters(new IStateParameter[] { });

            _currentGameState.Activate(parameters);
        }
    }
}


public interface IGame<T> where T : class, IGameWorld
{
    T GetGameWorld();
    Type GetCurrentStateType();
    GameStateItem GetPreviousState();
    GameStateItem[] GetStateHistory();
    void SwitchGameState<GameState>(GameStateConstructor<T> constructor = null) where GameState : BaseGameState<T>, new();
    void SwitchGameState(Type gameStateType, GameStateConstructor<T> constructor = null);
    void SwitchGameState(GameStateItem gameStateItem);
    void SwitchToPreviousGameState();
    void ResetGame();
}

public struct GameStateItem
{
    public Type GameStateType;
    public StateParameters Parameters;
    public Type[] SwitcherTypes;

    public GameStateItem(Type gameStateType, StateParameters parameters, Type[] switcherTypes)
    {
        GameStateType = gameStateType;
        Parameters = parameters;
        SwitcherTypes = switcherTypes;
    }
}

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