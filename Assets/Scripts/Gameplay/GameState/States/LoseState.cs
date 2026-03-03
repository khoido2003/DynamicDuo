using Unity.DynamicDuo.UI;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class LoseState : IGameState
    {
        private readonly GameResultPresenter m_resultPresenter;

        public LoseState(GameResultPresenter resultPresenter)
        {
            m_resultPresenter = resultPresenter;
        }

        public void Enter()
        {
            Debug.Log("[LoseState] Enter");
            m_resultPresenter.ShowLose();
        }

        public void Exit()
        {
            Debug.Log("[LoseState] Exit");
            m_resultPresenter.Hide();
        }
    }
}
