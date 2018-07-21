using System;
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

    public StateParameters Copy()
    {
        List<IStateParameter> parameters = new List<IStateParameter>();

        if (_parameters != null)
        {
            foreach (var p in _parameters)
            {
                parameters.Add(p.Value);
            }
        }

        return new StateParameters(parameters.ToArray());
    }

    public void Clean()
    {
        _parameters.Clear();
        _parameters = null;
    }

    public bool TryGetStateParameter<T>(out T parameterValue, T defaultValue = null) where T : class, IStateParameter
    {
        IStateParameter p;
        parameterValue = defaultValue;

        bool b = _parameters.TryGetValue(typeof(T), out p);

        if(b)
        {
            parameterValue = p as T;
        }

        return b;
    }
}

public interface IStateParameter
{

}