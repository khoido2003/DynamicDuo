using System;
using Unity.DynamicDuo.Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.DynamicDuo.UI
{
    public class GameResultPresenter : IDisposable
    {
        private readonly GameResultView m_view;

        private readonly IPublisher<RestartRequestedEvent> m_restartPub;
        private readonly IPublisher<NextLevelRequestedEvent> m_nextLevelPub;
        private readonly IPublisher<ReturnToMenuRequestedEvent> m_menuPub;

        public GameResultPresenter(
            GameResultView view,
            IPublisher<RestartRequestedEvent> restartPub,
            IPublisher<NextLevelRequestedEvent> nextLevelPub,
            IPublisher<ReturnToMenuRequestedEvent> menuPub
        )
        {
            m_view = view;
            m_restartPub = restartPub;
            m_nextLevelPub = nextLevelPub;
            m_menuPub = menuPub;

            m_view.OnRestartClicked += OnRestartClicked;
            m_view.OnNextLevelClicked += OnNextLevelClicked;
            m_view.OnMenuClicked += OnMenuClicked;
        }

        public void ShowWin() => m_view.ShowWin();

        public void ShowLose() => m_view.ShowLose();

        public void Hide() => m_view.Hide();

        private void OnNextLevelClicked()
        {
            m_nextLevelPub.Publish(new NextLevelRequestedEvent());
        }

        private void OnRestartClicked()
        {
            m_restartPub.Publish(new RestartRequestedEvent());
        }

        private void OnMenuClicked()
        {
            m_menuPub.Publish(new ReturnToMenuRequestedEvent());
        }

        public void Dispose()
        {
            m_view.OnRestartClicked -= OnRestartClicked;
            m_view.OnNextLevelClicked -= OnNextLevelClicked;
            m_view.OnMenuClicked -= OnMenuClicked;
        }
    }
}
