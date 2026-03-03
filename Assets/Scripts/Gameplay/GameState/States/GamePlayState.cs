using System;
using Unity.DynamicDuo.Infrastructure;
using Unity.DynamicDuo.UI;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class GameplayState : IGameState
    {
        private readonly GameStateMachine m_stateMachine;
        private readonly BoardModel m_boardModel;
        private readonly BoardPresenter m_boardPresenter;
        private readonly ISubscriber<WinEvent> m_winSub;
        private readonly ISubscriber<LoseEvent> m_loseSub;
        private readonly GameResultPresenter m_gameResultPresenter;

        private IDisposable m_winDisposable;
        private IDisposable m_loseDisposable;

        public GameplayState(
            GameStateMachine stateMachine,
            BoardModel boardModel,
            BoardPresenter boardPresenter,
            GameResultPresenter gameResultPresenter,
            ISubscriber<WinEvent> winSub,
            ISubscriber<LoseEvent> loseSub
        )
        {
            m_stateMachine = stateMachine;
            m_boardModel = boardModel;
            m_boardPresenter = boardPresenter;
            m_gameResultPresenter = gameResultPresenter;
            m_winSub = winSub;
            m_loseSub = loseSub;
        }

        public void Enter()
        {
            m_gameResultPresenter.Hide();
            Debug.Log($"[GameplayState] Enter - {m_boardModel.Tubes.Count} tubes ready");

            m_boardPresenter.Activate();

            m_winDisposable = m_winSub.Subscribe(_ => m_stateMachine.TransitionTo<WinState>());
            m_loseDisposable = m_loseSub.Subscribe(_ => m_stateMachine.TransitionTo<LoseState>());
        }

        public void Exit()
        {
            Debug.Log("[GameplayState] Exit");

            m_boardPresenter.Deactivate();

            m_winDisposable?.Dispose();
            m_loseDisposable?.Dispose();
        }
    }
}
