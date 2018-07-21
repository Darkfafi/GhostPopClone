using System;
using System.Collections.Generic;

namespace GameStateSystem
{
    public class StateParameters
    {
        private Dictionary<Type, IStateParameter> _parameters = new Dictionary<Type, IStateParameter>();

        public StateParameters(IStateParameter[] parameters)
        {
            Type t;
            for (int i = 0; i < parameters.Length; i++)
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

            if (b)
            {
                parameterValue = p as T;
            }

            return b;
        }
    }

    public interface IStateParameter
    {

    }
}