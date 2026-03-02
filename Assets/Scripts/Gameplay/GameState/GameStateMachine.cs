using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class GameStateMachine
    {
        private IGameState m_currentState;

        private readonly Dictionary<Type, IGameState> m_states = new();

        public void RegisterState<T>(T state)
            where T : IGameState
        {
            m_states[typeof(T)] = state;
        }

        public void TransitionTo<T>()
            where T : IGameState
        {
            if (!m_states.TryGetValue(typeof(T), out var nextState))
            {
                Debug.LogError($"[GameStateMachine] State {typeof(T).Name} not registered.");
                return;
            }

            m_currentState?.Exit();
            m_currentState = nextState;

            Debug.Log($"[GameStateMachine] → {typeof(T).Name}");
            m_currentState.Enter();
        }

        public bool IsInState<T>()
            where T : IGameState => m_currentState is T;
    }
}
