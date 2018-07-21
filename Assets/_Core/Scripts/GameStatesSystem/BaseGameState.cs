﻿using System;
using System.Collections.Generic;
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
        if(_initialized)
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
        if(_currentStateParameters != null)
        {
            return;
        }

        _currentStateParameters = parameters;
        OnActivated();
    }

    public GameStateItem? Deactivate(Type[] allSwitchers)
    {
        if (_currentStateParameters == null)
        {
            return null;
        }
        _currentStateParameters = null;
        OnDeactivated();
        return GetGameStateItem(allSwitchers);
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
        return new GameStateItem(GetType(), _currentStateParameters, allSwitchers);
    }

    protected IGame<T> GetGame()
    {
        return _game;
    }

    protected abstract void OnInitialized();
    protected abstract void OnDeinitialize();
    protected abstract void OnActivated();
    protected abstract void OnDeactivated();
}

public class StateParameters
{
    private Dictionary<Type, IStateParameter> _parameters = new Dictionary<Type, IStateParameter>();

    public StateParameters(IStateParameter[] parameters)
    {
        Type t;
        for(int i = 0; i < parameters.Length; i++)
        {
            t = parameters[i].GetType();
            if (!_parameters.ContainsKey(t))
            {
                _parameters.Add(t, parameters[i]);
            }
            else
            {
                UnityEngine.Debug.LogWarningFormat("Can't add parameter of Type {0} because a parameter of that type has already been added", t);
            }
        }
    }

    public T TryGetStateParameter<T>() where T : IStateParameter
    {
        IStateParameter p;
        _parameters.TryGetValue(typeof(T), out p);
        return (T)p;
    }
}

public interface IStateParameter
{

}