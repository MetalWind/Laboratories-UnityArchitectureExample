using System;
using System.Collections.Generic;
using Zenject;

namespace Laboratory.Core
{
    public abstract class GameStateMachineBase : StateMachineBase<GameState>
    {
        [Inject]
        public GameStateMachineBase(TypeObjFactory factory)
        {
            _states = new Dictionary<Type, GameState>();
            CreateStateDict(factory);
        }

        public abstract void StartGame();

        private void CreateStateDict(TypeObjFactory factory)
        {
            foreach (GameState state in factory.CreateAll<GameState>())
            {
                _states[state.GetType()] = state;
            }
        }
    }
}