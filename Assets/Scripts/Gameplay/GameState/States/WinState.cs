using Unity.DynamicDuo.UI;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class WinState : IGameState
    {
        private readonly GameResultPresenter m_resultPresenter;

        public WinState(GameResultPresenter resultPresenter)
        {
            m_resultPresenter = resultPresenter;
        }

        public void Enter()
        {
            Debug.Log("[WinState] Enter");
            m_resultPresenter.ShowWin();
        }

        public void Exit()
        {
            Debug.Log("[WinState] Exit");
            m_resultPresenter.Hide();
        }
    }
}
