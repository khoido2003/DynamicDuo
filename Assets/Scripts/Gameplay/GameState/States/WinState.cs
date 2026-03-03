using System;
using Unity.DynamicDuo.Infrastructure;
using Unity.DynamicDuo.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.DynamicDuo.Gameplay
{
    public class WinState : IGameState
    {
        private readonly GameResultPresenter m_resultPresenter;
        readonly ISubscriber<RestartRequestedEvent> m_restartSub;
        readonly ISubscriber<NextLevelRequestedEvent> m_nextLevelSub;
        readonly ISubscriber<ReturnToMenuRequestedEvent> m_menuSub;
        readonly GameStateMachine m_stateMachine;

        IDisposable m_restartDisposable;
        IDisposable m_nextLevelDisposable;
        IDisposable m_menuDisposable;

        public WinState(
            GameResultPresenter resultPresenter,
            ISubscriber<RestartRequestedEvent> restartSub,
            ISubscriber<NextLevelRequestedEvent> nextLevelSub,
            GameStateMachine stateMachine,
            ISubscriber<ReturnToMenuRequestedEvent> menuSub
        )
        {
            m_resultPresenter = resultPresenter;
            m_restartSub = restartSub;
            m_nextLevelSub = nextLevelSub;
            m_stateMachine = stateMachine;
            m_menuSub = menuSub;
        }

        public void Enter()
        {
            Debug.Log("[WinState] Enter");
            m_resultPresenter.ShowWin();

            m_restartDisposable = m_restartSub.Subscribe(OnRestart);
            m_nextLevelDisposable = m_nextLevelSub.Subscribe(OnNextLevel);
            m_menuDisposable = m_menuSub.Subscribe(OnMenu);
        }

        void OnRestart(RestartRequestedEvent _)
        {
            // Same seed = same puzzle
            LevelSession.KeepSeed();
            SceneManager.LoadScene("Gameplay");
        }

        void OnNextLevel(NextLevelRequestedEvent _)
        {
            // New seed = new puzzle
            LevelSession.NewSeed();
            SceneManager.LoadScene("Gameplay");
        }

        void OnMenu(ReturnToMenuRequestedEvent _) => m_stateMachine.TransitionTo<MainMenuState>();

        public void Exit()
        {
            Debug.Log("[WinState] Exit");
            m_resultPresenter.Hide();
        }
    }
}
