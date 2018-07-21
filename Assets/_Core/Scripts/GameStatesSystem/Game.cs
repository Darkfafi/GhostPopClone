using System;
using System.Collections.Generic;
using UnityEngine;

public class Game<T, U> : IGame<T> where T : class, IGameWorld where U : BaseGameState<T>, new()
{
    private Dictionary<Type, BaseGameState<T>> _cachedGameStates = new Dictionary<Type, BaseGameState<T>>();
    private Stack<GameStateItem> _gameStateHistory = new Stack<GameStateItem>();
    private BaseGameState<T> _currentGameState;
    private T _gameWorld;

    public Game(T gameWorld, params IStateParameter[] defaultStateParameters)
    {
        _gameWorld = gameWorld;
        _gameStateHistory = new Stack<GameStateItem>();
        SwitchGameState<U>(defaultStateParameters);
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

    public void SwitchGameState<GameState>(params IStateParameter[] parameters) where GameState : BaseGameState<T>, new()
    {
        SwitchGameState(typeof(GameState), parameters);
    }

    public void SwitchGameState(GameStateItem gameStateItem)
    {
        InternalSwitchGameState(gameStateItem.GameStateType, gameStateItem.Parameters, gameStateItem.RawData);
    }

    public void SwitchGameState(Type gameStateType, params IStateParameter[] parameters)
    {
        InternalSwitchGameState(gameStateType, new StateParameters(parameters), new GameStateRawData());
    }

    public void SwitchGameState(Type gameStateType, StateParameters parameters)
    {
        InternalSwitchGameState(gameStateType, parameters, new GameStateRawData());
    }

    public void SwitchToPreviousGameState()
    {
        if(_gameStateHistory.Count > 0)
        {
            SwitchGameState(_gameStateHistory.Peek()); // Not Pop, because we want the history chain to stay untouched.
        }
    }

    public void SwitchToPreviousGameState(IStateParameter[] parametersOverride)
    {
        if (_gameStateHistory.Count > 0)
        {
            SwitchGameState(new GameStateItem(_gameStateHistory.Peek().GameStateType, new StateParameters(parametersOverride), _gameStateHistory.Peek().RawData)); // Not Pop, because we want the history chain to stay untouched.
        }
    }

    public void ResetGame()
    {
        T gw = _gameWorld;
        CleanGame();
        _gameWorld = gw;
        _cachedGameStates = new Dictionary<Type, BaseGameState<T>>();
        _gameStateHistory = new Stack<GameStateItem>();
        SwitchGameState<U>();
    }

    public void CleanGame()
    {
        StateSwitchOperation(null, null, null);

        foreach(var pair in _cachedGameStates)
        {
            pair.Value.Deinitialize();
        }

        _gameWorld = null;

        _cachedGameStates.Clear();
        _cachedGameStates = null;

        _gameStateHistory.Clear();
        _gameStateHistory = null;
    }


    private void InternalSwitchGameState(Type gameStateType, StateParameters parameters, GameStateRawData rawData)
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
            StateSwitchOperation(gameStateInstance, parameters, rawData);
        }
    }

    private void StateSwitchOperation(BaseGameState<T> gameStateInstance, StateParameters parameters, GameStateRawData rawData)
    {
        if (_currentGameState != null)
        {
            GameStateItem? i = _currentGameState.Deactivate();
            if (i.HasValue)
            {
                _gameStateHistory.Push(i.Value);
            }
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
            _currentGameState.Activate(parameters, rawData);
        }
    }
}


public interface IGame<T> where T : class, IGameWorld
{
    T GetGameWorld();
    Type GetCurrentStateType();
    GameStateItem GetPreviousState();
    GameStateItem[] GetStateHistory();
    void SwitchGameState<GameState>(params IStateParameter[] parameters) where GameState : BaseGameState<T>, new();
    void SwitchGameState(Type gameStateType, params IStateParameter[] parameters);
    void SwitchToPreviousGameState();
    void ResetGame();
}

public struct GameStateItem
{
    public Type GameStateType;
    public StateParameters Parameters;
    public GameStateRawData RawData;

    public GameStateItem(Type gameStateType, StateParameters parameters, GameStateRawData currentRawData)
    {
        GameStateType = gameStateType;
        Parameters = parameters;
        RawData = currentRawData;
    }
}