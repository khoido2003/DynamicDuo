using Unity.DynamicDuo.Gameplay;
using UnityEngine;
using VContainer.Unity;

namespace Unity.DynamicDuo.ApplicationLifecycle
{
    public class GameBootstrapper : IStartable
    {
        readonly GameStateMachine m_stateMachine;
        readonly LoadingState m_loadingState;
        readonly GameplayState m_gameplayState;
        readonly PausedGameState m_pausedState;
        readonly WinState m_winState;
        readonly LoseState m_loseState;

        public GameBootstrapper(
            GameStateMachine stateMachine,
            LoadingState loadingState,
            GameplayState gameplayState,
            PausedGameState pausedState,
            WinState winState,
            LoseState loseState
        )
        {
            m_stateMachine = stateMachine;
            m_loadingState = loadingState;
            m_gameplayState = gameplayState;
            m_pausedState = pausedState;
            m_winState = winState;
            m_loseState = loseState;
        }

        public void Start()
        {
            m_stateMachine.RegisterState(m_loadingState);
            m_stateMachine.RegisterState(m_gameplayState);
            m_stateMachine.RegisterState(m_pausedState);
            m_stateMachine.RegisterState(m_winState);
            m_stateMachine.RegisterState(m_loseState);

            // Start the game
            m_stateMachine.TransitionTo<LoadingState>();
        }
    }
}
