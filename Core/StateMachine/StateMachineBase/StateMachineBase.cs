using System;
using System.Collections.Generic;

namespace Laboratory.Core
{
    public abstract class StateMachineBase<T> where T : IState
    {
        protected Dictionary<Type, T> _states;
        protected T _currentState;

        protected void Enter<TState>() where TState : T
        {
            if (_currentState is IStateWithExit)
            {
                ((IStateWithExit)_currentState).OnExit();
            }
            _currentState = _states[typeof(TState)];
            _currentState.OnEnter();
        }
    }
}