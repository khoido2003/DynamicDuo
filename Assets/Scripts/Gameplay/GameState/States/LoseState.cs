using System;
using Unity.DynamicDuo.Infrastructure;
using Unity.DynamicDuo.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.DynamicDuo.Gameplay
{
    public class LoseState : IGameState
    {
        readonly GameResultPresenter m_resultPresenter;
        readonly ISubscriber<RestartRequestedEvent> m_restartSub;
        readonly GameStateMachine m_stateMachine;
        readonly ISubscriber<ReturnToMenuRequestedEvent> m_menuSub;

        IDisposable m_restartDisposable;
        IDisposable m_menuDisposable;

        public LoseState(
            GameResultPresenter resultPresenter,
            ISubscriber<RestartRequestedEvent> restartSub,
            ISubscriber<ReturnToMenuRequestedEvent> menuSub,
            GameStateMachine stateMachine
        )
        {
            m_resultPresenter = resultPresenter;
            m_restartSub = restartSub;
            m_stateMachine = stateMachine;
        }

        public void Enter()
        {
            Debug.Log("[LoseState] Enter");
            m_resultPresenter.ShowLose();
            m_restartDisposable = m_restartSub.Subscribe(OnRestart);

            m_menuDisposable = m_menuSub.Subscribe(OnMenu);
        }

        void OnRestart(RestartRequestedEvent _)
        {
            LevelSession.KeepSeed();
            SceneManager.LoadScene("Gameplay");
        }

        void OnMenu(ReturnToMenuRequestedEvent _) => m_stateMachine.TransitionTo<MainMenuState>();

        public void Exit()
        {
            Debug.Log("[LoseState] Exit");
            m_resultPresenter.Hide();
            m_restartDisposable?.Dispose();
        }
    }
}
